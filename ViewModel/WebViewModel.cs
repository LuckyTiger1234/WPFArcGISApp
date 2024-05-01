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

        }
    }
}
