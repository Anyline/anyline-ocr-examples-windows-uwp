using Anyline.SDK.Modules.Energy;
using System;
using System.Collections.Generic;
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
using Anyline.SDK.Modules;
using System.Diagnostics;
using Anyline.SDK.Camera;
using Windows.UI;
using Anyline.SDK.Models;

namespace AnylineExamplesApp.Modules.Energy
{
    public sealed partial class ScanEnergyDoubleTariff : Page, IEnergyResultListener, ICameraListener
    {
        private DoubleTariffCutoutView doubleTariffCutoutView;
        bool isScanningSecondMeter = false;
        
        List<Tuple<string, AnylineImage>> results = new List<Tuple<string, AnylineImage>>();

        public ScanEnergyDoubleTariff()
        {
            NavigationCacheMode = NavigationCacheMode.Enabled;
            ((Frame)Window.Current.Content).CacheSize = 0;

            InitializeComponent();

            AnylineScanView.SetConfigFromAsset("Modules/Energy/EnergyConfig.json");
            AnylineScanView.InitAnyline(MainPage.LicenseKey, this);
            
            AnylineScanView.CameraListener = this;

            doubleTariffCutoutView = new DoubleTariffCutoutView();
            doubleTariffCutoutView.StrokeWidth = AnylineViewConfig.StrokeWidth;
            doubleTariffCutoutView.StrokeBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            doubleTariffCutoutView.FillBrush = new SolidColorBrush(Color.FromArgb(60, 255, 255, 255));

            RootGrid.Children.Add(doubleTariffCutoutView);
        }

        
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            
            AnylineScanView.CutoutView.LayoutUpdated += CutoutView_LayoutUpdated;
        }

        // we make sure to free all resources when leaving the page
        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            base.OnNavigatedFrom(args);
            
            if (AnylineScanView != null)
            {
                AnylineScanView.CancelScanning();
                AnylineScanView.ReleaseCameraInBackground();
            }
            AnylineScanView = null;
            doubleTariffCutoutView = null;
        }
        
        private void CutoutView_LayoutUpdated(object sender, object e)
        {
            if (AnylineScanView == null) return;
            var bounds = AnylineScanView.CutoutView.GetBounds();
            double verticalDistance = 2 * bounds.Height;
            
            // 1 to draw below, -1 to draw above
            int direction = 1 - 2*Convert.ToInt32(isScanningSecondMeter);
            doubleTariffCutoutView.UpdateSizeForRect(
                new Rect(bounds.X, bounds.Y + direction * verticalDistance, bounds.Width, bounds.Height));
        }
        
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
            // As soon as the camera is opened, we start scanning
            if (AnylineScanView != null)
                AnylineScanView.StartScanning();
        }
        #endregion

        void IAnylineModuleResultListener<EnergyResult>.OnResult(EnergyResult scanResult)
        {
            results.Add(new Tuple<string, AnylineImage>(scanResult.Result, scanResult.CutoutImage.Copy()));
            if (!isScanningSecondMeter)
            {
                isScanningSecondMeter = true;
                doubleTariffCutoutView.Text = scanResult.Result;

                AnylineViewConfig.CutoutOffset = new Point(0, 2 * AnylineScanView.CutoutView.GetBounds().Height);
                AnylineScanView.InvokeScanViewUpdate();
                
                AnylineScanView.StartScanning();
            }
            else
            {
                Frame.Navigate(typeof(Modules.Energy.DoubleTariffResultPage), results);
            }
        }
    }
}
