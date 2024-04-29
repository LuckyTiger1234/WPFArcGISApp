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
using System;
using WPFArcGISApp.ViewModel;
using Esri.ArcGISRuntime.Geometry;
namespace WPFArcGISApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // arcgis map
        public MainWindow()
        {
            InitializeComponent();

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

            SceneViewModel sceneViewModel = new SceneViewModel(MainSceneView);
            this.DataContext = sceneViewModel;
        }


    }
}