# 基于WPF的ArcGIS Maps windows桌面端应用

## 环境依赖
.NET 8.0 + WebView2 1.0.2478.35

## 目录说明
    ├─ /Properties                    打包配置文件
    ├─ /ViewModel                     VM 层文件
    ├─ MainWindow.xaml                主窗口
    ├─ EchartWebView.html             WebView控件对应html
    ├─ README.md                        

## 操作说明：
1.使用 visio studio 2022及以上版本打开项目，运行。  
2.等待程序初始化（如未出现三维球请重新运行）。   
3.点击 剖面分析工具 按钮，在三维球上点击两点，会在三维球中画出直线并在下方显示对应的剖面信息；点击清除可删除结果；再次点击 剖面分析工具 重复之前操作可继续使用。

## 注意事项：
1.安装完Arcgis Map SDK nuget后，出现报错：The 'Esri.ArcGISRuntime.WPF' nuget package cannot be used to target 'net8.0-windows'. Target 'net8.0-windows10.0.19041.0' or later instead。
解决方法：右击项目，编辑选择项目文件，更新 <TargetFramework> element with net8.0-windows10.0.19041.0 （或更高版本）


2.运行后，如出现WebView控件找不到资源文件，需手动把EchartWebView.html文件复制到与exe同级目录下。    
