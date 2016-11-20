using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Web.Http;

namespace ZhangZiShiRssRead
{
    public class RssReader
    {
        private const string LastBuildDate = "lastBuildDate";
        private const string Channel = "channel";
        private const string Img = "img";
        private const string Title = "title";

        private Dictionary<string, string> XmlNameSpaceDic { get; set; } = new Dictionary<string, string>();

        public async Task<string> DownloadRssString()
        {
            var httpClient = new HttpClient();
            var result = await httpClient.GetStringAsync(new Uri("http://www.zhangzishi.cc/feed"));
            return result;
        }

        internal bool HasNewItems(XElement rssNode)
        {
            var newValue = rssNode.Element(Channel).Element(LastBuildDate).Value;

            var localSettings = ApplicationData.Current.LocalSettings;
            var oldValue = localSettings.Values[LastBuildDate];
            if (oldValue == null)
            {
                localSettings.Values[LastBuildDate] = newValue;
                return true;
            }
            else
            {
                var lastBuildDate = DateTime.Parse(newValue);
                var oldBuildDate = DateTime.Parse(localSettings.Values[LastBuildDate].ToString());
                if (lastBuildDate > oldBuildDate)
                {
                    localSettings.Values[LastBuildDate] = newValue;
                    return true;
                }
                else
                {
                    return false;
                }
            }

           
        }

        public XElement GetRssNode(string rssContent)
        {
            var document = XDocument.Parse(rssContent);
            return document.Element("rss");
        }

        public List<Item> ParseRss(XElement rssNode)
        {
            var items = new List<Item>();
            //var document = XDocument.Parse(rrs);
            //var rssNode = document.Element("rss");

            //if (HasNewItems(rssNode) == false)
            //{
            //    return items;
            //}

            XmlNameSpaceDic = GetXmlNameSpaceDic(rssNode);
            var itemNodeList = rssNode.Element(Channel).Elements("item");

            foreach (var itemNode in itemNodeList)
            {
                var item = ParseItemNode(itemNode);
                if (item != null)
                {
                    items.Add(item);
                }
            }

            return items;
        }

        private Dictionary<string, string> GetXmlNameSpaceDic(XElement rssNode)
        {
            var dic = new Dictionary<string, string>();
            foreach (var attribute in rssNode.Attributes().Where(_ => _.IsNamespaceDeclaration))
            {
                dic.Add(attribute.Name.LocalName,attribute.Value);
            }

            return dic;
        }

        private Item ParseItemNode(XElement itemNode)
        {
            var item = new Item();
            item.Title = itemNode.Element("title").Value;
            string uriString = itemNode.Element("link").Value;
            if (string.IsNullOrEmpty(uriString) == false)
            {
                item.Link = new Uri(uriString);
            }
            item.PublishedDate = DateTime.Parse(itemNode.Element("pubDate").Value);

            XNamespace dc = XmlNameSpaceDic["dc"];
            item.Creator = itemNode.Element(dc + "creator").Value;
            item.Category = itemNode.Element("category").Value;
            item.Description = itemNode.Element("description").Value;
            XNamespace content = XmlNameSpaceDic["content"];
            var contentEncoded = itemNode.Element(content + "encoded").Value;
            
            var allImageUri = GetAllImageUri(ref contentEncoded);
            item.CoverImageUri = allImageUri.FirstOrDefault();
            item.ContentEncoded = RemoveEmbedFlash(contentEncoded);
            return item;
        }

        private List<string> GetAllImageUri(ref string content)
        {
            var matchList = new List<string>();
            string pattern = "<img.+?src=[\"'](.+?)[\"'].*?>";

            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            foreach (Match match in regex.Matches(content))
            {
                var uri = EditImageUri(match.Value);
                if (uri != match.Value)
                {
                    matchList.Add(match.Groups[1].Value);
                    content = content.Replace(match.Value, uri);
                }
            }

            return matchList;
        }

        private string EditImageUri(string imageUri)
        {
            var doc = XDocument.Parse(imageUri);
            var imgElement = doc.Element(Img);

            var imgTitle = imgElement.Attribute(Title);
            if (imgTitle != null && string.IsNullOrEmpty(imgTitle.Value) == false)
            {
                return imageUri;
            }

            imgElement.SetAttributeValue("width", "100%");
            imgElement.SetAttributeValue("height", "auto");
            var displayInfo = DisplayInformation.GetForCurrentView();
            var scaleFactor = displayInfo.RawPixelsPerViewPixel;
            var ppi = displayInfo.RawDpiX;
            imgElement.SetAttributeValue("style", "max-width:" + ppi * scaleFactor + "px");

            return imgElement.ToString();
        }

        private string RemoveEmbedFlash(string contentEncoded)
        {
            string pattern = "<embed.+?type=\"application/x-shockwave-flash\".*?>";

            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var result = regex.Match(contentEncoded);
            if (result.Success)
            {
                return contentEncoded.Replace(result.Value, string.Empty);
            }
            else
            {
                return contentEncoded;
            }
        }
    }
}
