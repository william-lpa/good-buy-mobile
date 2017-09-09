using GoodBuy.Service;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace GoodBuy.ViewModels
{
    public class MainMenuViewModel : BaseViewModel
    {

        public ICommand NovaOfertaCommand { get; }
        public ICommand SignOutCommand { get; }
        public ICommand ListarOfertasCommand { get; }

        public ImageSource Profile => ImageSource.FromUri(new Uri(azure.CurrentUser.User.Avatar));
        public string UserName => azure.CurrentUser.User.FullName;

        private readonly AzureService azure;
        public MainMenuViewModel(AzureService azureService)
        {
            azure = azureService;
            NovaOfertaCommand = new Command(ExecuteCadastrarNovaOferta);
            ListarOfertasCommand = new Command(ExecuteListarOfertasCadastradas);
            SignOutCommand = new Command(ExecuteSignOut);
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
