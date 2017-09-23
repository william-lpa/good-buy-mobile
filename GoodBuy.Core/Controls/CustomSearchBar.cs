using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace GoodBuy.Core.Controls
{
    public class CustomSearchBar : SearchBar
    {
        public static readonly BindableProperty TextChangedCommandProperty = BindableProperty.Create("TextChangedCommand", typeof(Command<string>), typeof(CustomListView), null);

        public Command<string> TextChangedCommand
        {
            get => (Command<string>)GetValue(TextChangedCommandProperty);
            set => SetValue(TextChangedCommandProperty, value);
        }

        public CustomSearchBar()
        {
            Initialize();
        }

        private void Initialize()
        {
            this.TextChanged += (sender, e) =>
             {
                 if (TextChangedCommand == null)
                     return;
                 if (TextChangedCommand.CanExecute(e.NewTextValue))
                     TextChangedCommand.Execute(e.NewTextValue);
             };
        }
    }
}
