using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace AnylineExamplesApp.Modules.Energy
{
    class DoubleTariffCutoutView : Canvas
    {
        private readonly Color _strokeColor = Color.FromArgb(255, 255, 0, 0);
        private readonly Color _fillColor = Color.FromArgb(100, 50, 50, 50);

        private Rectangle _cutoutRect;
        private TextBlock _textBlock;

        public string Text
        {
            get
            {
                return _textBlock.Text;
            }
            set
            {
                _textBlock.Text = value;
            }
        }

        public double StrokeWidth
        {
            get
            {
                return _cutoutRect.StrokeThickness;
            }
            set
            {
                _cutoutRect.StrokeThickness = value;
            }
        }

        public Brush StrokeBrush
        {
            get
            {
                return _cutoutRect.Stroke;
            }
            set
            {
                _cutoutRect.Stroke = value;
            }
        }

        public Brush FillBrush
        {
            get
            {
                return _cutoutRect.Fill;
            }
            set
            {
                _cutoutRect.Fill = value;
            }
        }

        public DoubleTariffCutoutView()
        {
            HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
            VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch;
            
            _cutoutRect = new Rectangle
            {
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch
            };

            Children.Add(_cutoutRect);

            _textBlock = new TextBlock
            {
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center,
                TextAlignment = Windows.UI.Xaml.TextAlignment.Center,
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 28
            };

            Children.Add(_textBlock);
        }

        public void UpdateSizeForRect(Rect rect)
        {
            _cutoutRect.Width = rect.Width;
            _cutoutRect.Height = rect.Height;

            var margin = new Windows.UI.Xaml.Thickness(rect.Left, rect.Top, 0, 0);
            _cutoutRect.Margin = margin;
            Canvas.SetLeft(_textBlock, rect.Left + rect.Width / 2 - _textBlock.ActualWidth / 2);
            Canvas.SetTop(_textBlock, rect.Top + rect.Height / 2 - _textBlock.ActualHeight / 2);
        }
    }
}
