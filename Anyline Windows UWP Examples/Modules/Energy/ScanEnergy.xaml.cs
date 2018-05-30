using Anyline.SDK.Camera;
using Anyline.SDK.Modules.Energy;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Anyline.SDK.Models;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;
using Windows.Graphics.Imaging;
using System.Threading.Tasks;
using Windows.Foundation;

namespace AnylineExamplesApp.Modules.Energy
{
    public sealed partial class ScanEnergy : Page, IEnergyResultListener, ICameraListener, IPhotoCaptureListener
    {

        private bool _isBusy;
        private object _lock = new object();

        private TextBlock TxtBarcodeResult { get; set; }
        private Image FullFrameImage { get; set; }

        #region initialization
        public ScanEnergy()
        {
            NavigationCacheMode = NavigationCacheMode.Enabled;
            ((Frame)Window.Current.Content).CacheSize = 0;
            
            InitializeComponent();

            TxtBarcodeResult = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Foreground = new SolidColorBrush(Colors.White),
                TextWrapping = TextWrapping.Wrap
            };
            AnylineScanView.Children.Add(TxtBarcodeResult);

            // to display full frame results / photos
            FullFrameImage = new Image
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            FullFrameImage.Tapped += (s, args) =>
            {
                FullFrameImage.Visibility = Visibility.Collapsed;
            };
            AnylineScanView.Children.Add(FullFrameImage);
            
            try
            {
                // set viewconfig for analog meter here, because module is initialized with it
                AnylineScanView.SetConfigFromAsset("Modules/Energy/EnergyConfig.json");
                AnylineScanView.InitAnyline(MainPage.LicenseKey, this);            
            }
            catch (Exception e)
            {
                new MessageDialog(e.Message, "Exception").ShowAsync().AsTask().ConfigureAwait(false);
            }

            // to handle camera callbacks
            AnylineScanView.CameraListener = this;

            // to handle photo capture callbacks
            AnylineScanView.PhotoCaptureListener = this;

            ResultView.OkButton.Tapped += ResultView_Tapped;            
        }

        private void ResultView_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            // enable radiobuttons
            foreach (var rb in RadioButtonGroup.Children)
                (rb as RadioButton).IsEnabled = true;

            ResultView.Visibility = Visibility.Collapsed;
            if (AnylineScanView != null && !AnylineScanView.IsScanning)
                AnylineScanView?.StartScanning();
        }
        #endregion

        #region navigation
        // We'll dynamically populate the scan mode choices based on a parameter we provided
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            
            //remove all existing children
            RadioButtonGroup.Children.Clear();

            if (args.Parameter == null) return;

            var modeSelection = args.Parameter.ToString();
            
            switch (modeSelection)
            {
                case "analog":
                    AnylineScanView.SetConfigFromAsset("Modules/Energy/EnergyConfig.json");
                    AnylineScanView.SetScanMode(EnergyScanView.ScanMode.AnalogMeter);
                    RegisterRadioButtons(new Dictionary<string, EnergyScanView.ScanMode>
                    {
                        {"Analog Meter", EnergyScanView.ScanMode.AnalogMeter},
                        {"Analog/Digital Auto Mode", EnergyScanView.ScanMode.AutoAnalogDigitalMeter}                        
                    });
                    break;
                case "digital":
                    RegisterRadioButtons(new Dictionary<string, EnergyScanView.ScanMode>
                    {
                        {"Digital Meter", EnergyScanView.ScanMode.DigitalMeter},
                        {"Heat Meter 4 digits (up to 3 dec.)", EnergyScanView.ScanMode.HeatMeter4},
                        {"Heat Meter 5 digits (up to 3 dec.)", EnergyScanView.ScanMode.HeatMeter5},
                        {"Heat Meter 6 digits (up to 3 dec.)", EnergyScanView.ScanMode.HeatMeter6}
                    });
                    AnylineScanView.SetConfigFromAsset("Modules/Energy/EnergyConfig.json");
                    AnylineScanView.SetScanMode(EnergyScanView.ScanMode.DigitalMeter);
                    break;
                case "barcode":
                    AnylineScanView.SetConfigFromAsset("Modules/Energy/EnergyConfig.json");
                    AnylineScanView.SetScanMode(EnergyScanView.ScanMode.Barcode);
                    AnylineViewConfig.VisualFeedbackConfig.FeedbackStyle = FeedbackStyle.Rect;
                    break;
                case "photo":
                    AnylineScanView.SetConfigFromAsset("Modules/Energy/PhotoCaptureConfig.json");
                    AnylineScanView.SetScanMode(EnergyScanView.ScanMode.PhotoCapture);
                    break;
                case "serialnumber":
                    AnylineScanView.SetConfigFromAsset("Modules/Energy/EnergyConfig.json");
                    AnylineScanView.SetScanMode(EnergyScanView.ScanMode.SerialNumber);

                    // after setting the SerialNumber scan mode, you can optionally set the character whitelist and regex
                    AnylineScanView.SetSerialNumberCharWhitelist("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ");
                    AnylineScanView.SetSerialNumberValidationRegex("^[A-Z]{1}[A-Z0-9]{1,}$");
                    
                    break;
                case "dial":
                    AnylineScanView.SetConfigFromAsset("Modules/Energy/EnergyConfig.json");
                    AnylineScanView.SetScanMode(EnergyScanView.ScanMode.DialMeter);
                    break;
                case "dotmatrix":
                    AnylineScanView.SetConfigFromAsset("Modules/Energy/EnergyConfig.json");
                    AnylineScanView.SetScanMode(EnergyScanView.ScanMode.DotMatrixMeter);
                    break;
            }           
        }

        // we make sure to free all resources when leaving the page
        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            base.OnNavigatedFrom(args);

            UnregisterRadioButtons();
            ResultView.OkButton.Tapped -= ResultView_Tapped;

            if (AnylineScanView != null)
            {
                AnylineScanView.CancelScanning();
                AnylineScanView.ReleaseCameraInBackground();
            }
            AnylineScanView = null;            
        }
        #endregion

        #region radiobutton methods
        // populate
        private void RegisterRadioButtons(Dictionary<string, EnergyScanView.ScanMode> keyValuePair)
        {
            foreach (KeyValuePair<string, EnergyScanView.ScanMode> entry in keyValuePair)
            {
                var radioButton = new ExtendedRadioButton(entry.Value);
                radioButton.Foreground = new SolidColorBrush { Color = Colors.White };
                radioButton.Content = entry.Key;
                radioButton.Checked -= RadioButton_Checked;
                radioButton.Checked += RadioButton_Checked;
                RadioButtonGroup.Children.Add(radioButton);

            }

            var selectedRadioButton = RadioButtonGroup.Children.First() as ExtendedRadioButton;

            if (selectedRadioButton != null)
                selectedRadioButton.IsChecked = true;            
        }

        // remove and unregister radio buttons
        private void UnregisterRadioButtons()
        {
            foreach (var child in RadioButtonGroup.Children)
            {
                var rb = child as ExtendedRadioButton;
                rb.Checked -= RadioButton_Checked;
            }

            RadioButtonGroup.Children.Clear();
        }

        // Changes scan mode
        private void RadioButton_Checked(object sender, RoutedEventArgs args)
        {
            // prevents button spamming
            lock (_lock)
            {
                if (_isBusy) return;
                _isBusy = true;
            }

            foreach (var rb in RadioButtonGroup.Children)
                (rb as RadioButton).IsEnabled = false;

            var selected = (from ExtendedRadioButton rb in RadioButtonGroup.Children
                               where rb.IsChecked != null && (bool)rb.IsChecked
                               select rb).SingleOrDefault();
            
            AnylineScanView.SetScanMode(selected.Value);

            // we override the visual feedback with another style for heat meters for cosmetic purposes
            if (selected.Value == EnergyScanView.ScanMode.DigitalMeter)
                AnylineViewConfig.VisualFeedbackConfig.FeedbackStyle = FeedbackStyle.ContourRect;
            if (selected.Value == EnergyScanView.ScanMode.HeatMeter4
                || selected.Value == EnergyScanView.ScanMode.HeatMeter5
                || selected.Value == EnergyScanView.ScanMode.HeatMeter6)
                AnylineViewConfig.VisualFeedbackConfig.FeedbackStyle = FeedbackStyle.Rect;
            
            lock (_lock)
                _isBusy = false;

            foreach (var rb in RadioButtonGroup.Children)
                (rb as RadioButton).IsEnabled = true;
        }
        #endregion
        
        #region camera callbacks
        public void OnCameraClosed(bool success)
        {
            if (AnylineScanView != null)
                AnylineScanView.CancelScanning();
        }

        public void OnCameraError(Exception e)
        {
            Debug.WriteLine($"Camera error occurred: {e.Message}");
        }

        public void OnCameraOpened(uint width, uint height)
        {
            // hide misc. UI elements
            ResultView.Visibility = Visibility.Collapsed;
            FullFrameImage.Visibility = Visibility.Collapsed;

            // enable radiobuttons
            foreach (var rb in RadioButtonGroup.Children)
                (rb as RadioButton).IsEnabled = true;

            // As soon as the camera is opened, we start scanning
            if (AnylineScanView != null)
                AnylineScanView.StartScanning();
        }
        #endregion

        #region result callback
        /// <summary>
        /// When a scan result is found, this method is called.
        /// </summary>
        /// <param name="scanResult"></param>
        public async void OnResult(EnergyResult scanResult)
        {
            foreach (var rb in RadioButtonGroup.Children)
                (rb as RadioButton).IsEnabled = false;

            var resultBitmap = await scanResult.CutoutImage.GetBitmapAsync();

            Debug.WriteLine("Result: " + scanResult.Result);
            ResultView.SetResult(resultBitmap, scanResult.Result);

            ResultView.FadeIn();

            // Images can be saved like this
            //await SaveAnylineImageAsync(scanResult.FullImage, "FullImage");
            //await SaveAnylineImageAsync(scanResult.CutoutImage, "CutoutImage");
            
        }

// small helper method to save images
public async Task SaveAnylineImageAsync(AnylineImage image, string suggestedName)
{
    if (image == null || image.Data == null) return;

    // Create the picker object and set options
    var savePicker = new Windows.Storage.Pickers.FileSavePicker();
    savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
    // Dropdown of file types the user can save the file as
    savePicker.FileTypeChoices.Add("png", new List<string> { ".png" });
    // Default file name if the user does not type one in or select a file to replace
    savePicker.SuggestedFileName = suggestedName;

    StorageFile saveFile = await savePicker.PickSaveFileAsync();
    if (saveFile != null)
    {
        using (IRandomAccessStream fileStream = await saveFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
        {
            // save the image as PNG
            BitmapEncoder encode = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);
            byte[] buf = image.Data;
            encode.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                (uint)image.Width, (uint)image.Height, 96.0, 96.0, buf);
            await encode.FlushAsync();
            await fileStream.FlushAsync();
        }
    }
}

        /// <summary>
        /// When a photo is captured in the PhotoCapture scan mode, this method is called.
        /// </summary>
        /// <param name="anylineImage"></param>
        public async void OnPhotoCaptured(AnylineImage anylineImage)
        {
            WriteableBitmap bitmap = await anylineImage.GetBitmapAsync();
                        
            if (bitmap != null)
            {
                FullFrameImage.Source = bitmap;
                FullFrameImage.Width = bitmap.PixelWidth / 3;
                FullFrameImage.Height = bitmap.PixelHeight / 3;
                FullFrameImage.Visibility = Visibility.Visible;
            }
        }        
        #endregion
    }
}
