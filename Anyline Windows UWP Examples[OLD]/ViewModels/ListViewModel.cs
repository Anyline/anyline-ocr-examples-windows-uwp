using AnylineExamplesApp.Model;
using System.Collections.ObjectModel;

namespace ViewModels
{
    /// <summary>
    /// The ViewModel for the item list that presents the use-cases.
    /// </summary>
    public class ListViewModel : NotificationBase
    {
        ExampleList _examplesList;
        int _selectedIndex;
        
        public EntryViewModel SelectedEntry
        {
            get { return (_selectedIndex >= 0) ? _entries[_selectedIndex] : null; }
        }
        
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { if (SetProperty(ref _selectedIndex, value)) { RaisePropertyChanged(nameof(SelectedEntry)); } }
        }
        
        ObservableCollection<EntryViewModel> _entries = new ObservableCollection<EntryViewModel>();
        public ObservableCollection<EntryViewModel> Entries
        {
            get { return _entries; }
            set { SetProperty(ref _entries, value); }
        }

        public ListViewModel()
        {
            _examplesList = new ExampleList();
            _selectedIndex = -1;

            // load data
            foreach (var entry in _examplesList.Entries)
            {
                var e = new EntryViewModel(entry);
                _entries.Add(e);
            }            
        }
    }
}
