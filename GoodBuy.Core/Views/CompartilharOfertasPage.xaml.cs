using Autofac;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoodBuy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompartilharOfertasPage : TabbedPage
    {
        public CompartilharOfertasPage()
        {
            InitializeComponent();
            using (var scope = App.Container.BeginLifetimeScope())
            {
                var usuariosVm = scope.Resolve<ViewModels.ListagemUsuariosPageViewModel>();
                usuarios.BindingContext = usuariosVm;

                var grupoofertaVm = scope.Resolve<ViewModels.GruposOfertasPageViewModel>();
                grupoofertaVm.NotSharing = false;
                grupoOfertas.BindingContext = grupoofertaVm;

            }
        }
    }
}