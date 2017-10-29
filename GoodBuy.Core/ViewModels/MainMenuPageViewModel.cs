using Autofac;
using GoodBuy.Service;
using System;
using System.IO;
using System.Windows.Input;
using Xamarin.Forms;

namespace GoodBuy.ViewModels
{
    public class MainMenuPageViewModel : BaseViewModel
    {
        public ICommand NovaOfertaCommand { get; }
        public ICommand SignOutCommand { get; }
        public ICommand ListarOfertasCommand { get; }
        public ICommand ListarGrupoOfertasCommand { get; }
        public ICommand ListasDeComprasCommand { get; }

        private readonly AzureService azure;
        private readonly SyncronizedAccessService syncronizeAccessService;
        public ImageSource Profile
        {
            get
            {
                if (azure?.CurrentUser?.User?.FacebookId != null)
                    return ImageSource.FromUri(new Uri(azure?.CurrentUser?.User?.Avatar));

                if (azure?.CurrentUser?.User?.Avatar != null)
                    return ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(azure?.CurrentUser?.User?.Avatar)));
                return null;
            }
        }
        public string UserName => azure.CurrentUser?.User?.FullName;

        public MainMenuPageViewModel(AzureService azureService, SyncronizedAccessService syncronizeAccessService)
        {
            azure = azureService;
            this.syncronizeAccessService = syncronizeAccessService;
            NovaOfertaCommand = new Command(ExecuteCadastrarNovaOfertaAsync);
            ListarOfertasCommand = new Command(ExecuteListarOfertasCadastradasAsync);
            ListarGrupoOfertasCommand = new Command(ExecuteListarGrupoOfertasAsync);
            ListasDeComprasCommand = new Command(ExecuteListasDeComprasAsync);
            SignOutCommand = new Command(ExecuteSignOutAsync);

            InitilizeDatabaseAsync();
        }

        private async void ExecuteListasDeComprasAsync()
        {
            await PushAsync<ListaDeComprasPageViewModel>();
        }

        private async void InitilizeDatabaseAsync()
        {
            if (await syncronizeAccessService.FirstUsageAsync())
            {
                await Log.MessageDisplayer.Instance.ShowMessageAsync("Sincronizar base de dados", "O aplicativo irá sincronizar a base de dados para um primeiro uso. Isto pode levar alguns segundos", "OK");
                await PushModalAsync<LoadingPageViewModel>(null, new NamedParameter("operation", Operation.SyncInitalDataBase));
                syncronizeAccessService.SyncronizeFirstUseAsync();
                await PopModalAsync();
            }
        }

        private async void ExecuteListarGrupoOfertasAsync()
        {
            await PushAsync<GruposOfertasPageViewModel>();
        }

        private async void ExecuteListarOfertasCadastradasAsync()
        {
            await PushAsync<OfertasPageViewModel>();
        }
        private async void ExecuteSignOutAsync()
        {
            await azure.LogoutAsync();
            await PushAsync<LoginPageViewModel>(resetNavigation: true);
        }

        private async void ExecuteCadastrarNovaOfertaAsync()
        {
            await PushModalAsync<OfertaDetalhePageViewModel>();
        }
    }
}
