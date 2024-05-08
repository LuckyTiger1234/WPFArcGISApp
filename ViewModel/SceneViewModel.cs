using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using Esri.ArcGISRuntime.Symbology;
using System.Drawing;
using System.Linq;
using Microsoft.Web.WebView2.Wpf;

using Newtonsoft.Json;
using System.Windows.Controls;
using System.Net;

namespace WPFArcGISApp.ViewModel
{

    class SceneViewModel : INotifyPropertyChanged
    {
        private SceneView _sceneView;
        private Surface _surface;
        private WebView2 _webView;
        private ProgressBar _loadingBar;
        private TextBlock _loadingText;

        // 创建图层，保存Polyline Graphic
        private GraphicsOverlay _LineOverlay;
        private GraphicsOverlay _PointOverlay;
        private List<MapPoint> _clickedPoints = new List<MapPoint>();

        // 差值点
        private List<MapPoint> _interpolatePoints = new List<MapPoint>();
        private List<double> _elevations = new List<double>();
        private List<int> _distances = new List<int>();

        // 通过该变量控制绘制状态
        private bool _isDrawingLine = false;

        private string _buttonContent = "剖面分析工具";
        
        // 绑定button content
        public string ButtonContent
        {
            get { return _buttonContent; }
            set
            {
                if (_buttonContent != value)
                {
                    _buttonContent = value;
                    OnPropertyChanged();
                }
            }
        }
        // 绑定按钮点击事件
        public ICommand DrawLineCommand { get; private set; }

        public SceneViewModel(
            SceneView sceneView, 
            Surface elevationSurface, 
            WebView2 webView,
            ProgressBar progressBar,
            TextBlock textBlock
            )
        {
            _sceneView = sceneView;
            _surface = elevationSurface;
            _webView = webView;
            _loadingBar = progressBar;
            _loadingText = textBlock;

            _LineOverlay = new GraphicsOverlay();
            _PointOverlay = new GraphicsOverlay();

            // 添加点图层到场景视图
            _sceneView.GraphicsOverlays.Add(_PointOverlay);
            _sceneView.GraphicsOverlays.Add(_LineOverlay);

            // 绑定butoon事件
            DrawLineCommand = new RelayCommand(DrawLine);
            // 绑定对球操作事件
            _sceneView.MouseUp += OnSceneViewMouseUp;
            _sceneView.MouseDoubleClick += OnSceneViewDoubleClick;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void DrawLine()
        {
            if (ButtonContent == "清除") 
            {
                // 清除
                _PointOverlay.Graphics.Clear();
                _LineOverlay.Graphics.Clear();
                _clickedPoints.Clear();
                _interpolatePoints.Clear();
                _distances.Clear();
                _elevations.Clear();

                _isDrawingLine = false;
                _webView.Visibility = Visibility.Collapsed;
                ButtonContent = "剖面分析工具";
            }
            else
            {
                // 绘制
                _isDrawingLine = true;
            }
        }


        private async void OnSceneViewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isDrawingLine)
                return;
            // 获取鼠标点击的地理坐标点
            MapPoint clickedPoint = await _sceneView.ScreenToLocationAsync(e.GetPosition(_sceneView));
            SimpleMarkerSymbol PointSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, Color.Blue, 8);

            // 添加到图层中
            Graphic pointGraphic = new Graphic(clickedPoint, PointSymbol);
            _PointOverlay.Graphics.Add(pointGraphic);
            // 将点击的点添加到列表中
            _clickedPoints.Add(clickedPoint);
        }

        private async void OnSceneViewDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // 获取鼠标点击的地理坐标点
            MapPoint clickedPoint = await _sceneView.ScreenToLocationAsync(e.GetPosition(_sceneView));
            SimpleMarkerSymbol PointSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, Color.Blue, 8);

            // 添加到图层中
            Graphic pointGraphic = new Graphic(clickedPoint, PointSymbol);
            _PointOverlay.Graphics.Add(pointGraphic);

            // 将点击的点添加到列表中
            _clickedPoints.Add(clickedPoint);

            if (_clickedPoints.Count < 2)
            {
                MessageBox.Show("至少需要两个点才能绘制折线。");
                return;
            }

            // 将线条添加到图层中
            SimpleLineSymbol lineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Color.Red, 2);
            Polyline line = new Polyline(_clickedPoints);
            Graphic polylineGraphic = new Graphic(line, lineSymbol);

            _LineOverlay.Graphics.Add(polylineGraphic);

            _isDrawingLine = false;
            ButtonContent = "清除";

            drawChart(line, 10);

        }

        /// <summary>
        /// 对polyline直线按照线性插值，取线上的点的高程,数据发送到webview控件，绘制折线图
        /// </summary>
        /// <param name="line">polyline 线</param>
        /// <param name="stepLen">int 差值步长（m）</param>
        private async void drawChart(Polyline line, int stepLen)
        {
            _loadingBar.Visibility = Visibility.Visible;
            _loadingText.Visibility = Visibility.Visible;



            // 获取polyline长度, 根据步长确定差值数量
            double length = GeometryEngine.LengthGeodetic(line, linearUnit: LinearUnits.Meters, geodeticCurveType: GeodeticCurveType.Geodesic);

            int interpolateNum =(int)length / stepLen;

            // 计算插值步长
            double step = line.Length() / (interpolateNum - 1);
            for (int i = 0; i < interpolateNum; i++)
            {
                // 计算当前插值点的位置
                double distance = step * i;
                MapPoint interpolatedPoint = GeometryEngine.Project(
                    GeometryEngine.CreatePointAlong(line, distance),
                    SpatialReferences.Wgs84) as MapPoint;

                // 获取插值点的高程信息
                double elevation = await _surface.GetElevationAsync(interpolatedPoint);
                GeodeticDistanceResult distanceResult = GeometryEngine.DistanceGeodetic(_clickedPoints[0], interpolatedPoint, null, null, GeodeticCurveType.Geodesic);
                int distanceInt = (int)distanceResult.Distance;
                elevation = Math.Round(elevation, 2);

                _interpolatePoints.Add(interpolatedPoint);
                _elevations.Add(elevation);
                _distances.Add(distanceInt);
            }

            
               
            // 将海拔数据传递给 WebView2，并绘制折线图
            String script = $"drawElevationChart({JsonConvert.SerializeObject(_elevations)},{JsonConvert.SerializeObject(_distances)} );" +
                $"updateStatics({_elevations.Max()},{_elevations.Min()},{Math.Round(_elevations.Average())},{Math.Round(CalculateGain(_elevations), 2)},{Math.Round(CalculateLoss(_elevations), 2)})";
            await _webView.ExecuteScriptAsync(script);

            _webView.Visibility = Visibility.Visible;
            _loadingBar.Visibility = Visibility.Collapsed;
            _loadingText.Visibility = Visibility.Collapsed;
        }

        // 计算gain
        static double CalculateGain(List<double> numbers)
        {
            double gain = 0;
            for (int i = 1; i < numbers.Count; i++)
            {
                double diff = numbers[i] - numbers[i - 1];
                if (diff > 0)
                    gain += diff;
            }
            return gain;
        }
        // 计算loss
        static double CalculateLoss(List<double> numbers)
        {
            double loss = 0;
            for (int i = 1; i < numbers.Count; i++)
            {
                double diff = numbers[i] - numbers[i - 1];
                if (diff < 0)
                    loss -= diff; // 取负数表示减少值的总和
            }
            return loss;
        }

    }
}

