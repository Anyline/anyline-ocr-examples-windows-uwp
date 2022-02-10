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
        public static readonly string LicenseKey = "ewogICJsaWNlbnNlS2V5VmVyc2lvbiI6ICIzLjAiLAogICJkZWJ1Z1JlcG9ydGluZyI6ICJwaW5nIiwKICAibWFqb3JWZXJzaW9uIjogIjM3IiwKICAic2NvcGUiOiBbCiAgICAiQUxMIgogIF0sCiAgIm1heERheXNOb3RSZXBvcnRlZCI6IDAsCiAgImFkdmFuY2VkQmFyY29kZSI6IGZhbHNlLAogICJwbGF0Zm9ybSI6IFsKICAgICJXaW5kb3dzIgogIF0sCiAgInNob3dXYXRlcm1hcmsiOiB0cnVlLAogICJ0b2xlcmFuY2VEYXlzIjogMzAsCiAgInZhbGlkIjogIjIwMjItMTItMzEiLAogICJ3aW5kb3dzSWRlbnRpZmllciI6IFsKICAgICJBbnlsaW5lRXhhbXBsZXNBcHAuV2luZG93cyIKICBdCn0KVGRXUlloVWY1aXhSVFNZdHZVKzlaYnMzS1l2M3JsUTlFYkwrbG0ra1MwaENEQUhSYkZUUzA4Skc3cFpXYUpuZjZBR1ZsLzF1eG5wUWcxYnk0cUp2cHVtL3p0TitrNXV4SCt5Z0FES1FpUG1FZXExbWYxdWlqY203cjE1SUp3VEtveG5aUmpCQmk1dFVIcmxqN0M1OGRyN3V6RVFrZnNVSHJWdTd1aTN5d0NPeWlZVktPTm5rZ2oyK2lyakU4OVdqMTlUQmFRVUVKZEc1L1FkQTlPWDdpRWlmdEFMU2xnNkF1UGdPL3hoSHJoK2JHRnMvMHlnSElwVzlhNEtZaXdKNTZGWUp5MnZKVUdvenRzN20veUJzTFdVU0s2M1BmdnVSQVBTS0FVV0tkcG1hNVpjdjFhU1ptelJ1L3NoVkRQbFhVaEZDTlZhT0JKWmdHTGJURnhGbjRnPT0=";

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
