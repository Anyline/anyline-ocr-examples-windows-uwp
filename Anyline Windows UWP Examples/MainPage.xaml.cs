using AnylineExamplesApp.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

/*
 * How to integrate Anyline in your UWP App:
 * 
 * 1) Add AnylineSDK.dll and AnylineWindowsCore.winmd as reference to the project.
 * Make sure that AnylineWindowsCore.dll is in the same directory as the winmd file.
 * 
 * 2) Make sure to set The capabilities "Webcam", "Microphone" and "Internet (Client)" in Package.appxmanifest.
 * 
 * 3) Make sure the Package Name in Package.appxmanifest matches the license
 * 
 * 4) Add a config.json for the view configuration as Asset to your project. 
 * Set the build action to "Content".
 * 
 * 5) Follow the implementation below and enjoy scanning! :)
 */

namespace AnylineExamplesApp
{    
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        // we store the generated license key for the Package Name of this App here:
        public static readonly string LicenseKey = "eyAiZGVidWdSZXBvcnRpbmciOiAib24iLCAibGljZW5zZUtleVZlcnNpb24iOiAyLCAibWFqb3JWZXJzaW9uIjogIjMiLCAicGluZ1JlcG9ydGluZyI6IHRydWUsICJwbGF0Zm9ybSI6IFsgIldpbmRvd3MiIF0sICJzY29wZSI6IFsgIkFMTCIgXSwgInNob3dXYXRlcm1hcmsiOiB0cnVlLCAidG9sZXJhbmNlRGF5cyI6IDkwLCAidmFsaWQiOiAiMjAyMC0wMS0zMSIsICJ3aW5kb3dzSWRlbnRpZmllciI6IFsgIkFueWxpbmVFeGFtcGxlc0FwcC5XaW5kb3dzIiBdIH0Ka21tb1ljQng5TTFPS29tU0JnY1R5ZmRHM1BtT3RFbDA3VkhjYU1TZndyb25aNVJBeWp5dG9CeG5OY2NMNVJwegpkdm1raDZOQi9PYzN5eWcrMnRya0VOVzNaM2tielFaV2g1d0VIUE1zT3l1R01aclNPTlVtUnpOT0VIQ2xJTUVSCjVNMVkxWFlFb0RBeEx2VitwRGJYV3JFcXpLUnlFRCtiK0w3czN0UzgxdSs2QXNGYXNod0VJZnBXZ3d0d0UvdnEKZER4YzBNVmU3dEFrWTFKT0E5alVIYmFSNUNWMUZuVC9zYmNNMVpma1JYcVRxV0lMbWRiMWJRTktyTkV0NUpZUQpTeGZSc3hxSHczL3Z5aGJlV2NETXYyc3ZaZzZQWkgyVFgzaHhnQVZpRG5TMlhtMXFpWnhETlRzZUdqR3hrVDB1Cld2bk5ZMmV4czZtc1BkL0dWclNwdGc9PQo=";
        
        public ListViewModel MainList { get; set; }
        
        public MainPage()
        {
            // We don't want to keep multiple instances of the scan views that we're navigating to.
            NavigationCacheMode = NavigationCacheMode.Enabled;
            ((Frame)Window.Current.Content).CacheSize = 0;
            
            ApplicationView.GetForCurrentView().Title = "Anyline Examples";

            Utils.ResizeWindow(300, 425);

            InitializeComponent();
            MainList = new ListViewModel();
            MainListView.SelectionChanged += OnSelectionChanged;

            // Back button functionality
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
        
        }
                
        void App_BackRequested(object sender, BackRequestedEventArgs args)
        {
            // immediately unsubscribe from the event
            SystemNavigationManager.GetForCurrentView().BackRequested -= App_BackRequested;

            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
                return;

            // Navigate back if possible, and if the event has not 
            // already been handled.
            if (args.Handled == false)
            {                
                args.Handled = true;
                if (rootFrame.CanGoBack)
                    rootFrame.GoBack();
                else
                    Application.Current.Exit();
            }
        }

        public void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            var lv = sender as ListView;
            var entry = (lv?.SelectedItem as EntryViewModel);
            
            if (entry == null || entry.Description.Equals(""))
            {
                lv.SelectedIndex = -1;
                return;
            }

            Debug.WriteLine("Selection changed to " + entry.Name);

            ApplicationView.GetForCurrentView().Title = entry.Description;

            switch (entry.UseCase)
            {
                case Model.UseCase.AnalogMeter:
                    Frame.Navigate(typeof(Modules.Energy.ScanEnergy), "analog");
                    break;
                case Model.UseCase.DigitalMeter:
                    Frame.Navigate(typeof(Modules.Energy.ScanEnergy), "digital");
                    break;
                case Model.UseCase.BarcodeScan:
                    Frame.Navigate(typeof(Modules.Energy.ScanEnergy), "barcode");
                    break;
                case Model.UseCase.MRZScan:
                    Frame.Navigate(typeof(Modules.Mrz.ScanMrz));
                    break;
                default:break;
            }            
        }
    }
}
