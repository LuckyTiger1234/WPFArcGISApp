using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI.Controls;
using Microsoft.Web.WebView2.Core;

namespace WPFArcGISApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // arcgis map

        // webview2
        public MainWindow()
        {
            InitializeComponent();
            webView.NavigationStarting += EnsureHttps;
            InitializeAsync();
            InitBasemap();
        }
        async void InitializeAsync()
        {
            await webView.EnsureCoreWebView2Async(null);
            webView.CoreWebView2.WebMessageReceived += UpdateAddressBar;

            await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.postMessage(window.document.URL);");
            await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.addEventListener(\'message\', event => alert(event.data));");
        }
        void UpdateAddressBar(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            String uri = args.TryGetWebMessageAsString();
            addressBar.Text = uri;
            webView.CoreWebView2.PostWebMessageAsString(uri);
        }
        void ButtonGo_Click(object sender, RoutedEventArgs e)
        {
            if (webView != null && webView.CoreWebView2 != null)
            {
                webView.CoreWebView2.Navigate(addressBar.Text);
            }
        }
        void EnsureHttps(object sender, CoreWebView2NavigationStartingEventArgs args)
        {
            String uri = args.Uri;
            if (!uri.StartsWith("https://"))
            {
                webView.CoreWebView2.ExecuteScriptAsync($"alert('{uri} is not safe, try an https link')");
                args.Cancel = true;
            }
        }

        private const string TianMapBaseMapUri = "http://t2.tianditu.gov.cn/vec_w/wmts?SERVICE=WMTS&REQUEST=GetTile&VERSION=1.0.0&LAYER=vec&STYLE=default&TILEMATRIXSET=w&FORMAT=tiles&TILEMATRIX={level}&TILEROW={row}&TILECOL={col}&tk=fa9bb502614cf3335b92e50f6fe0c371";

        private const string TianMapLabelUri = "http://t2.tianditu.gov.cn/cva_w/wmts?SERVICE=WMTS&REQUEST=GetTile&VERSION=1.0.0&LAYER=cva&STYLE=default&TILEMATRIXSET=w&FORMAT=tiles&TILEMATRIX={level}&TILEROW={row}&TILECOL={col}&tk=fa9bb502614cf3335b92e50f6fe0c371";

        private void InitBasemap()
        {
            Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.ApiKey = "AAPKb965a244c5de4ccdbac2c1af7aed9a160S2VFaaMzk2R_1M41OZjD-iKTWrlMFWHaXedZt_CITdfVNwRxZttpo_p1PMeSlsW";
            var map = new Map(BasemapStyle.ArcGISNavigation);
            MapView.Map = map;
            //加载天地图 矢量底图
            //var webtileBaseLayer = new WebTiledLayer(TianMapBaseMapUri);
            //MapView.Map.Basemap?.BaseLayers.Add(webtileBaseLayer);

            ////加载天地图 矢量注记
            //var webtileNodeLayer = new WebTiledLayer(TianMapLabelUri);
            //MapView.Map.OperationalLayers.Add(webtileNodeLayer);

            MapView.SetViewpoint(new Viewpoint(
                latitude: 34.2784,
                longitude: 108.9414,
                scale: 120000));

        }
    }
}