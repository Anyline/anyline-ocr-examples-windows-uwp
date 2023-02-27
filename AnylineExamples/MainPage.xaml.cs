using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

/*
 * How to integrate Anyline in your UWP App:
 * 
 * 1) Add AnylineSDK.dll, Anyline.winmd as reference to the project.
 * Make sure that Anyline.dll is in the same directory as the winmd file.
 * Add System.Text.Json from the NuGet package manager.
 * 
 * 2) Make sure to set The capabilities "Webcam", "Microphone" and "Internet (Client)" in Package.appxmanifest.
 * 
 * 3) Make sure the Package Name in Package.appxmanifest matches the license.
 * 
 * 4) Add a config.json for the view configuration as Asset to your project. 
 * Set the build action to "Content".
 * 
 * 5) Follow the implementation below and enjoy scanning! :)
 */
namespace AnylineExamples
{
    /// <summary>
    /// Page responsible for listing the available scanning modes
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private Example selectedExample = null;
        
        public MainPage()
        {
            // We don't want to keep multiple instances of the views that we're navigating to.
            NavigationCacheMode = NavigationCacheMode.Required;
            
            // Back button functionality
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;

            this.InitializeComponent();

            // Initialize Anyline SDK asynchronously and hide loading display upon completion
            Task.Run((Action)(() =>
            {
                try
                {
                    // Check out our documentation on how to acquire a license key for your app!
					// Please visit https://documentation-preview.anyline.com/main-component/license-key-generation.html
                    const string licenseKey = "<insert your license key here>";

                    // This has to be done once per app lifetime, otherwise Anyline will not scan.
                    AnylineSDK.Init(licenseKey);
                }
                catch(Exception e)
                {
                    // display error if SDK initialization fails
                    _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => {
                        var dialog = new MessageDialog(e.Message, "Error");                        
                        await dialog.ShowAsync();
                    });
                }
                finally
                {
                    _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        LoadingView.Visibility = Visibility.Collapsed;
                        ExampleListView.Visibility = Visibility.Visible;
                    });
                }
            }));

            FillExampleListWithExamples();

        }

        private void FillExampleListWithExamples()
        {
            try
            {
                var examples = ExampleHelper.GetProjectExamples();
                
                foreach(var category in examples)
                {
                    // Categories
                    var categoryItem = new CategoryItem(category.Key);
                    ExampleListView.Items.Add(categoryItem);

                    // Examples per category
                    foreach(var example in category.Value)
                    {
                        var exampleItem = new ExampleItem(example);                        
                        ExampleListView.Items.Add(exampleItem);
                    }
                }
                ExampleListView.SelectionChanged += List_SelectionChanged;
                ExampleListView.Visibility = Visibility.Visible;

                ScanButton.Click += ScanButton_Click;
                JSONConfigTextBox.TextChanged += JSONConfigTextBox_TextChanged;

            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        private void JSONConfigTextBox_TextChanged(object sender, TextChangedEventArgs args)
        {
            if (selectedExample != null)
            {
                selectedExample.CustomizedJSONConfig = JSONConfigTextBox.Text;
            }
        }

        private void ScanButton_Click(object sender, RoutedEventArgs args)
        {
            if (selectedExample != null)
            {
                (Window.Current.Content as Frame).Navigate(typeof(ScanPage), selectedExample, new SuppressNavigationTransitionInfo());
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            ApplicationView.GetForCurrentView().Title = "Anyline Examples";
        }

        void App_BackRequested(object sender, BackRequestedEventArgs args)
        {
            if (!(Window.Current.Content is Frame rootFrame)) return;

            // Navigate back if possible, and if the event has not already been handled.
            if (args.Handled == false)
            {
                args.Handled = true;
                if (rootFrame.CanGoBack && rootFrame.CurrentSourcePageType != typeof(MainPage))
                    rootFrame.GoBack(new SuppressNavigationTransitionInfo());
                else
                    Application.Current.Exit();
            }            
        }

        bool selectionChangeHandled = false;
        private void List_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (selectionChangeHandled)
                return;

            selectionChangeHandled = true;

            var list = sender as ListView;
            
            // navigate to the scan page
            if (list.SelectedItem is ExampleItem exampleItem)
            {
                var assetPath = exampleItem.Example.RelativeFilePath;
                var json = Anyline.SDK.Views.ScanView.GetJsonObjectFromAssetPath(assetPath);

                ScanButton.IsEnabled = true;
                ScanButton.Content = "Click to scan " + exampleItem.Example.Name;
                JSONConfigTextBox.Text = json.ToString();

                selectedExample = exampleItem.Example;
            } else
            {
                ScanButton.IsEnabled = false;
                ScanButton.Content = "(Select an example)";
                JSONConfigTextBox.Text = "";
                list.SelectedIndex = -1;
            }
            
            //list.SelectedIndex = -1;
            selectionChangeHandled = false;
        }
    }
}
