using Autofac;
using GoodBuy.Service;
using GoodBuy.Service.Interfaces;
using GoodBuy.ViewModels;
using GoodBuy.ViewModels.ListaDeCompras;
using GoodBuy.Views;
using Xamarin.Forms;

namespace GoodBuy
{
    public partial class App : Application
    {
        public static IContainer Container { get; private set; }
        public App() => InitializeComponent();
        public App(ContainerBuilder container, string param)
        {
            InitializeComponent();

            Container = BuildDependencies(container).Build();

            MainPage = new NavigationPage(new LoginPage(param));
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }
        private ContainerBuilder BuildDependencies(ContainerBuilder container)
        {
            container.RegisterType<AzureService>().SingleInstance();
            container.RegisterType<LoginPageViewModel>().SingleInstance();
            container.RegisterType<MainMenuPageViewModel>();
            container.RegisterType<OfertaDetalhePageViewModel>();
            container.RegisterType<OfertasPageViewModel>();
            container.RegisterType<LoadingPageViewModel>().SingleInstance();
            container.RegisterType<ContactLoginPageViewModel>().SingleInstance();
            container.RegisterType<GruposOfertasPageViewModel>();
            container.RegisterType<NovoGrupoOfertaPageViewModel>();
            container.RegisterType<GrupoOfertaService>();
            container.RegisterType<OfertasService>();
            container.RegisterType<UserService>();
            container.RegisterType<OfertaDetalhePageViewModel>();
            container.RegisterType<CompartilharOfertasPageViewModel>();
            container.RegisterType<ListagemUsuariosPageViewModel>();
            container.RegisterType<OfertasTabDetailPageViewModel>();
            container.RegisterType<HistoricosOfertaPageViewModel>();
            container.RegisterType<ListaDeComprasPageViewModel>();
            container.RegisterType<ListaDeComprasDetalhePageViewModel>();
            container.RegisterType<ListaDeComprasPageParticipantesViewModel>();
            container.RegisterType<SimulacaoCompraPageViewModel>();
            container.RegisterType<SimulacaoCompraDetalhePageViewModel>();
            container.RegisterType<ListaCompraService>();
            container.RegisterType<SyncronizedAccessService>().SingleInstance();
            container.RegisterType<FiltroSimulacaoCompraService>().As<IFilterSimulacaoCompra>().SingleInstance();
            container.RegisterType<GenericRepository<Model.IEntity>>();
            return container;
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps

            using (var scope = App.Container?.BeginLifetimeScope())
            {
                var azure = scope?.Resolve<AzureService>();
                if (azure.LoginIn) return;
                scope?.Resolve<SyncronizedAccessService>()?.SyncronizeFirstUseAsync();
            }
        }

        protected override void OnResume()
        {

            // Handle when your app resumes
            using (var scope = App.Container?.BeginLifetimeScope())
            {
                var azure = scope?.Resolve<AzureService>();
                if (azure.LoginIn) return;
                scope?.Resolve<SyncronizedAccessService>()?.SyncronizeFirstUseAsync();
                azure.CreateOrRefreshPushRegistration();
            }
        }
    }
}
