using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI.Controls;
using System.Configuration;
using System.Data;
using System.Windows;

namespace WPFArcGISApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Note: it is not best practice to store API keys in source code.
            // The API key is referenced here for the convenience of this tutorial.
            Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.ApiKey = "AAPKb965a244c5de4ccdbac2c1af7aed9a160S2VFaaMzk2R_1M41OZjD-iKTWrlMFWHaXedZt_CITdfVNwRxZttpo_p1PMeSlsW";
            var map = new Map(BasemapStyle.ArcGISNavigation);

        }
    }

}
