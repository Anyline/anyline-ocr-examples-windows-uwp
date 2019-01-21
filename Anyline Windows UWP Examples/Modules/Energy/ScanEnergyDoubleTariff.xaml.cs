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

            AnylineScanView.SetConfigFromAsset("Modules/Energy/energy_view_config.json");
            AnylineScanView.InitAnyline(MainPage.LicenseKey, this);

            AnylineScanView.CameraListener = this;

            doubleTariffCutoutView = new DoubleTariffCutoutView();
            doubleTariffCutoutView.StrokeWidth = AnylineViewConfig.StrokeWidth;
            doubleTariffCutoutView.StrokeBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            doubleTariffCutoutView.FillBrush = new SolidColorBrush(Color.FromArgb(60, 255, 255, 255));

            RootGrid.Children.Add(doubleTariffCutoutView);

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

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            
            AnylineScanView.CutoutView.LayoutUpdated += CutoutView_LayoutUpdated;
        }

        // we make sure to free all resources when leaving the page
        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            base.OnNavigatedFrom(args);

            Window.Current.VisibilityChanged -= Current_VisibilityChanged;
            
            if (AnylineScanView != null)
            {
                if (AnylineScanView.IsScanning)
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

            // we define how we want to shift the 2nd cutout
            double verticalDistance = 2 * bounds.Height;
            
            Rect newRect;
            
            if (!isScanningSecondMeter)
                newRect = new Rect(bounds.X, bounds.Y + verticalDistance, bounds.Width, bounds.Height);
            else
                newRect = new Rect(bounds.X, bounds.Y - verticalDistance, bounds.Width, bounds.Height);
            
            doubleTariffCutoutView.UpdateSizeForRect(newRect);
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
            CutoutView_LayoutUpdated(this, null);

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

                // start scanning again
                AnylineScanView.StartScanning();
            }
            else
            {
                Frame.Navigate(typeof(Modules.Energy.DoubleTariffResultPage), results);
            }
        }
    }
}
