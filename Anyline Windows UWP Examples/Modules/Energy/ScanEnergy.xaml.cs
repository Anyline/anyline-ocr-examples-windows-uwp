using AnylineSDK.Camera;
using AnylineSDK.Modules.Energy;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AnylineSDK.Models;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml.Media;
using AnylineExamplesApp.Util;

namespace AnylineExamplesApp.Modules.Energy
{
    public sealed partial class ScanEnergy : Page, IEnergyResultListener, ICameraListener
    {

        private bool _isBusy;
        private object _lock = new object();
        
        #region initialization
        public ScanEnergy()
        {
            NavigationCacheMode = NavigationCacheMode.Enabled;
            ((Frame)Window.Current.Content).CacheSize = 0;
            
            InitializeComponent();

            Utils.ResizeWindow(500, 888);

            try
            {
                AnylineScanView.SetConfigFromAsset("Modules/Energy/EnergyConfig.json");
                AnylineScanView.InitAnyline(MainPage.LicenseKey, this);
            }
            catch (Exception e)
            {
                new MessageDialog(e.Message, "Exception").ShowAsync().AsTask().ConfigureAwait(false);
            }

            AnylineScanView.CameraListener = this;
            
            ResultView.OkButton.Tapped += ResultView_Tapped;            
        }

        private void ResultView_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            // enable radiobuttons
            foreach (var rb in RadioButtonGroup.Children)
                (rb as RadioButton).IsEnabled = true;

            ResultView.Visibility = Visibility.Collapsed;
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

                    RegisterRadioButtons(new Dictionary<string, EnergyScanView.ScanMode>
                    {
                        {"Electric Meter (Auto)", EnergyScanView.ScanMode.ElectricMeter},
                        {"Electric Meter 5+1 digits (A)", EnergyScanView.ScanMode.ElectricMeter51},
                        {"Electric Meter 6+1 digits (A)", EnergyScanView.ScanMode.ElectricMeter61},
                        {"Gas Meter", EnergyScanView.ScanMode.GasMeter},
                        {"Water Meter White (A)", EnergyScanView.ScanMode.WaterMeterWhite},
                        {"Water Meter Black (A)", EnergyScanView.ScanMode.WaterMeterBlack}                        
                    });
                    AnylineScanView.SetScanMode(EnergyScanView.ScanMode.ElectricMeter);
                    break;

                case "digital":

                    RegisterRadioButtons(new Dictionary<string, EnergyScanView.ScanMode>
                    {
                        {"Digital Meter", EnergyScanView.ScanMode.DigitalMeter},
                        {"Heat Meter 4 digits (up to 3 dec.)", EnergyScanView.ScanMode.HeatMeter4},
                        {"Heat Meter 5 digits (up to 3 dec.)", EnergyScanView.ScanMode.HeatMeter5},
                        {"Heat Meter 6 digits (up to 3 dec.)", EnergyScanView.ScanMode.HeatMeter6}
                    });
                    AnylineScanView.SetScanMode(EnergyScanView.ScanMode.DigitalMeter);
                    break;

                case "barcode":
                    AnylineScanView.SetScanMode(EnergyScanView.ScanMode.Barcode);
                    break;
            }           
        }

        // we make sure to free all resources when leaving the page
        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            base.OnNavigatedFrom(args);

            UnregisterRadioButtons();
            ResultView.OkButton.Tapped -= ResultView_Tapped;

            // Frees every managed and unmanaged resources
            AnylineScanView.Dispose();
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
            {
                selectedRadioButton.IsChecked = true;                
            }
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

            var selected = (from ExtendedRadioButton rb in RadioButtonGroup.Children
                               where rb.IsChecked != null && (bool)rb.IsChecked
                               select rb).SingleOrDefault();
            
            AnylineScanView.SetScanMode(selected.Value);

            lock (_lock)
                _isBusy = false;
        }
        #endregion
        
        #region camera callbacks
        public void OnCameraClosed(bool success)
        {
            AnylineScanView.CancelScanning();
        }

        public void OnCameraError(Exception e) { }

        public void OnCameraOpened(uint width, uint height)
        {
            // hide result view
            ResultView.Visibility = Visibility.Collapsed;

            // enable radiobuttons
            foreach (var rb in RadioButtonGroup.Children)
                (rb as RadioButton).IsEnabled = true;

            // As soon as the camera is opened, we start scanning
            AnylineScanView.StartScanning();
        }
        #endregion

        #region result callback
        /// <summary>
        /// When a scan result is found, this method is called with the current scan mode, result text, result image and the full frame.
        /// </summary>
        /// <param name="scanMode">The scan mode.</param>
        /// <param name="result">The result as string.</param>
        /// <param name="resultImage">The result frame.</param>
        /// <param name="fullSizedImage">The fullsized frame.</param>
        public async void OnResult(EnergyScanView.ScanMode scanMode, string result, AnylineImage resultImage, AnylineImage fullSizedImage)
        {
            // disable radiobuttons
            foreach (var rb in RadioButtonGroup.Children)
                (rb as RadioButton).IsEnabled = false;
            
            var bitmap = await resultImage.GetBitmap();

            Debug.WriteLine("Result: " + result);
            ResultView.SetResult(bitmap, result);

            ResultView.FadeIn();
        }
        #endregion
    }
}
