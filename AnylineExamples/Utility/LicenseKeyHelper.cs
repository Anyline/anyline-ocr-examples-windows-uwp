using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace AnylineExamples.Utility
{
    public class LicenseKeyHelper
    {
        public static string ReadLicenseKeyFromFile(CoreDispatcher dispatcher, string path)
        {
            string licenseKey = "";

            FileInfo licenseKeyFile = new FileInfo(path);
            if (licenseKeyFile.Exists)
            {
                licenseKey = File.ReadAllText(licenseKeyFile.FullName);
            } else
            {
                // display error if no license was provided
                _ = dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    string message = "Please follow our documentation to get your license key.";
                    var dialog = new ContentDialog();
                    dialog.Title = "No Anyline license key found";
                    dialog.Content = message;
                    dialog.PrimaryButtonText = "OK";
                    dialog.PrimaryButtonClick += (sender, args) =>
                    {
                        dialog.Hide();
                    };
                    dialog.SecondaryButtonText = "Visit documentation";
                    dialog.SecondaryButtonClick += async (sender, args) =>
                    {
                        await Windows.System.Launcher.LaunchUriAsync(new Uri("https://documentation-preview.anyline.com/main-component/license-key-generation.html"));
                        dialog.Hide();
                    };
                    await dialog.ShowAsync();
                });
            }

            return licenseKey;
        }
    }
}
