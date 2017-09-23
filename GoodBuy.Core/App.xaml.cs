using Autofac;
using GoodBuy.Service;
using GoodBuy.ViewModels;
using GoodBuy.Views;
using Xamarin.Forms;

namespace GoodBuy
{
    public partial class App : Application
    {
        public static IContainer Container { get; private set; }
        public App() => InitializeComponent();
        public App(ContainerBuilder container)
        {
            InitializeComponent();

            Container = BuildDependencies(container).Build();

            MainPage = new NavigationPage(new LoginPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }
        private ContainerBuilder BuildDependencies(ContainerBuilder container)
        {
            container.RegisterType<AzureService>().SingleInstance();
            container.RegisterType<LoginPageViewModel>().SingleInstance();
            container.RegisterType<MainMenuViewModel>();
            container.RegisterType<NovaOfertaViewModel>();
            container.RegisterType<OfertasPageViewModel>();
            container.RegisterType<LoadingPageViewModel>().SingleInstance();
            container.RegisterType<ContactLoginPageViewModel>().SingleInstance();
            container.RegisterType<GruposOfertasPageViewModel>();
            container.RegisterType<NovoGrupoOfertaPageViewModel>();
            container.RegisterType<GrupoOfertaService>().SingleInstance();
            container.RegisterType<OfertasService>().SingleInstance();
            container.RegisterType<UserService>().SingleInstance();
            container.RegisterType<SyncronizedAccessService>().SingleInstance();
            container.RegisterType<GenericRepository<Model.IEntity>>();
            return container;
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
