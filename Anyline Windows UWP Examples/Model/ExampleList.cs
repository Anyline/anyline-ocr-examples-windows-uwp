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

                new Entry {
                    Name = GetAssemblyVersion()
                }
            };
        }

        private string GetAssemblyVersion()
        {
            var assembly = typeof(Anyline.SDK.Views.ScanView).GetTypeInfo().Assembly;
            if (assembly != null)
            {
                Version ver = assembly.GetName().Version;
                return $"SDK version: {ver}";
            }
            return "";
        }
    }
}