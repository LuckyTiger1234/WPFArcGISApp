using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Newtonsoft.Json;
using System;

namespace WPFArcGISApp.ViewModel
{
    class WebViewModel
    {
        private WebView2 _webView;

        public WebViewModel(WebView2 webView)
        {
            _webView = webView;
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            await _webView.EnsureCoreWebView2Async(null);
            // 在webview控件增加获取点信息的监听
            _webView.CoreWebView2.WebMessageReceived += getLinePoint;
        }

        // 监听webview控件发送的消息
        void getLinePoint(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            String paramsStr = e.TryGetWebMessageAsString();

            dynamic paramsJson = JsonConvert.DeserializeObject (paramsStr);
            Console.WriteLine("\"linePointStr\"", paramsJson);
            
            int intIndex = paramsJson.pointIndex;
            string eventType = paramsJson.eventType;
            switch (eventType)
            {
                case "onHover":
                    SceneViewModel.drawHoverPoint(intIndex);
                    break;

                case "onClick":
                    SceneViewModel.moveCamera(intIndex);
                    break;

                default:
                    break;
            }
        }
    }
}
