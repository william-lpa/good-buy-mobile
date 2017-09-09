using Autofac;
using GoodBuy.Core.ViewModels;
using GoodBuy.Service;
using GoodBuy.ViewModels;
using NUnit.Framework;

namespace UITest1
{
    public abstract class BaseTest
    {
        protected IContainer Container { get; private set; }
        protected AzureService azureService;

        [SetUp]
        public void BeforeBaseEachTest()
        {
            Container = BuildDependencies().Build();
            Initilialize();
        }

        private ContainerBuilder BuildDependencies()
        {
            var container = new ContainerBuilder();

            container.RegisterType<AzureService>().SingleInstance();
            container.RegisterType<LoginPageViewModel>();
            container.RegisterType<MainMenuViewModel>();
            container.RegisterType<NovaOfertaViewModel>();
            container.RegisterType<LoadingPageViewModel>().SingleInstance();
            container.RegisterType<GenericRepository<GoodBuy.Model.IEntity>>();
            return container;
        }

        private void Initilialize()
        {
            using (var scope = Container.BeginLifetimeScope())
                azureService = scope.Resolve<AzureService>();
        }

    }
}
