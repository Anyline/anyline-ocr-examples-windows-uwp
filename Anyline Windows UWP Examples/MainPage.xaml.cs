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
        public static readonly string LicenseKey = "ew0KICAibGljZW5zZUtleVZlcnNpb24iOiAiMy4wIiwNCiAgImRlYnVnUmVwb3J0aW5nIjogInBpbmciLA0KICAibWFqb3JWZXJzaW9uIjogIjM3IiwNCiAgInNjb3BlIjogWw0KICAgICJBTEwiDQogIF0sDQogICJtYXhEYXlzTm90UmVwb3J0ZWQiOiAwLA0KICAiYWR2YW5jZWRCYXJjb2RlIjogZmFsc2UsDQogICJwbGF0Zm9ybSI6IFsNCiAgICAiV2luZG93cyINCiAgXSwNCiAgInNob3dXYXRlcm1hcmsiOiB0cnVlLA0KICAidG9sZXJhbmNlRGF5cyI6IDMwLA0KICAidmFsaWQiOiAiMjAyMy0xMi0zMSIsDQogICJ3aW5kb3dzSWRlbnRpZmllciI6IFsNCiAgICAiQW55bGluZUV4YW1wbGVzQXBwLldpbmRvd3MiDQogIF0NCn0KaUgvY2s1LzMrR0JhdXFWQjdSbG9HdnZhaVJJS3JNK0ZaRjd4ZjVmbi9OL00yT2wra1R6eC9INWQrTWRjRWxtU1dPUExEdVlHSjBGenF5cGJuOGkxWmRTMDFvM2dMZDVycjdyUEd2L295dXRkTmZQTjJ0VXp4VFZNbmF0STJkU0pvc0c0d1gwWnVZTkFBRUJGR2tXQllmWjMvVzJvbTdwcXhGNm5xeTY0eStnVFFjcXgvbUt4Q2ZnQkk3S0kvek05am52UVZrMGpZYVFtOVh4TndGakZqODFzVlV5U3lQd0pyTXEyZFpuc2h5amg1TDQ5ZzM3MXU2cVFFeXlUamNKTHRXTlk3WXBFdnVEMTljVFN6bk1hd3hLTDI5MHlXdEpadDRvMFc2NW10SHFua1ZlVHhTZEFNUFp5NS9XK3R5WnBwK0hDWXZmTXZBMDhDL2RIRGh5bndBPT0=";

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
                AnylineLoader.Load();
                _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => LoadingView.Visibility = Visibility.Collapsed);
            });
        }
    }
}
