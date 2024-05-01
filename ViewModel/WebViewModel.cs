using Microsoft.Web.WebView2.Wpf;
using System;
using System.IO;

namespace WPFArcGISApp.ViewModel
{
    class WebViewModel
    {
        private WebView2 _webView;

        public WebViewModel(WebView2 webView)
        {
            _webView = webView;
            //_webView.EnsureCoreWebView2Async();
            //// 将html与webview关联
            //String htmlContent = File.ReadAllText("../../../EchartWebView.html");
            //Console.WriteLine(htmlContent);
            //_webView.EnsureCoreWebView2Async();
            
            //webView.CoreWebView2.Navigate("https://www.baidu.com");
        }
    }
}
