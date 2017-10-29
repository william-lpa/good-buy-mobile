using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace GoodBuy.Core.Controls
{
    public class AutoComplete : Entry
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create("ItemsSource", typeof(List<string>), typeof(AutoComplete), null, BindingMode.OneWay);
        public static readonly BindableProperty NameProperty = BindableProperty.Create("Name", typeof(string), typeof(AutoComplete), null, BindingMode.OneWay);

        public List<string> ItemsSource
        {
            get => (List<string>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public string Name
        {
            get => GetValue(NameProperty)?.ToString();
            set => SetValue(NameProperty, value);
        }

        //public string TextChangedBind
        //{
        //    get => (string)GetValue(ItemsSourceProperty);
        //    set => SetValue(ItemsSourceProperty, value);
        //}

        //public AutoComplete()
        //{
        //    TextChanged += (sender, e) =>
        //    {
        //        TextChangedBind = e.NewTextValue;
        //    };
        //}
    }
}
