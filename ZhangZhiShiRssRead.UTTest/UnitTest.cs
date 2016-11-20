using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ZhangZiShiRssRead;
using System.Threading.Tasks;

namespace ZhangZhiShiRssRead.UTTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestDownloadRss()
        {
            var rssReader = new RssReader();
            var result = await rssReader.DownloadRssString();
            Assert.IsNotNull(result);
            Assert.AreNotEqual<string>(string.Empty, result);
        }

        [TestMethod]
        public async Task TestParseRssXml()
        {
            var rssReader = new RssReader();
            var result = await rssReader.DownloadRssString();
            var rssNode = rssReader.GetRssNode(result);
            var items = rssReader.ParseRss(rssNode);

            Assert.IsTrue(items.Count > 0);
        }

        [TestMethod]
        public async Task TestSaveFile()
        {
            var fileStoreHelper = new FileStoreHelper();
            var rssReader = new RssReader();
            var result = await rssReader.DownloadRssString();
            var isSuccess = await fileStoreHelper.SaveRssFileAsync(result);

            Assert.IsTrue(isSuccess);
        }

        [TestMethod]
        public async Task TestReadFile()
        {
            var fileStoreHelper = new FileStoreHelper();
            var rssReader = new RssReader();
            var result = await rssReader.DownloadRssString();
            await fileStoreHelper.SaveRssFileAsync(result);
            var content = await fileStoreHelper.ReadRssFileAsync();

            Assert.AreEqual<string>(result, content);
        }
    }
}
