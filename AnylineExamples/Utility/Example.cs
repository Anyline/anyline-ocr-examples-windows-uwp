using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AnylineExamples
{
    public class ExampleItem : Grid
    {
        private TextBlock textBlock;
        public Example Example { get; private set; }        
        public ExampleItem(Example example) : base()
        {
            Example = example;
            textBlock = new TextBlock();
            textBlock.Text = example.Name;
            textBlock.FontSize = 20;
            textBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
            textBlock.Padding = new Thickness(4);
            this.Children.Add(textBlock);
        }
    }

    public class CategoryItem : Grid
    {
        private TextBlock textBlock;        
        public CategoryItem(string category) : base()
        {            
            textBlock = new TextBlock();
            textBlock.Text = category;
            textBlock.FontSize = 14;
            textBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
            textBlock.FontStyle = Windows.UI.Text.FontStyle.Italic;  
            textBlock.Padding = new Thickness(4);
            this.Children.Add(textBlock);
        }
    }

    public class Example
    {
        /// <summary>
        /// Gets the category
        /// </summary>
        public string Category { get; }
        
        /// <summary>
        /// Gets the name
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Gets the relaitve file path to the JSON configuration
        /// </summary>
        public string RelativeFilePath { get; }

        /// <summary>
        ///  Gets or sets a custom JSON configuration. 
        ///  If this is set, the RelativeFilePath is ignored in the scan page
        ///  and instead, the JSON config is loaded directly into the ScanView.
        /// </summary>
        public string CustomizedJSONConfig { get; set; }

        public Example(string jsonString, string relativeFilePath)
        {
            RelativeFilePath = relativeFilePath;
            var jsonObject = JObject.Parse(jsonString);
            string id;

            // single usecase
            if (jsonObject.ContainsKey("viewPluginConfig"))
            {
                id = jsonObject["viewPluginConfig"]["pluginConfig"]["id"].ToString();
            } else if (jsonObject.ContainsKey("viewPluginCompositeConfig"))
            {
                id = jsonObject["viewPluginCompositeConfig"]["id"].ToString();
            } else
            {
                throw new Exception("Invalid JSON config");                
            }

            var tuple = id.Split('|');
            if (tuple.Length == 2)
            {
                Category = tuple[0];
                Name = tuple[1];
            }
            if (tuple.Length == 1)
            {
                Category = "Other";
                Name = tuple[0];
            }
        }
    }
}
