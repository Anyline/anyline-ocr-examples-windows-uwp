using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Anyline_Windows_UWP_Examples.Model
{
    public class ExampleCategory : IGrouping<string, ExamplePlugin>
    {
        private string _category;

        public string _Category
        {
            get { return _category; }
            set { _category = value; }
        }

        private List<ExamplePlugin> _Plugins;
        public ExampleCategory(string category, List<ExamplePlugin> plugins)
        {
            _Category = category;
            _Plugins = plugins;
        }

        string IGrouping<string, ExamplePlugin>.Key => _Category;
        public IEnumerator<ExamplePlugin> GetEnumerator() => _Plugins.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _Plugins.GetEnumerator();
    }
}