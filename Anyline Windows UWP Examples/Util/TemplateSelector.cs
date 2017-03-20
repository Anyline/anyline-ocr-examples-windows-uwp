using ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AnylineExamplesApp
{

    /// <summary>
    /// Distinguishes between Header and Entry items from the ListView.
    /// </summary>
    class TemplateSelector : DataTemplateSelector
    {

        public DataTemplate HeaderTemplate { get; set; }
        public DataTemplate EntryTemplate { get; set; }
        
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item == null) return null;
            var element = item as EntryViewModel;

            if (element != null)
            {                
                if (element.Description.Equals(""))
                    return HeaderTemplate;
                else
                    return EntryTemplate;
                       
            }

            return null;
        }
    }
}
