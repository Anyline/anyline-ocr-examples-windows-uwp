using Anyline.SDK.Util;
using Anyline_Windows_UWP_Examples.Model;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


/*
 * How to integrate Anyline in your UWP App:
 * 
 * 1) Add AnylineSDK.dll, Anyline.winmd as reference to the project.
 * Make sure that Anyline.dll is in the same directory as the winmd file.
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

namespace Anyline_Windows_UWP_Examples
{
    /// <summary>
    /// Page responsible for listing the available scanning modes
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // we store the generated license key for the Package Name of this App here:
        public static readonly string LicenseKey = "ew0KICAibGljZW5zZUtleVZlcnNpb24iOiAyLA0KICAiZGVidWdSZXBvcnRpbmciOiAib24iLA0KICAiaW1hZ2VSZXBvcnRDYWNoaW5nIjogZmFsc2UsDQogICJtYWpvclZlcnNpb24iOiAiMjkiLA0KICAibWF4RGF5c05vdFJlcG9ydGVkIjogMCwNCiAgImFkdmFuY2VkQmFyY29kZSI6IGZhbHNlLA0KICAicGluZ1JlcG9ydGluZyI6IHRydWUsDQogICJwbGF0Zm9ybSI6IFsNCiAgICAiV2luZG93cyINCiAgXSwNCiAgInNjb3BlIjogWw0KICAgICJBTEwiDQogIF0sDQogICJzaG93UG9wVXBBZnRlckV4cGlyeSI6IGZhbHNlLA0KICAic2hvd1dhdGVybWFyayI6IHRydWUsDQogICJ0b2xlcmFuY2VEYXlzIjogOTAsDQogICJ2YWxpZCI6ICIyMDIxLTAxLTMxIiwNCiAgIndpbmRvd3NJZGVudGlmaWVyIjogWw0KICAgICJBbnlsaW5lRXhhbXBsZXNBcHAuV2luZG93cyINCiAgXQ0KfQpvMmliRWZEK1FSQmp0bCtKMzk5YUtFNHVscGI3b1R1dms4L01vNG12MXFIazBuSWNtV1ppWDFHa2Y1NTQrcVNkelhWa0l5TC9nbW1uRDBaV3hyai9sUjRrVTU5aFZ5SGMybmV0bVFZeDdmNVJKRjN0bjdtLy9qK09GQnRiZUozZHg5bkl1c25DdU9UZCtCZEZhTUNMeWVNWUZhSjNtWlh4Z0VKY28zcm43RW1OWHhKL0MyTTE3Qk9CajFaSytSdEIvZFFzVFlweTVnQkRsd0NBVzRSVnI4dTl6MkUxV1MzU1NYZEhFK0I1Rmp0dmRyVDhwdG54TENRMCtZS3l2eUJZVjU3MXliOXFWQXM1SFcrKysxYnh6dVdOeWY1ZUp6TjZpMklObG8yOHNVdWFEUWhoMlBCZldrcVduQ25IZnoxaThhUml5aDBCbk5ZTFZFZHM1bVlJTWc9PQ==";

        public MainPage()
        {
            // We don't want to keep multiple instances of the scan views that we're navigating to.
            NavigationCacheMode = NavigationCacheMode.Required;
            ((Frame)Window.Current.Content).CacheSize = 0;

            ApplicationView.GetForCurrentView().Title = "Anyline Examples";

            this.InitializeComponent();

            // This must be called before doing anything Anyline-related!
            // Try/Catch this to check wheather or not your license key is valid!
            AnylineSDK.Init(LicenseKey);

            // Back button functionality
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;

            lvPlugins.Tapped += LvPlugins_Tapped;
        }

        void App_BackRequested(object sender, BackRequestedEventArgs args)
        {
            if (!(Window.Current.Content is Frame rootFrame)) return;

            // Navigate back if possible, and if the event has not already been handled.
            if (args.Handled == false)
            {
                args.Handled = true;
                if (rootFrame.CanGoBack && rootFrame.CurrentSourcePageType != typeof(MainPage))
                    rootFrame.GoBack();
                else
                    Application.Current.Exit();
            }
        }

        private void LvPlugins_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if ((sender as ListView).SelectedItem != null)
            {
                var plugin = (sender as ListView).SelectedItem as ExamplePlugin;
                (sender as ListView).SelectedItem = null;
                (Window.Current.Content as Frame).Navigate(typeof(ScanViewPage), plugin);
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await Task.Run(() =>
            {
                ResourceManager.Load();
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => LoadingView.Visibility = Visibility.Collapsed);
            });
        }
    }
}
