using System;
using GoodBuy.Service;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Autofac;
using GoodBuy.Core.ViewModels;

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

        public async void TrySSO()
        {
            while (azureService.LoginIn) await Task.Delay(200);

            if (azureService.CurrentUser != null)
                await this.PushAsync<MainMenuViewModel>();
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
                await PushAsync<LoadingPageViewModel>();
                await azureService.LoginAsync(provider);
                await PushAsync<MainMenuViewModel>();
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }
    }
}
