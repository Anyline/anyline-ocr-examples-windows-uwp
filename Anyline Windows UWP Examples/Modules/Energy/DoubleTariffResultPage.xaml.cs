using Anyline.SDK.Models;
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

namespace AnylineExamplesApp.Modules.Energy
{
    public sealed partial class DoubleTariffResultPage : Page
    {
        public DoubleTariffResultPage()
        {
            NavigationCacheMode = NavigationCacheMode.Enabled;
            ((Frame)Window.Current.Content).CacheSize = 0;

            InitializeComponent();

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            ResultListView.Items.Clear();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            var list = (List<Tuple<string, AnylineImage>>)args.Parameter;

            foreach(var item in list)
            {
                StackPanel stackPanel = new StackPanel();
                stackPanel.Children.Add(new TextBlock
                {
                    Text = $"Result #{list.IndexOf(item)}:",
                    Margin = new Thickness(10)
                });
                stackPanel.Children.Add(new Image
                {
                    Source = await item.Item2.GetBitmapAsync(),
                    Margin = new Thickness(20)
                });
                stackPanel.Children.Add(new TextBlock
                {
                    Text = item.Item1,
                    Margin = new Thickness(10)
                });
                
                ResultListView.Items.Add(stackPanel);
            }
        }
    }
}
