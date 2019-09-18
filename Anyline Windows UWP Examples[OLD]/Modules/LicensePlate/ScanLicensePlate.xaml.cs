using Anyline.SDK.Camera;
using Anyline.SDK.Modules.LicensePlate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace AnylineExamplesApp.Modules.LicensePlate
{
    public sealed partial class ScanLicensePlate : Page, ILicensePlateResultListener, ICameraListener
    {
        #region initialization
        public ScanLicensePlate()
        {
            NavigationCacheMode = NavigationCacheMode.Enabled;
            ((Frame)Window.Current.Content).CacheSize = 0;

            InitializeComponent();

            try
            {
                AnylineScanView.SetConfigFromAsset("Modules/LicensePlate/license_plate_view_config.json");
                AnylineScanView.InitAnyline(MainPage.LicenseKey, this);

                AnylineScanView.CameraListener = this;

                ResultView.OkButton.Tapped += ResultView_Tapped;

            }
            catch (Exception e)
            {
                new MessageDialog(e.Message, "Exception").ShowAsync().AsTask().ConfigureAwait(false);
            }

            Window.Current.VisibilityChanged += Current_VisibilityChanged;

            if (!AnylineScanView.IsCameraOpen())
                AnylineScanView.OpenCameraInBackground();
        }

        private void ResultView_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            ResultView.Visibility = Visibility.Collapsed;
            if (AnylineScanView != null && !AnylineScanView.IsScanning)
                AnylineScanView?.StartScanning();
        }

        // we do this because the UWP camera stream automatically shuts down when a window is minimized
        private async void Current_VisibilityChanged(object sender, Windows.UI.Core.VisibilityChangedEventArgs args)
        {
            if (args.Visible == false)
            {
                if (AnylineScanView.IsCameraOpen())
                    await AnylineScanView.ReleaseCameraAsync();
            }
            if (args.Visible == true)
            {
                if (!AnylineScanView.IsCameraOpen())
                    AnylineScanView.OpenCameraInBackground();
            }
        }
        #endregion

        #region navigation
        // we make sure to free all resources when leaving the page        
        protected override async void OnNavigatedFrom(NavigationEventArgs args)
        {
            base.OnNavigatedFrom(args);

            ResultView.OkButton.Tapped -= ResultView_Tapped;

            Window.Current.VisibilityChanged -= Current_VisibilityChanged;

            if (AnylineScanView != null)
            {
                AnylineScanView.CancelScanning();
                await AnylineScanView.ReleaseCameraAsync();
            }
            AnylineScanView = null;
        }
        #endregion

        #region camera callbacks
        public void OnCameraClosed(bool success)
        {
            var s = success ? "sucessfully." : "with an error";
            Debug.WriteLine($"Camera closed {s}");

            // we cancel scanning when the camera is closed
            if (AnylineScanView != null)
                AnylineScanView.CancelScanning();
        }

        public void OnCameraError(Exception e)
        {
            Debug.WriteLine($"A camera error occurred: {e.Message}.");
            AnylineScanView?.CancelScanning();
        }

        public void OnCameraOpened(uint width, uint height)
        {
            Debug.WriteLine($"Camera opened: {width}x{height}.");
            ResultView.Visibility = Visibility.Collapsed;

            // As soon as the camera is opened, we start scanning
            if (AnylineScanView != null)
                AnylineScanView.StartScanning();
        }
        #endregion

        #region result callback        
        public async void OnResult(LicensePlateResult scanResult)
        {
            var resultBitmap = await scanResult.CutoutImage.GetBitmapAsync();

            var res = string.IsNullOrEmpty(scanResult.Country) ? scanResult.Result : $"{scanResult.Country} - {scanResult.Result}";

            Debug.WriteLine("Result: " + res);
            ResultView.SetResult(resultBitmap, res);

            ResultView.FadeIn();
        }
        #endregion
    }
}
