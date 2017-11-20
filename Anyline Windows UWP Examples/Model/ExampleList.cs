using System;
using System.Collections.Generic;
using System.Reflection;

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

                new Entry
                {
                    Name ="MRZ",
                    Description ="Scan MRZ of passport or ID",
                    BackgroundSource = "ms-appx:///Assets/Images/passport.png",
                    IconSource = "ms-appx:///Assets/Images/icon_mrz.png",
                    UseCase = UseCase.MRZScan
                },

                // Energy
                new Entry { Name="Energy" },

                new Entry
                {
                    Name ="Analog Meters",
                    Description ="Scan analog meters",
                    BackgroundSource = "ms-appx:///Assets/Images/electric.png",
                    IconSource = "ms-appx:///Assets/Images/icon_analog.png",
                    UseCase = UseCase.AnalogMeter
                },
                new Entry
                {
                    Name ="Digital Meters",
                    Description ="Scan digital meters",
                    BackgroundSource = "ms-appx:///Assets/Images/digital.png",
                    IconSource = "ms-appx:///Assets/Images/icon_digital.png",
                    UseCase = UseCase.DigitalMeter
                },
                new Entry
                {
                    Name ="Photo Snapper",
                    Description ="Take photos for meters",
                    BackgroundSource = "ms-appx:///Assets/Images/electric.png",
                    IconSource = "ms-appx:///Assets/Images/icon_mrz.png",
                    UseCase = UseCase.PhotoMode
                },

                new Entry
                {
                    Name ="Serial Numbers",
                    Description ="Scan serial numbers",
                    BackgroundSource = "ms-appx:///Assets/Images/digital.png",
                    IconSource = "ms-appx:///Assets/Images/icon_mrz.png",
                    UseCase = UseCase.SerialNumbers
                },
                new Entry
                {
                    Name ="Serial Numbers (with 3 Letter Prefix)",
                    Description ="Scan serial numbers with 3-letter alphanumeric prefix",
                    BackgroundSource = "ms-appx:///Assets/Images/digital.png",
                    IconSource = "ms-appx:///Assets/Images/icon_mrz.png",
                    UseCase = UseCase.SerialNumberAlphaNumeric3Letters
                },

                // Barcode
                new Entry { Name="Barcode" },

                new Entry
                {
                    Name ="Barcodes",
                    Description ="Scan barcodes & QR codes",
                    BackgroundSource = "ms-appx:///Assets/Images/barcode.png",
                    IconSource = "ms-appx:///Assets/Images/icon_barcode.png",
                    UseCase = UseCase.BarcodeScan
                },

                new Entry {
                    Name = GetAssemblyVersion()
                }
            };
        }

        private string GetAssemblyVersion()
        {
            var assembly = typeof(Anyline.SDK.Core.AnylineController).GetTypeInfo().Assembly;
            if (assembly != null)
            {
                Version ver = assembly.GetName().Version;
                return $"SDK version: {ver}";
            }
            return "";
        }
    }
}