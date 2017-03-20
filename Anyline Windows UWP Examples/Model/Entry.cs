namespace AnylineExamplesApp.Model
{

    public enum UseCase
    {
        AnalogMeter = 0,
        DigitalMeter,
        BarcodeScan,
        MRZScan
    }

    /// <summary>
    /// Model for an Entry in the Example List.
    /// </summary>
    public class Entry
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string BackgroundSource { get; set; } = "ms-appx:///";
        public string IconSource { get; set; } = "ms-appx:///";
        public UseCase UseCase { get; set; } = UseCase.AnalogMeter;
    }
}