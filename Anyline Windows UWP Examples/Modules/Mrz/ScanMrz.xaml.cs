using AnylineSDK.Modules.Mrz;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using AnylineSDK.Models;
using System.Diagnostics;
using Windows.UI.Popups;
using AnylineSDK.Camera;
using AnylineExamplesApp.Util;

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

            Utils.ResizeWindow(664, 478);
            
            try
            {
                AnylineScanView.SetConfigFromAsset("Modules/Mrz/MrzConfig.json");
                AnylineScanView.InitAnyline(MainPage.LicenseKey, this);

                AnylineScanView.CameraListener = this;
            }
            catch (Exception e)
            {
                new MessageDialog(e.Message, "Exception").ShowAsync().AsTask().ConfigureAwait(false);
            }
            
            ResultView.Tapped += ResultView_Tapped;
            
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

            ResultView.Tapped -= ResultView_Tapped;

            ResultView?.Dispose();
            ResultView = null;

            // Frees every managed and unmanaged resource.
            AnylineScanView?.Dispose();
        }
        #endregion

        #region camera callbacks
        public void OnCameraClosed(bool success)
        {
            var s = success ? "sucessfully." : "with an error";
            Debug.WriteLine($"Camera closed {s}");

            // we cancel scanning when the camera is closed
            AnylineScanView?.CancelScanning();
        }

        public void OnCameraError(Exception e)
        {
            Debug.WriteLine($"A camera error occurred: {e.Message}.");
        }

        public void OnCameraOpened(uint width, uint height)
        {
            Debug.WriteLine($"Camera opened: {width}x{height}.");

            // As soon as the camera is opened, we start scanning
            AnylineScanView.StartScanning();
        }
        #endregion

        #region result callback
        public void OnResult(Identification result, AnylineImage resultImage)
        {
            Debug.WriteLine("Result: " + result);

            ResultView.UpdateResult(result);
            ResultView.FadeIn();
        }
        #endregion
    }
}
