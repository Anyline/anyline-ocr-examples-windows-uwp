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
        public static readonly string LicenseKey = "ewogICJsaWNlbnNlS2V5VmVyc2lvbiI6ICIzLjAiLAogICJkZWJ1Z1JlcG9ydGluZyI6ICJvbiIsCiAgIm1ham9yVmVyc2lvbiI6ICIzNyIsCiAgInNjb3BlIjogWwogICAgIkFMTCIKICBdLAogICJtYXhEYXlzTm90UmVwb3J0ZWQiOiAwLAogICJhZHZhbmNlZEJhcmNvZGUiOiBmYWxzZSwKICAicGxhdGZvcm0iOiBbCiAgICAiV2luZG93cyIKICBdLAogICJzaG93V2F0ZXJtYXJrIjogdHJ1ZSwKICAidG9sZXJhbmNlRGF5cyI6IDMwLAogICJ2YWxpZCI6ICIyMDIyLTEyLTMxIiwKICAid2luZG93c0lkZW50aWZpZXIiOiBbCiAgICAiQW55bGluZUV4YW1wbGVzQXBwLldpbmRvd3MiCiAgXQp9ClQvQ3d6S3g3bnNacUYxNk8ySVI2alJQYjQxZGtMZ043UzhseG0vYkYzM21jMUtneFFWR0lsU0xVVzhEQmVhV3BHZnpPTGRoZWlNMGZhRjhLYVlPK3dsU3Q5clhVMC82S3RwMFI1cldQemJjTkZDZWFqYTJnSDYzaFA3eEtHQ2kvQWQrSnZRVGYybm00d0MybWg4bEpTMFJ4elpITXBrdEY5TUQzcUJKb2lxZjNMVzRZcE83VGxiRFFZaXlIMjliVVN5TTlUU2FuL1pTbHJWUTQySExqZ2xQdHdKb0h2bHQwSFJjSXBlcGpqb2Q2cGo0d3lqZ2JCQ3FqeXQ0Q1cyR05Xb3pkbjNhOXlQMEhyQWhxbVBpc215LzFGckNaVC9MSnlYL3hBWFdOMWNMWUFQcDY2VjVXT3BoTGpLRk5UVG5HcHJIeHBvQWNGZDZCUVozaTR6cy81UT09";

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
