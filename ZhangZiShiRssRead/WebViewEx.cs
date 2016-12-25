using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ZhangZiShiRssRead
{
    public class WebViewEx
    {
        public static string GetUri(DependencyObject obj)
        {
            return (string)obj.GetValue(UriProperty);
        }

        public static void SetUri(DependencyObject obj, string value)
        {
            obj.SetValue(UriProperty, value);
        }

        // Using a DependencyProperty as the backing store for WebViewUri.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UriProperty =
            DependencyProperty.RegisterAttached("Uri", typeof(string), typeof(WebViewEx), new PropertyMetadata(null,PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var webView = d as WebView;
            if (e.NewValue != null)
            {
                string newValue = e.NewValue.ToString();
                if (string.IsNullOrEmpty(newValue) == false)
                {
                    if (Window.Current.Bounds.Width > 800)
                    {
                        newValue = newValue.Replace("style=\"max-width:100%; custom:OnlyForImage\"", "style=\"max-width:60%; custom:OnlyForImage\"");
                    }
                    webView.NavigateToString(newValue);
                }
            }
        }
    }
}
