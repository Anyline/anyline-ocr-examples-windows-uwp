using Anyline.SDK.Modules.Energy;
using Windows.UI.Xaml.Controls;

namespace AnylineExamplesApp.Modules.Energy
{
    class ExtendedRadioButton : RadioButton
    {
        public ExtendedRadioButton(EnergyScanView.ScanMode value) : base()
        {
            Value = value;
        }

        public EnergyScanView.ScanMode Value { get; private set; }
    }    
}
