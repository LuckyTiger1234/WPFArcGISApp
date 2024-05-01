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

namespace WPFArcGISApp.ViewModel
{

    class SceneViewModel : INotifyPropertyChanged
    {
        private SceneView _sceneView;
        private Surface _surface;
        private WebView2 _webView;


        // 创建图层，保存Polyline Graphic
        private GraphicsOverlay LineOverlay;
        private MapPoint _startPoint;
        private MapPoint _endPoint;

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
        public ICommand ClearLineCommand { get; private set; }

        public SceneViewModel(SceneView sceneView, Surface elevationSurface, WebView2 webView)
        {
            _sceneView = sceneView;
            _surface = elevationSurface;
            _webView = webView;

            LineOverlay = new GraphicsOverlay();
            // 绑定butoon事件
            DrawLineCommand = new RelayCommand(DrawLine);
            // 绑定对球操作事件
            _sceneView.MouseUp += OnSceneViewMouseUp;

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
                _sceneView.GraphicsOverlays.Remove(LineOverlay);
                // 重置状态变量和起点终点
                _isDrawingLine = false;
                _startPoint = null;
                _endPoint = null;
                LineOverlay = new GraphicsOverlay();
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
            if (_startPoint == null)
            {
                // 第一次点击，设置起点
                _startPoint = clickedPoint;
            }
            else
            {
                // 第二次点击，设置终点，并绘制直线
                _endPoint = clickedPoint;

                // 绘制线条
                PolylineBuilder polylineBuilder = new PolylineBuilder(SpatialReferences.Wgs84);
                polylineBuilder.AddPoint(_startPoint);
                polylineBuilder.AddPoint(_endPoint);
                // 创建 Polyline
                Polyline polyline = polylineBuilder.ToGeometry();

                // 获取线的剖面信息
                getElevationfromLine(polyline, 100);

                // 将线条添加到图层中
                SimpleLineSymbol lineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Color.Red, 2);
                Graphic lineGraphic = new Graphic(polyline, lineSymbol);

                LineOverlay.Graphics.Add(lineGraphic);
                _sceneView.GraphicsOverlays.Add(LineOverlay);

                _isDrawingLine = false;
                ButtonContent = "清除";
            }
        }

        /// <summary>
        /// 对polyline直线按照线性插值，取线上的点的高程
        /// </summary>
        /// <param name="line">polyline 线</param>
        /// <param name="pointsNum">int 插值数量</param>
        private async void getElevationfromLine(Polyline line, int pointsNum)
        {
            List<double> elevations = new List<double>();
            // 计算插值步长
            double step = line.Length() / (pointsNum - 1);

            for(int i=0; i < pointsNum; i++)
            {
                // 计算当前插值点的位置
                double distance = step * i;
                MapPoint interpolatedPoint = GeometryEngine.Project(
                    GeometryEngine.CreatePointAlong(line, distance),
                    SpatialReferences.Wgs84) as MapPoint;
                // 获取插值点的高程信息
                double elevation = await _surface.GetElevationAsync(interpolatedPoint);
                elevations.Add(elevation);
            }
            Console.WriteLine("\"length\"",elevations.Count);


            _webView.Visibility = Visibility.Visible;

            // 将海拔数据传递给 WebView2，并绘制折线图
            String script = $"drawElevationChart({JsonConvert.SerializeObject(elevations)})";
            await _webView.ExecuteScriptAsync(script);
        }

    }

}

