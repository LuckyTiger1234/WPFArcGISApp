# 基于WPF的ArcGIS Maps windows桌面端应用

## 环境依赖
.NET 8.0
WebView2 1.0.2478.35

## 目录说明
    ├─ /WPFArcGISApp                    项目代码
    ├─ README.md                        node编译脚本


## 注意事项：
1.安装完Arcgis Map SDK nuget后，出现报错：The 'Esri.ArcGISRuntime.WPF' nuget package cannot be used to target 'net8.0-windows'. Target 'net8.0-windows10.0.19041.0' or later instead。
解决方法：右击项目，编辑选择项目文件，更新 <TargetFramework> element with net8.0-windows10.0.19041.0 （或更高版本）
