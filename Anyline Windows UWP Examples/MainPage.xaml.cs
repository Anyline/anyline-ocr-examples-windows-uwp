using Anyline.SDK.Util;
using Anyline_Windows_UWP_Examples.Model;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;


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
        public static readonly string LicenseKey = "ewogICJsaWNlbnNlS2V5VmVyc2lvbiI6IDIsCiAgImRlYnVnUmVwb3J0aW5nIjogIm9uIiwKICAiaW1hZ2VSZXBvcnRDYWNoaW5nIjogZmFsc2UsCiAgIm1ham9yVmVyc2lvbiI6ICIyOSIsCiAgIm1heERheXNOb3RSZXBvcnRlZCI6IDAsCiAgImFkdmFuY2VkQmFyY29kZSI6IGZhbHNlLAogICJwaW5nUmVwb3J0aW5nIjogdHJ1ZSwKICAicGxhdGZvcm0iOiBbCiAgICAiV2luZG93cyIKICBdLAogICJzY29wZSI6IFsKICAgICJBTEwiCiAgXSwKICAic2hvd1BvcFVwQWZ0ZXJFeHBpcnkiOiBmYWxzZSwKICAic2hvd1dhdGVybWFyayI6IHRydWUsCiAgInRvbGVyYW5jZURheXMiOiA5MCwKICAidmFsaWQiOiAiMjAyMS0xMi0zMSIsCiAgIndpbmRvd3NJZGVudGlmaWVyIjogWwogICAgIkFueWxpbmVFeGFtcGxlc0FwcC5XaW5kb3dzIgogIF0KfQpBeUVnWUtyd1NhMnFQNUZOalIxemo0dHludnpLV1JkMVNNbmdMOGYyYWRMVndLMzgwcXZDMm5OZVVsbS8yYS9FQmgyV3dWYWZhRWhSRGFMTEttemFiTW5XWlpzQ0FYbytWeGlFSkV3VG5JMEE0M29WcTdZSjE4N1dOVkhSL21PNnFBOVpXakRTdjdETUI3Ty9MZk5sR3g2UWZwRVhuVStnaHRLeWxIYk9SSUFUMWJSWW03czB6NUZSRVI5RXVuS1FKdjJ2NFlPamxyc1p6ZThESkNIRW1QUVUzZ3UyYTdZcGFoOVcxMTVrRk9kTE04SVBxd3RuREVPQ29FWkRhYk5PZFFSNVByZWhJY0pmbmRLOWZTRWd3cmZtTzJMNFBRazh3UWQ4cU5ydStqOTlIamtvUGJ2MXIxakQvZW9ITzdtcEVXOVlGUHRXR21Cbnkrd25idjBhMmc9PQ==";

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
