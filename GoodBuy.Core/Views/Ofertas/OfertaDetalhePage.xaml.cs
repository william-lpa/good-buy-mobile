using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoodBuy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OfertaDetalhePage : ContentPage
    {
        public OfertaDetalhePage()
        {
            InitializeComponent();
        }
        
        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            sabor.IsEnabled = e.Value;
        }
    }
}