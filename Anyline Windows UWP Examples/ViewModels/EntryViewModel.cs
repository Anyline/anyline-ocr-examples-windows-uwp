using System;
using AnylineExamplesApp.Model;
using Windows.UI.Xaml.Controls;

namespace ViewModels
{
    public class EntryViewModel : NotificationBase<Entry>
    {
        public EntryViewModel(Entry entry = null) : base(entry) { }

        public string Name
        {
            get { return This.Name; }
            set { SetProperty(This.Name, value, () => This.Name = value); }
        }

        public string Description
        {
            get { return This.Description; }
            set { SetProperty(This.Description, value, () => This.Description = value); }
        }

        public string BackgroundSource
        {
            get { return This.BackgroundSource; }
            set { SetProperty(This.BackgroundSource, value, () => This.BackgroundSource = value); }
        }

        public string IconSource
        {
            get { return This.IconSource; }
            set { SetProperty(This.IconSource, value, () => This.IconSource = value); }
        }
        
        public UseCase UseCase
        {
            get { return This.UseCase; }
            set { SetProperty(This.UseCase, value, () => This.UseCase = value); }
        }        
    }
}
