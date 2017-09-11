using System;
using GoodBuy.Service;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Autofac;
using GoodBuy.Core.ViewModels;
using System.Windows.Input;
using GoodBuy.Models;

namespace GoodBuy.ViewModels
{
    public class LoginPageViewModel : BaseViewModel
    {
        private readonly AzureService azureService;
        public Command FacebookLoginCommand { get; }
        public Command ContactListLoginCommand { get; }
        public Command ContactListCommand { get; }
        private User profileUserContact;
        private string phoneNumber;
        private string phoneLabel;

        public string PhoneNumber
        {
            get { return phoneNumber; }
            set
            {
                if (SetProperty(ref phoneNumber, value))
                {
                    ContactListLoginCommand.ChangeCanExecute();
                    FacebookLoginCommand.ChangeCanExecute();
                }
            }
        }
        public string PhoneLabel
        {
            get { return phoneLabel; }
            set { SetProperty(ref phoneLabel, value); }
        }

        public LoginPageViewModel(AzureService azure)
        {
            azureService = azure;
            FacebookLoginCommand = new Command(ExecuteFacebookLogin, CanExecuteLogin);
            ContactListLoginCommand = new Command(ExecuteLocalProfileLogin, CanExecuteLogin);
            ContactListCommand = new Command(ExecuteOpenContactList);
        }

        private void ContactPicked(User user)
        {
            this.profileUserContact = user;
            PhoneNumber = user?.Id;
            AlterarLegendaTelefone();
        }
        private void ExecuteOpenContactList()
        {
            using (var scope = App.Container.BeginLifetimeScope())
                scope.Resolve<IContactListService>().PickContactList(ContactPicked);
        }

        public bool CanExecuteLogin() => !string.IsNullOrEmpty(PhoneNumber) && PhoneNumber.Length >= 8;

        public async void TrySSO()
        {
            while (azureService.LoginIn) await Task.Delay(200);

            if (azureService.CurrentUser != null)
                await this.PushAsync<MainMenuViewModel>(resetNavigation: true);
            else
            {
                using (var scope = App.Container.BeginLifetimeScope())
                    ContactPicked(profileUserContact = scope.Resolve<IContactListService>().PickProfileUser());
            }
        }

        private void AlterarLegendaTelefone()
        {
            if (string.IsNullOrEmpty(PhoneNumber))
                PhoneLabel = "Favor informar o número do dispositivo. Não foi possível detectar automaticamente";
            else
                PhoneLabel = "Favor confirmar o número do dispositivo detectado";
        }
        private void VerifyNumberManualChange()
        {
            if (profileUserContact.Id != PhoneNumber)
            {
                profileUserContact.Id = PhoneNumber;
            }
        }

        private async void ExecuteLocalProfileLogin()
        {
            try
            {
                VerifyNumberManualChange();
                await PushAsync<ContactLoginPageViewModel>(false, new NamedParameter(nameof(profileUserContact), profileUserContact));
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }

        private async void ExecuteFacebookLogin()
        {
            try
            {
                VerifyNumberManualChange();
                await PushModalAsync<LoadingPageViewModel>();
                await azureService.LoginAsync(MobileServiceAuthenticationProvider.Facebook, profileUserContact);
                await PushAsync<MainMenuViewModel>(resetNavigation: true);
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }
    }
}
