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
            tipo.IsEnabled = e.Value;
            if (!tipo.IsEnabled)
                tipo.Text = "";
        }

        private void SwitchMarca_Toggled(object sender, ToggledEventArgs e)
        {
            marca.IsEnabled =!e.Value;
            if (marca.IsEnabled)
                marca.Text = "";
        }
    }
}