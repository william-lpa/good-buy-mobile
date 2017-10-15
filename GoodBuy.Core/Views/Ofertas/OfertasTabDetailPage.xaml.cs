using Autofac;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoodBuy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OfertasTabDetailPage : TabbedPage
    {
        public OfertasTabDetailPage()
        {
            InitializeComponent();
            using (var scope = App.Container.BeginLifetimeScope())
            {
                ViewModels.OfertasTabDetailPageViewModel.Oferta = null;
                var ofertaInfoVm = scope.Resolve<ViewModels.OfertaDetalhePageViewModel>();
                ofertaInfoVm.EditingOferta = true;
                oferta.BindingContext = ofertaInfoVm;

                var historicoVm = scope.Resolve<ViewModels.HistoricosOfertaPageViewModel>();
                historico.BindingContext = historicoVm;
            }
        }
    }
}