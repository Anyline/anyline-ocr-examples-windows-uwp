using Anyline.SDK.Modules.Mrz;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Anyline.SDK.Models;
using System.Diagnostics;
using Windows.UI.Popups;
using Anyline.SDK.Camera;
using AnylineExamplesApp.Util;
using Windows.Data.Json;

namespace AnylineExamplesApp.Modules.Mrz
{
    public sealed partial class ScanMrz : Page, IMrzResultListener, ICameraListener
    {
        #region initialization
        public ScanMrz()
        {
            NavigationCacheMode = NavigationCacheMode.Enabled;
            ((Frame)Window.Current.Content).CacheSize = 0;

            InitializeComponent();
                        
            try
            {
                AnylineScanView.SetConfigFromAsset("Modules/Mrz/mrz_view_config.json");
                AnylineScanView.InitAnyline(MainPage.LicenseKey, this);

                AnylineScanView.CameraListener = this;
            }
            catch (Exception e)
            {
                new MessageDialog(e.Message, "Exception").ShowAsync().AsTask().ConfigureAwait(false);
            }

            ResultView.Tapped += ResultView_Tapped;
            Window.Current.VisibilityChanged += Current_VisibilityChanged;

            if (!AnylineScanView.IsCameraOpen())
                AnylineScanView.OpenCameraInBackground();
        }

        // we do this because the UWP camera stream automatically shuts down when a window is minimized
        private void Current_VisibilityChanged(object sender, Windows.UI.Core.VisibilityChangedEventArgs args)
        {
            if (args.Visible == false)
            {
                if (AnylineScanView.IsCameraOpen())
                    AnylineScanView.ReleaseCameraInBackground();
            }
            if (args.Visible == true)
            {
                if (!AnylineScanView.IsCameraOpen())
                    AnylineScanView.OpenCameraInBackground();
            }
        }

        private void ResultView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ResultView.FadeOut();
            AnylineScanView.StartScanning();
        }
        #endregion

        #region navigation
        // we make sure to free all resources when leaving the page        
        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            base.OnNavigatedFrom(args);

            Window.Current.VisibilityChanged -= Current_VisibilityChanged;
            ResultView.Tapped -= ResultView_Tapped;

            ResultView?.Dispose();
            ResultView = null;

            if (AnylineScanView != null)
            {
                AnylineScanView.CancelScanning();
                AnylineScanView.ReleaseCameraInBackground();
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
        }

        public void OnCameraOpened(uint width, uint height)
        {
            Debug.WriteLine($"Camera opened: {width}x{height}.");

            if (ResultView != null)
                ResultView.Visibility = Visibility.Collapsed;
            
            // As soon as the camera is opened, we start scanning
            if (AnylineScanView != null)
                AnylineScanView.StartScanning();
        }
        #endregion

        #region result callback        
        public void OnResult(MrzResult scanResult)
        {
            Debug.WriteLine("Result: " + scanResult.Result);
            
            ResultView.UpdateResult(scanResult.Result);
            ResultView.FadeIn();
        }
        #endregion
    }
}
