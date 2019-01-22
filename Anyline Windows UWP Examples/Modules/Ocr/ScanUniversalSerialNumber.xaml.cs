using Anyline.SDK.Camera;
using Anyline.SDK.Modules.Ocr;
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

namespace AnylineExamplesApp.Modules.Ocr
{
    public sealed partial class ScanUniversalSerialNumber : Page, ICameraListener, IAnylineOcrResultListener
    {
        #region initialization
        public ScanUniversalSerialNumber()
        {
            NavigationCacheMode = NavigationCacheMode.Enabled;
            ((Frame)Window.Current.Content).CacheSize = 0;

            InitializeComponent();

            try
            {
                AnylineScanView.SetConfigFromAsset("Modules/Ocr/serial_number_view_config.json");
                AnylineScanView.InitAnyline(MainPage.LicenseKey, this);

                AnylineScanView.CameraListener = this;

                SetOcrConfig();

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

        private void SetOcrConfig()
        {
            AnylineOcrConfig anylineOcrConfig = new AnylineOcrConfig();

            anylineOcrConfig.SetLanguages(@"Modules\Ocr\USNr.any");
            // AUTO ScanMode automatically detects the correct text without any further parameters to be set
            anylineOcrConfig.ScanMode = AnylineOcrConfig.OcrScanMode.Auto;
            anylineOcrConfig.ValidationRegex = "[A-Z0-9]{4,}";
            
            // set the ocr config
            AnylineScanView.SetAnylineOcrConfig(anylineOcrConfig);
        }
        #endregion

        #region navigation
        // we make sure to free all resources when leaving the page        
        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            base.OnNavigatedFrom(args);

            Window.Current.VisibilityChanged -= Current_VisibilityChanged;

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
            ResultView.Visibility = Visibility.Collapsed;

            // As soon as the camera is opened, we start scanning
            if (AnylineScanView != null)
                AnylineScanView.StartScanning();
        }
        #endregion

        #region result callback        
        public async void OnResult(AnylineOcrResult scanResult)
        {
            var resultBitmap = await scanResult.CutoutImage.GetBitmapAsync();

            Debug.WriteLine("Result: " + scanResult.Result);
            ResultView.SetResult(resultBitmap, scanResult.Result);
            ResultView.FadeIn();
        }
        #endregion
    }
}
