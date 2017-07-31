using System;
using GoodBuy.Service;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using GoodBuy.Authentication;

namespace GoodBuy.ViewModels
{
    class LoginPageViewModel : BaseViewModel
    {
        private readonly AzureService azureService;
        public Command FacebookLoginCommand { get; }
        public Command GoogleLoginCommand { get; }

        public LoginPageViewModel()
        {
            azureService = new AzureService();
            FacebookLoginCommand = new Command(ExecuteFacebookLogin);
            GoogleLoginCommand = new Command(ExecuteGoogleLogin);
        }

        private void ExecuteGoogleLogin()
        {
            ExecuteLogin(MobileServiceAuthenticationProvider.Google);
        }

        private void ExecuteFacebookLogin()
        {
            ExecuteLogin(MobileServiceAuthenticationProvider.Facebook);
        }
        private async void ExecuteLogin(MobileServiceAuthenticationProvider provider)
        {
            var user = await azureService.LoginAsync(provider);

            Device.BeginInvokeOnMainThread(async () => await App.Current.MainPage.DisplayAlert("Logado", $"Logado com sucesso!", "OK"));
            await PushAsync<MainMenuViewModel>(user.UserId ?? "error");
        }
    }
}
