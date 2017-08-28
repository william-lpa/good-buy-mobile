using System;
using GoodBuy.Service;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;

namespace GoodBuy.ViewModels
{
    class LoginPageViewModel : BaseViewModel
    {
        private readonly AzureService azureService;
        public Command FacebookLoginCommand { get; }
        public Command GoogleLoginCommand { get; }

        public LoginPageViewModel(AzureService azure)
        {
            azureService = azure;
            FacebookLoginCommand = new Command(ExecuteFacebookLogin);
            GoogleLoginCommand = new Command(ExecuteGoogleLogin);
        }

        private void ExecuteGoogleLogin()
        {
            try
            {
                ExecuteLogin(MobileServiceAuthenticationProvider.Google);
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }

        private void ExecuteFacebookLogin()
        {
            try
            {
                ExecuteLogin(MobileServiceAuthenticationProvider.Facebook);
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }
        private async void ExecuteLogin(MobileServiceAuthenticationProvider provider)
        {
            try
            {
                var user = await azureService.LoginAsync(provider);
                //Device.BeginInvokeOnMainThread(async () => await App.Current.MainPage.DisplayAlert("Logado", $"Logado com sucesso!", "OK"));
                await PushAsync<MainMenuViewModel>("" ?? "error", azureService);
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }
    }
}
