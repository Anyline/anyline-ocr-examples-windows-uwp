using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Anyline.SDK;
using Anyline.SDK.Plugins;
using Anyline.SDK.PluginConfigs;
using Newtonsoft.Json.Linq;
using Windows.UI.Popups;

namespace AnylineExamples
{
    public sealed partial class ScanPage : Page
    {
        public ScanPage()
        {
            this.InitializeComponent();
        }

        // Initialize the scan view here
        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            var example = args.Parameter as Example;
            ApplicationView.GetForCurrentView().Title = example.Name;

            try
            {
                VinConfig myVinConfig = new VinConfig();                

                PluginConfig myPluginConfig = new PluginConfig();
                myPluginConfig.Id = "TEST";
                myPluginConfig.VinConfig = myVinConfig;

                ScanPluginConfig myScanPluginConfig = new ScanPluginConfig(myPluginConfig);
                ScanPlugin myScanPlugin = new ScanPlugin(myScanPluginConfig);

                if (example.CustomizedJSONConfig != null)
                {
                    JObject jsonConfig = JObject.Parse(example.CustomizedJSONConfig);

                    // Initialize the scan view with a JSON config.
                    await AnylineScanView.InitAsync(jsonConfig);
                }
                else
                {
                    // Initialize the scan view with a given path to the json config file, relative to the project directory.
                    // Be sure to include JSON files as 'Content'.
                    await AnylineScanView.InitAsync(example.RelativeFilePath);
                }

                Debug.WriteLine("Anyline Initialization successful");

                // once the scan view is initialized, we can hook on the plugin events
                // depending on the configuration, the ViewPlugin is either a
                // ViewPluginComposite or a ScanViewPlugin
                if (AnylineScanView.ScanViewPlugin is Anyline.SDK.ViewPlugins.ScanViewPlugin scanViewPlugin)
                {
                    // A ScanViewPlugin holds a ScanPlugin which is the scanning component and delivers us a scan result
                    scanViewPlugin.ScanPlugin.ResultReceived += ScanPlugin_ResultReceived;
                } else if (AnylineScanView.ScanViewPlugin is Anyline.SDK.ViewPlugins.ViewPluginComposite viewPluginComposite)
                {
                    // A ViewPluginComposite holds multiple ScanViewPlugins, which run either sequentially or in parallel
                    // In this case, we simply want to get all scan results once the workflow is done
                    viewPluginComposite.AllResultsReceived += ViewPluginComposite_AllResultsReceived;
                }

                // register to camera events
                AnylineScanView.CameraView.CameraOpened += CameraView_CameraOpened;
                AnylineScanView.CameraView.CameraClosed += CameraView_CameraClosed;

                // if we minimize the window, we want to close the camera
                // - if we maximize, we want to open it again
                Window.Current.VisibilityChanged += Current_VisibilityChanged;

                // start the camera
                AnylineScanView.StartCamera();
            }
            catch(Exception e)
            {
                Debug.WriteLine("Unable to initialize Anyline: " + e.Message + ((e.InnerException != null) ? $" ({e.InnerException.Message})" : ""));

                    MessageDialog dialog = new MessageDialog(e.ToString(), e.GetType().Name);
                    await dialog.ShowAsync();

            }
        }

        private async void Current_VisibilityChanged(object sender, Windows.UI.Core.VisibilityChangedEventArgs args)
        {
            if (args.Visible)
            {
                AnylineScanView.StartCamera();
            } else
            {
                await AnylineScanView.StopCameraAsync();
            }
        }

        private void ViewPluginComposite_AllResultsReceived(object sender, List<Anyline.SDK.Plugins.ScanResult> scanResults)
        {
            NavigateToResultPageInUIThread(scanResults);
        }

        private void ScanPlugin_ResultReceived(object sender, Anyline.SDK.Plugins.ScanResult scanResult)
        {
            var scanResults = new List<Anyline.SDK.Plugins.ScanResult> { scanResult };

            NavigateToResultPageInUIThread(scanResults);
        }

        private async void NavigateToResultPageInUIThread(List<Anyline.SDK.Plugins.ScanResult> scanResults)
        {
            // we have to trigger the navigation on the UI thread, otherwise Window.Current is null
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                // navigate to the result page
                (Window.Current.Content as Frame).Navigate(typeof(ResultPage), scanResults, new SuppressNavigationTransitionInfo());
            });
        }

        private void CameraView_CameraOpened(object sender, Size resolutionSize)
        {
            try
            {
                // start scanning once the camera is opened
                AnylineScanView.StartScanning();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unable to start scanning: " + e.Message);
            }
        }

        private void CameraView_CameraClosed(object sender, bool closedSuccessfully)
        {
            // stop scanning when the camera is closed
            AnylineScanView.StopScanning();
        }

        // clean up any dependencies here
        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs args)
        {
            base.OnNavigatingFrom(args);
            
            // unregister events
            Window.Current.VisibilityChanged -= Current_VisibilityChanged;

            await AnylineScanView.StopCameraAsync();

            // IMPORTANT: call dispose to clean up all resources!
            AnylineScanView.Dispose();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            base.OnNavigatedFrom(args);

            GC.Collect();
        }
    }
}
