using GoodBuy.Service;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace GoodBuy.ViewModels
{
    class MainMenuViewModel : BaseViewModel
    {

        public Command NovaOfertaCommand { get; }
        public ICommand SignOutCommand { get; }

        public ImageSource Profile => ImageSource.FromUri(new Uri(azure.CurrentUser.User.Avatar));
        public String UserName => azure.CurrentUser.User.FullName;

        private readonly AzureService azure;
        public MainMenuViewModel(AzureService azureService)
        {
            azure = azureService;
            NovaOfertaCommand = new Command(ExecuteCadastrarNovaOferta);
            SignOutCommand = new Command(ExecuteSignOut);
        }

        private async void ExecuteSignOut()
        {
            await azure.LogoutAsync();
            await PopToRootAsync();
        }

        private async void ExecuteCadastrarNovaOferta(object obj)
        {
            await PushModalAsync<NovaOfertaViewModel>(azure);
        }
    }
}
