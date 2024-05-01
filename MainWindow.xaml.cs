
using System.Windows;
using Esri.ArcGISRuntime.Mapping;
using Microsoft.Web.WebView2.Core;
using System;
using WPFArcGISApp.ViewModel;
using Esri.ArcGISRuntime.Geometry;
using System.IO;
namespace WPFArcGISApp
{
    public partial class MainWindow : Window
    {
        // arcgis map
        public MainWindow()
        {
            InitializeComponent();

            // 初始化webview
            string openUrl = Directory.GetCurrentDirectory() + "/EchartWebView.html";
            webView.Source = new Uri(openUrl);

            // Create a new scene with an imagery basemap.
            Scene scene = new Scene(BasemapStyle.ArcGISImageryStandard);
            string elevationServiceUrl = "http://elevation3d.arcgis.com/arcgis/rest/services/WorldElevation3D/Terrain3D/ImageServer";
            ArcGISTiledElevationSource elevationSource = new ArcGISTiledElevationSource(new Uri(elevationServiceUrl));
            Surface elevationSurface = new Surface();
            elevationSurface.ElevationSources.Add(elevationSource);
            elevationSurface.ElevationExaggeration = 2.5;
            scene.BaseSurface = elevationSurface;
            MapPoint cameraLocation = new MapPoint(-118.804, 33.909, 5330.0, SpatialReferences.Wgs84);

            Camera sceneCamera = new Camera(locationPoint: cameraLocation,
                                  heading: 355.0,
                                  pitch: 72.0,
                                  roll: 0.0);

            MapPoint sceneCenterPoint = new MapPoint(-118.805, 34.027, SpatialReferences.Wgs84);

            // Set an initial viewpoint for the scene using the camera and observation point.
            Viewpoint initialViewpoint = new Viewpoint(sceneCenterPoint, sceneCamera);
            scene.InitialViewpoint = initialViewpoint;

            MainSceneView.Scene = scene;

            webView.Visibility = Visibility.Collapsed;
            webView.EnsureCoreWebView2Async();

            WebViewModel webViewModel = new WebViewModel(webView);
            SceneViewModel sceneViewModel = new SceneViewModel(MainSceneView, elevationSurface, webView);
            this.DataContext = sceneViewModel;

        }
    }
}