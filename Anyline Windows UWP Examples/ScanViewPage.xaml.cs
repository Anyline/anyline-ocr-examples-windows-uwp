using Anyline.SDK.Models;
using Anyline.SDK.Plugins;
using Anyline.SDK.Plugins.Barcode;
using Anyline.SDK.Plugins.ID;
using Anyline.SDK.Plugins.LicensePlate;
using Anyline.SDK.Plugins.Meter;
using Anyline.SDK.Plugins.Ocr;
using Anyline.SDK.ViewPlugins;
using Anyline.SDK.Views;
using Anyline_Windows_UWP_Examples.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace Anyline_Windows_UWP_Examples
{
    /// <summary>
    /// This page is used to setup the plugin, open the camera and start scanning
    /// it has to implement the IScanResultListener for each Plugin that will be used
    /// </summary>
    public sealed partial class ScanViewPage : Page,
        IScanResultListener<BarcodeScanResult>,
        IScanResultListener<ScanResult<ID>>,
        IScanResultListener<LicensePlateScanResult>,
        IScanResultListener<MeterScanResult>,
        IScanResultListener<OcrScanResult>,
        IPhotoCaptureListener
    {
        private ScanView _anylineScanView;
        private AbstractBaseScanViewPlugin _scanViewPlugin;

        public ScanViewPage()
        {
            // We don't want to keep multiple instances of the scan views that we're navigating to.
            NavigationCacheMode = NavigationCacheMode.Disabled;
            ((Frame)Window.Current.Content).CacheSize = 0;

            ApplicationView.GetForCurrentView().Title = "Anyline Examples";


            this.InitializeComponent();

            _anylineScanView = AnylineScanView;

            if (_anylineScanView != null)
            {
                _anylineScanView.CameraView.CameraOpened += CameraView_CameraOpened;
                _anylineScanView.CameraView.CameraClosed += CameraView_CameraClosed;
                _anylineScanView.CameraView.CameraError += CameraView_CameraError;

                Window.Current.VisibilityChanged -= Current_VisibilityChanged;
                Window.Current.VisibilityChanged += Current_VisibilityChanged;
            }
        }

        private void CameraView_CameraOpened(object sender, Size args)
        {
            Debug.WriteLine($"(APP) Camera Opened: {args.Width}x{args.Height}");

            try
            {
                _anylineScanView.StartScanning();
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
                _anylineScanView.StopScanning();
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

        // important because the UWP camera stream automatically shuts down when a window is minimized
        private async void Current_VisibilityChanged(object sender, Windows.UI.Core.VisibilityChangedEventArgs args)
        {
            if (args.Visible == false)
            {
                await _anylineScanView.StopCameraAsync();
            }
            if (args.Visible == true)
            {
                _anylineScanView.StartCamera();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _anylineScanView?.StopCameraAsync();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ApplicationView.GetForCurrentView().Title = (e.Parameter as ExamplePlugin).Name;

            string jsonConfig = (e.Parameter as ExamplePlugin).JSONConfigFile;
            _anylineScanView?.Init("Assets/jsonConfigs/" + jsonConfig.Replace(".json", "") + ".json", MainPage.LicenseKey);
            // the correct plugin is loaded according to the .json config file informed
            _scanViewPlugin = _anylineScanView?.ScanViewPlugin;

            /// here you need to add this class as a Result Listener for the plugin that you will use
            (_scanViewPlugin as BarcodeScanViewPlugin)?.AddScanResultListener(this);
            (_scanViewPlugin as IDScanViewPlugin)?.AddScanResultListener(this);
            (_scanViewPlugin as LicensePlateScanViewPlugin)?.AddScanResultListener(this);
            (_scanViewPlugin as MeterScanViewPlugin)?.AddScanResultListener(this);
            (_scanViewPlugin as OcrScanViewPlugin)?.AddScanResultListener(this);

            if (_scanViewPlugin is MeterScanViewPlugin)
            {
                (_scanViewPlugin as MeterScanViewPlugin).PhotoCaptureListener = this;
                (_scanViewPlugin as MeterScanViewPlugin).PhotoCaptureTarget = PhotoCaptureTarget.File;
            }

            _anylineScanView?.StartCamera();
        }

        public void OnResult(LicensePlateScanResult result)
        {
            OpenResultsPage(result.CreatePropertyDictionary());
        }

        public void OnResult(BarcodeScanResult result)
        {
            OpenResultsPage(result.CreatePropertyDictionary());
        }

        public void OnResult(MeterScanResult result)
        {
            OpenResultsPage(result.CreatePropertyDictionary());
        }

        public void OnResult(OcrScanResult result)
        {
            OpenResultsPage(result.CreatePropertyDictionary());
        }

        public void OnResult(ScanResult<ID> result)
        {
            OpenResultsPage(result.CreatePropertyDictionary());
        }

        private void OpenResultsPage(Dictionary<string, object> dictionary)
        {
            /// in our example app, we dynamically extract result values from the object via reflection.
            /// Usually, you can just use the result on the object type that matches your ScanPlugin type (e.g. ALMeterResult for ALMeterScanPlugin etc.) and access the properties directly
            (Window.Current.Content as Frame).Navigate(typeof(ResultsPage), dictionary);
        }

        public void OnPhotoCaptured(AnylineImage anylineImage)
        {
            var result = new Dictionary<string, object> { };
            result.Add("Image result", anylineImage);
            (Window.Current.Content as Frame).Navigate(typeof(ResultsPage), result);
        }

        public async void OnPhotoToFile(StorageFile file)
        {
            BitmapImage bitmapImage = new BitmapImage();
            FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);
            bitmapImage.SetSource(stream);

            var result = new Dictionary<string, object> { };
            result.Add("Image result", bitmapImage);
            (Window.Current.Content as Frame).Navigate(typeof(ResultsPage), result);
        }
    }
}
