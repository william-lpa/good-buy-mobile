using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoodBuy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoricosOfertaPage : ContentPage
    {
        public HistoricosOfertaPage()
        {
            InitializeComponent();
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            if (campoMonitore.IsEnabled)
            {
                campoMonitore.IsEnabled = e.Value;
                campoMonitore.Text= string.Empty;
            }
            else
                campoMonitore.IsEnabled = e.Value;
        }
    }
}