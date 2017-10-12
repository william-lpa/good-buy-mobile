using System;
using GoodBuy.Service;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Autofac;
using GoodBuy.Models;
using System.Collections.ObjectModel;
using GoodBuy.Models.Logical;
using System.Collections.Generic;

namespace GoodBuy.ViewModels
{
    public class LoginPageViewModel : BaseViewModel
    {
        private readonly AzureService azureService;
        public Command FacebookLoginCommand { get; }
        public Command ContactListLoginCommand { get; }
        public Command ContactListCommand { get; }
        private bool isRunning;
        public bool IsLoading
        {
            get => isRunning;
            set => SetProperty(ref isRunning, value);
        }
        public bool NotRunning => !IsLoading;


        public ObservableCollection<OfertaDto> UltimasOfertas { get; }
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
            IsLoading = true;
            OnPropertyChanged(nameof(NotRunning));
            azureService = azure;
            UltimasOfertas = new ObservableCollection<OfertaDto>();
            FacebookLoginCommand = new Command(ExecuteFacebookLogin, CanExecuteLogin);
            ContactListLoginCommand = new Command(ExecuteLocalProfileLogin, CanExecuteLogin);
            ContactListCommand = new Command(ExecuteOpenContactList);
        }

        private async Task InitializeCollection(OfertasService service)
        {
            var ofertas = await service.ObterUltimasTresOfertasFromServer();

            if (ofertas != null)
            {
                UltimasOfertas.Clear();
                foreach (var item in ofertas)
                {
                    UltimasOfertas.Add(item);
                }
            }
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

        public async void TrySSO(string param)
        {
            while (azureService.LoginIn) await Task.Delay(200);

            if (azureService.CurrentUser != null)
            {
                await this.PushAsync<MainMenuViewModel>(resetNavigation: true);
                if (param == "grupos")
                    await this.PushAsync<GruposOfertasPageViewModel>();
                else if (param != null)
                    await this.PushAsync<OfertasTabDetailPageViewModel>(false, new Dictionary<string, string>() { ["ID"] = param });
            }
            else
            {
                using (var scope = App.Container.BeginLifetimeScope())
                {
                    ContactPicked(profileUserContact = scope.Resolve<IContactListService>().PickProfileUser());
                    await InitializeCollection(scope.Resolve<OfertasService>());
                    IsLoading = false;
                    OnPropertyChanged(nameof(NotRunning));
                }
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
                await PushAsync<ContactLoginPageViewModel>(false, null, new NamedParameter(nameof(profileUserContact), profileUserContact));
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
                await PushModalAsync<LoadingPageViewModel>(null, new NamedParameter("operation", Operation.Login));
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
