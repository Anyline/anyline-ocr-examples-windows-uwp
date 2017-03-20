using System.Collections.Generic;

namespace AnylineExamplesApp.Model
{    
    /// <summary>
    /// A list of use-case examples.
    /// </summary>
    public class ExampleList
    {
        public List<Entry> Entries { get; set; }

        /// <summary>
        /// This constructor pre-fills the list of examples with hardcoded values to match our use-cases.
        /// </summary>
        public ExampleList()
        {
            Entries = new List<Entry>
            {
                // MRZ
                new Entry { Name="MRZ" },

                new Entry {
                    Name ="MRZ",
                    Description ="Scan MRZ of passport or ID",
                    BackgroundSource = "ms-appx:///Assets/Images/passport.png",
                    IconSource = "ms-appx:///Assets/Images/icon_mrz.png",
                    UseCase = UseCase.MRZScan
                },

                // Energy
                new Entry { Name="Energy (BETA)" },

                new Entry {
                    Name ="Analog Meters",
                    Description ="Scan analog meters",
                    BackgroundSource = "ms-appx:///Assets/Images/electric.png",
                    IconSource = "ms-appx:///Assets/Images/icon_analog.png",
                    UseCase = UseCase.AnalogMeter
                },
                new Entry {
                    Name ="Digital Meters",
                    Description ="Scan digital meters",
                    BackgroundSource = "ms-appx:///Assets/Images/digital.png",
                    IconSource = "ms-appx:///Assets/Images/icon_digital.png",
                    UseCase = UseCase.DigitalMeter
                },

                // Barcode
                new Entry { Name="Barcode (BETA)" },

                new Entry {
                    Name ="Barcodes",
                    Description ="Scan barcodes & QR codes",
                    BackgroundSource = "ms-appx:///Assets/Images/barcode.png",
                    IconSource = "ms-appx:///Assets/Images/icon_barcode.png",
                    UseCase = UseCase.BarcodeScan
                }                
            };
        }        
    }
}