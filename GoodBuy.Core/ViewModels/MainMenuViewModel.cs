using Autofac;
using GoodBuy.Service;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace GoodBuy.ViewModels
{
    public class MainMenuViewModel : BaseViewModel
    {

        public ICommand NovaOfertaCommand { get; }
        public ICommand SignOutCommand { get; }
        public ICommand ListarOfertasCommand { get; }
        public ICommand ListarGrupoOfertasCommand { get; }
        private readonly AzureService azure;
        private readonly SyncronizedAccessService syncronizeAccessService;
        public ImageSource Profile
        {
            get
            {
                if (azure?.CurrentUser?.User.FacebookId != null)
                    return ImageSource.FromUri(new Uri(azure?.CurrentUser?.User?.Avatar));

                if (azure?.CurrentUser?.User?.Avatar != null)
                    return ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(azure?.CurrentUser?.User?.Avatar)));
                return null;
            }
        }
        public string UserName => azure.CurrentUser?.User?.FullName;

        public MainMenuViewModel(AzureService azureService, SyncronizedAccessService syncronizeAccessService)
        {
            azure = azureService;
            this.syncronizeAccessService = syncronizeAccessService;
            NovaOfertaCommand = new Command(ExecuteCadastrarNovaOferta);
            ListarOfertasCommand = new Command(ExecuteListarOfertasCadastradas);
            ListarGrupoOfertasCommand = new Command(ExecuteListarGrupoOfertas);
            SignOutCommand = new Command(ExecuteSignOut);

            InitilizeDatabase();
        }

        private async void InitilizeDatabase()
        {
            if (await syncronizeAccessService.FirstUsage())
            {
                await Log.MessageDisplayer.Instance.ShowMessage("Sincronizar base de dados", "O aplicativo irá sincronizar a base de dados para um primeiro uso. Isto pode levar alguns segundos", "OK");
                await PushModalAsync<LoadingPageViewModel>(new NamedParameter("operation", Operation.SyncInitalDataBase));
                syncronizeAccessService.SyncronizeFirstUse();
                await PopModalAsync();
            }

        }

        private async void ExecuteListarGrupoOfertas()
        {
            await PushAsync<GruposOfertasPageViewModel>();
        }

        private async void ExecuteListarOfertasCadastradas()
        {
            await PushAsync<OfertasPageViewModel>();
        }
        private async void ExecuteSignOut()
        {
            await azure.LogoutAsync();
            await PushAsync<LoginPageViewModel>(resetNavigation: true);
        }

        private async void ExecuteCadastrarNovaOferta()
        {
            await PushModalAsync<NovaOfertaViewModel>();
        }
    }
}
