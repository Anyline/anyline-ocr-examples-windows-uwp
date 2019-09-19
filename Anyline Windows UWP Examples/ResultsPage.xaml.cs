using Anyline.SDK.Models;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace Anyline_Windows_UWP_Examples
{
    /// <summary>
    /// This page is used to display the scanning results
    /// </summary>
    public sealed partial class ResultsPage : Page
    {
        public ResultsPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var results = e.Parameter as Dictionary<string, object>;
            if (results == null) return;

            var defaultMargin = new Thickness(20, 5, 20, 5);

            foreach (var result in results)
            {
                stackResults.Children.Add(new TextBlock { Text = result.Key, FontSize = 15, Foreground = new SolidColorBrush(Colors.Gray), Margin = defaultMargin });
                if (result.Value is AnylineImage anylineImage)
                {
                    WriteableBitmap bitmap = await anylineImage.GetBitmapAsync();
                    var img = new Image()
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Source = bitmap,
                        Stretch = Stretch.Uniform,
                        Width = ApplicationView.GetForCurrentView().VisibleBounds.Width / 2,
                        Margin = defaultMargin
                    };

                    stackResults.Children.Add(img);
                }
                else if(result.Value is BitmapImage bitmap)
                {
                    var img = new Image()
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Source = bitmap,
                        Stretch = Stretch.Uniform,
                        Width = ApplicationView.GetForCurrentView().VisibleBounds.Width / 2,
                        Margin = defaultMargin
                    };

                    stackResults.Children.Add(img);
                }
                else
                {
                    stackResults.Children.Add(new TextBlock { Text = result.Value.ToString().Replace("|", " - "), FontSize = 17, Foreground = new SolidColorBrush(Colors.Black), Margin = defaultMargin, FontWeight = FontWeights.Bold });
                }
                stackResults.Children.Add(new Line { X1 = 0, X2 = 1, Stretch = Stretch.Fill, Stroke = new SolidColorBrush(Colors.LightGray), Margin = new Thickness(0, 10, 0, 10), StrokeThickness = 1 });
            }
        }
    }
}
