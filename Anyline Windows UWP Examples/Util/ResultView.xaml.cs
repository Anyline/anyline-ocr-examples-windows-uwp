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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AnylineExamplesApp.Util
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ResultView : UserControl, IDisposable
    {

        public Button OkButton
        {
            get { return BtnOk; }
        }

        public ResultView()
        {
            InitializeComponent();
            Visibility = Visibility.Collapsed;

        }

        public TextBlock ResultTextBlock { get { return TxtResult; } }

        public void SetResult(WriteableBitmap resultImage, string result)
        {
            ImgResult.Source = resultImage;
            TxtResult.Text = result;
        }

        public void FadeIn()
        {
            Visibility = Visibility.Visible;
            FadeInStoryBoard.Begin();
        }

        public void Dispose()
        {
            ImgResult.Source = null;
        }
    }
}
