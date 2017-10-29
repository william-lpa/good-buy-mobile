using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoodBuy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListaDeComprasPageParticipantes : ContentPage
    {
        public ListaDeComprasPageParticipantes()
        {
            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

        }
    }
}