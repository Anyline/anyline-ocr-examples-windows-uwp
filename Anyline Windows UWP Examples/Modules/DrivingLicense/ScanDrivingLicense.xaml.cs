using Anyline.SDK.Plugins;
using Anyline.SDK.Plugins.ID;
using Anyline.SDK.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AnylineExamplesApp.Modules.DrivingLicense
{
    public sealed partial class ScanDrivingLicense : Page, IScanResultListener<ScanResult<ID>>
    {
        private IDScanViewPlugin _scanViewPlugin;

        #region initialization
        public ScanDrivingLicense()
        {
            this.InitializeComponent();

            if (AnylineScanView != null)
            {
                AnylineScanView.CameraView.CameraOpened += CameraView_CameraOpened;
                AnylineScanView.CameraView.CameraClosed += CameraView_CameraClosed;
                AnylineScanView.CameraView.CameraError += CameraView_CameraError;

                Window.Current.VisibilityChanged -= Current_VisibilityChanged;
                Window.Current.VisibilityChanged += Current_VisibilityChanged;
            }

            ResultView.OkButton.Tapped += ResultView_Tapped;
        }

        private void ResultView_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            ResultView.Visibility = Visibility.Collapsed;
            if (AnylineScanView != null && !(_scanViewPlugin?.ScanPlugin as IDScanPlugin).IsScanning())
                AnylineScanView?.StartScanning();
        }
        #endregion

        #region navigation
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            try
            {
                // you can create everything via JSON like so:

                //AnylineScanView.Init("Modules/DrivingLicense/driving_license_view_config.json", MainPage.LicenseKey);
                //_scanViewPlugin = AnylineScanView?.ScanViewPlugin as IDScanViewPlugin;

                // or you can create the plugins via code and add them to the scan view:

                // load the BaseConfig for the ScanView
                var fileName = "Modules/DrivingLicense/driving_license_view_config.json";
                AnylineScanView.SetupWithConfig(new AnylineBaseConfig(fileName));

                // create a scanviewplugin config from the same file
                var json = Anyline.SDK.Util.ConfigUtil.GetJsonObjectFromConfigFileName(fileName);
                var scanViewPluginConfig = new ScanViewPluginConfig(json);

                // create a Scanplugin
                IDScanPlugin scanPlugin = new IDScanPlugin("DrivingLicense_PluginID_1",
                    MainPage.LicenseKey,
                    new DrivingLicenseConfig
                    {
                        ScanMode = DrivingLicenseConfig.DrivingLicenseCountry.AUTO
                    });
                // create a ScanViewPlugin with the previously created scanPlugin
                _scanViewPlugin = new IDScanViewPlugin(scanPlugin, scanViewPluginConfig);

                // attach the ScanViewPlugin as root plugin to the ScanView
                AnylineScanView.SetScanViewPlugin(_scanViewPlugin);

                if (_scanViewPlugin != null)
                    _scanViewPlugin.AddScanResultListener(this);

                // open the camera preview
                AnylineScanView.StartCamera();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        // we make sure to free all resources when leaving the page        
        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            base.OnNavigatedFrom(args);

            Window.Current.VisibilityChanged -= Current_VisibilityChanged;
            AnylineScanView?.StopCamera();
            _scanViewPlugin?.StopScanning();

            AnylineScanView = null;
        }
        #endregion

        #region camera handling
        // important because the UWP camera stream automatically shuts down when a window is minimized or the computer is locked for example
        private void Current_VisibilityChanged(object sender, Windows.UI.Core.VisibilityChangedEventArgs args)
        {
            if (args.Visible == false)
            {
                AnylineScanView.StopCamera();
            }
            if (args.Visible == true)
            {
                AnylineScanView.StartCamera();
            }
        }

        private void CameraView_CameraOpened(object sender, Size args)
        {
            Debug.WriteLine($"(APP) Camera Opened: {args.Width}x{args.Height}");

            try
            {
                ResultView.Visibility = Visibility.Collapsed;

                if (!(_scanViewPlugin?.ScanPlugin as IDScanPlugin).IsScanning())
                    AnylineScanView.StartScanning();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"(APP) {e.Message}");
            }
        }

        private void CameraView_CameraClosed(object sender, bool success)
        {
            Debug.WriteLine($"(APP) Camera Closed: {success}");

            try
            {
                if ((_scanViewPlugin?.ScanPlugin as IDScanPlugin).IsScanning())
                    AnylineScanView.StopScanning();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"(APP) {e.Message}");
            }
        }

        private void CameraView_CameraError(object sender, Exception exception)
        {
            Debug.WriteLine($"(APP) Camera Error: {exception.Message}");
        }
        #endregion

        #region result callback
        public async void OnResult(ScanResult<ID> result)
        {
            Debug.WriteLine($"(APP) Scan result: {result.Result}");

            var identification = result.Result as DrivingLicenseIdentification;
            var resultBitmap = await result.CutoutImage.GetBitmapAsync();
            
            ResultView.ResultTextBlock.FontSize = 16;
            ResultView.SetResult(resultBitmap, identification.ToString().Replace("|", " - "));
            ResultView.FadeIn();
        }
        #endregion

    }
}
