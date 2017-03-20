using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.ViewManagement;

namespace AnylineExamplesApp.Util
{
    /// <summary>
    /// Represents the utility class for miscellaneous helper methods.
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Attempts to resize the current view to a given width / height.
        /// </summary>
        /// <param name="w">The width.</param>
        /// <param name="h">The height.</param>
        public static void ResizeWindow(int w, int h)
        {
            var destSize = new Size { Width = w, Height = h };
            ApplicationView.GetForCurrentView().SetPreferredMinSize(destSize);
            ApplicationView.GetForCurrentView().TryResizeView(destSize);
        }
    }
}
