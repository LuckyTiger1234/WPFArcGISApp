﻿using Microsoft.Web.WebView2.Core;
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
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            await _webView.EnsureCoreWebView2Async(null);
            // 在webview控件增加获取点信息的监听
            _webView.CoreWebView2.WebMessageReceived += getLinePoint;
            // 注册 JavaScript 交互对象
            //await _webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.external = window");
        }

        void getLinePoint(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            String linePointIndex = e.TryGetWebMessageAsString();
            Console.WriteLine("\"linePointStr\"", linePointIndex);

        }
    }
}
