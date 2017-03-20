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

namespace AnylineExamplesApp.Modules.Energy
{
    public sealed partial class EnergyResultView : UserControl, IDisposable
    {

        public Button OkButton
        {
            get { return BtnOk; }
        }
        
        public EnergyResultView()
        {
            InitializeComponent();            
            Visibility = Visibility.Collapsed;
            
        }

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
