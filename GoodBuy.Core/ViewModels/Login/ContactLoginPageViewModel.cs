using GoodBuy.Service;
using System;
using Xamarin.Forms;
using Autofac;
using GoodBuy.Models;
using Microsoft.WindowsAzure.MobileServices;

namespace GoodBuy.ViewModels
{
    public class ContactLoginPageViewModel : BaseViewModel
    {
        private readonly AzureService azureService;
        private string email;
        private string name;
        private string phoneNumber;
        private bool male;
        private DateTime birth;
        private string city;
        private User profileUserContact;
        public Command ProfileDeviceLoginCommand { get; }
        public Command SearchContactProfile { get; }

        public string City
        {
            get { return city; }
            set
            {
                if (SetProperty(ref city, value))
                    ProfileDeviceLoginCommand.ChangeCanExecute();
            }
        }

        public DateTime Birth
        {
            get { return birth; }
            set
            {
                if (SetProperty(ref birth, value))
                    ProfileDeviceLoginCommand.ChangeCanExecute();
            }
        }

        public bool Male
        {
            get { return male; }
            set
            {
                if (SetProperty(ref male, value))
                    ProfileDeviceLoginCommand.ChangeCanExecute();
            }
        }

        public string PhoneNumber
        {
            get { return phoneNumber; }
            set
            {
                if (SetProperty(ref phoneNumber, value))
                    ProfileDeviceLoginCommand.ChangeCanExecute();
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (SetProperty(ref name, value))
                    ProfileDeviceLoginCommand.ChangeCanExecute();
            }
        }

        public string Email
        {
            get { return email; }
            set
            {
                if (SetProperty(ref email, value))
                    ProfileDeviceLoginCommand.ChangeCanExecute();
            }
        }

        public ContactLoginPageViewModel(AzureService azureService, User profileUserContact)
        {
            this.profileUserContact = profileUserContact;
            this.azureService = azureService;
            ProfileDeviceLoginCommand = new Command(ExecuteLoginWithLocalProfile, CanLoginWithLocalProfile);
            SearchContactProfile = new Command(ExecuteOpenContactList);
            Adapter(profileUserContact);
        }

        private void ExecuteOpenContactList()
        {
            using (var scope = App.Container.BeginLifetimeScope())
                scope.Resolve<IContactListService>().PickContactList(ContactPicked);
        }

        private void ContactPicked(User user)
        {
            Adapter(user);
            profileUserContact = user;
        }

        private async void ExecuteLoginWithLocalProfile()
        {
            var currentUser = Transform();
            await azureService.LoginAsync(MobileServiceAuthenticationProvider.Google, currentUser);
            await PushAsync<MainMenuViewModel>(resetNavigation: true);
        }

        private bool CanLoginWithLocalProfile()
        {
            return (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Email)
                            && !string.IsNullOrEmpty(PhoneNumber)
                            && !string.IsNullOrEmpty(City));
        }

        private User Transform()
        {
            User user = profileUserContact;
            user.FullName = Name;
            user.Email = Email;
            user.Birthday = Birth;
            user.Id = PhoneNumber;
            user.Male = Male;
            user.Location = city;
            return user;
        }

        private void Adapter(User currentUser)
        {
            var (fullName, id, birthday, email, male, location) = currentUser;
            Name = !string.IsNullOrEmpty(fullName) ? fullName : Name;
            PhoneNumber = !string.IsNullOrEmpty(id) ? id : PhoneNumber;
            Birth = birthday != default(DateTime) ? birthday : Birth;
            Email = !string.IsNullOrEmpty(email) ? email : Email;
            City = !string.IsNullOrEmpty(location) ? location : City;
        }
    }
}
