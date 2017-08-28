using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoodBuy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NovaOferta : ContentPage
    {

        public event EventHandler<FocusEventArgs> ValidatableEntries = delegate { };
        public NovaOferta()
        {
            InitializeComponent();
        }

        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            //(BindingContext as ViewModels.NovaOfertaViewModel).ValidatableEntries<string>(nameof(produto));
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            sabor.IsEnabled = e.Value;
        }
    }
}