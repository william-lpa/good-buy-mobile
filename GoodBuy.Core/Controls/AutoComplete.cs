using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace GoodBuy.Core.Controls
{
    public class AutoComplete : Entry
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create("ItemsSource", typeof(List<string>), typeof(AutoComplete), null, BindingMode.OneWay);
        public static readonly BindableProperty TextChangedProperty = BindableProperty.Create("TextChangedBind", typeof(string), typeof(AutoComplete), null, BindingMode.TwoWay);


        public List<string> ItemsSource
        {
            get => (List<string>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
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
