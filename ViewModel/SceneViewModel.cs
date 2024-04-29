using CommunityToolkit.Mvvm.Input;

using System;
using System.Collections.Generic;
using System.Text;

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

namespace WPFArcGISApp.ViewModel
{

    class SceneViewModel : INotifyPropertyChanged
    {
        private SceneView _sceneView;
        // 创建图层，保存Polyline Graphic
        private GraphicsOverlay LineOverlay;
        private MapPoint _startPoint;
        private MapPoint _endPoint;
        private bool _isDrawingLine = false;
        // 绑定按钮点击事件
        public ICommand DrawLineCommand { get; private set; }

        public SceneViewModel(SceneView sceneView)
        {
            _sceneView = sceneView;
            LineOverlay = new GraphicsOverlay();
            DrawLineCommand = new RelayCommand(DrawLine);
            _sceneView.MouseUp += OnSceneViewMouseUp;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Scene? _scene;
        public Scene? Scene
        {
            get { return _scene; }
            set
            {
                _scene = value;
                OnPropertyChanged();
            }
        }

        private void DrawLine()
        {
            _isDrawingLine = true;
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
                // 创建 PolylineBuilder
                PolylineBuilder polylineBuilder = new PolylineBuilder(SpatialReferences.Wgs84);

                // 添加起点和终点
                polylineBuilder.AddPoint(_startPoint);
                polylineBuilder.AddPoint(_endPoint);

                // 创建 Polyline
                Polyline polyline = polylineBuilder.ToGeometry();

                // 将线条添加到图层中
                SimpleLineSymbol lineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Color.Red, 2);
                // 创建Graphic并添加到GraphicsOverlay
                Graphic lineGraphic = new Graphic(polyline, lineSymbol);
                // 设置线条 Graphic 的 SurfacePlacement 为 Draped
                //LineOverlay.SceneProperties.SurfacePlacement = SurfacePlacement.Draped;
                LineOverlay.Graphics.Add(lineGraphic);
                _sceneView.GraphicsOverlays.Add(LineOverlay);

                // 重置状态变量和起点终点
                _isDrawingLine = false;
                _startPoint = null;
                _endPoint = null;
            }
        }

        // 点击按钮画线事件
        //private void DrawLine_Click()
        //{

        //    _isDrawingLine = true;
        //    // 添加鼠标点击事件监听器
        //    //sceneView.Mouse

        //    // 创建 PolylineBuilder
        //    PolylineBuilder polylineBuilder = new PolylineBuilder(SpatialReferences.Wgs84);

        //    // 创建线条的几何体
        //    List<MapPoint> points = new List<MapPoint>
        //    {
        //        new MapPoint(-116.804, 33.909),
        //        new MapPoint(-119.804, 32.909),
        //        // 添加更多点来定义线条路径
        //    };
        //    Polyline polyline = new PolylineBuilder(points).ToGeometry();
        //    // 设置线条符号
        //    SimpleLineSymbol lineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Color.Red, 2);
        //    // 创建Graphic并添加到GraphicsOverlay
        //    Graphic lineGraphic = new Graphic(polyline, lineSymbol);
        //    // 设置线条 Graphic 的 SurfacePlacement 为 Draped
        //    //LineOverlay.SceneProperties.SurfacePlacement = SurfacePlacement.Draped;
        //    LineOverlay.Graphics.Add(lineGraphic);
        //    _sceneView.GraphicsOverlays.Add(LineOverlay);
        //    MessageBox.Show("Draw Line button clicked!");
        //}
    }

}

