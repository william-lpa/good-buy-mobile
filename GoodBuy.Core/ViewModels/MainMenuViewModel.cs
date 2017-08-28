using GoodBuy.Service;
using System;
using Xamarin.Forms;

namespace GoodBuy.ViewModels
{
    class MainMenuViewModel : BaseViewModel
    {
        public string Token { get; }
        public Command NovaOfertaCommand { get; }
        private readonly AzureService azure;
        public MainMenuViewModel(string token, AzureService azureService)
        {
            azure = azureService;
            Token = token;
            NovaOfertaCommand = new Command(ExecuteCadastrarNovaOferta);
        }

        private async void ExecuteCadastrarNovaOferta(object obj)
        {
            await PushModalAsync<NovaOfertaViewModel>(azure);
        }
    }
}
