using GoodBuy.Authentication;
using GoodBuy.Model;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GoodBuy.Service
{
    public class AzureService
    {
        private const string appURL = "https://good-buy.azurewebsites.net";
        public MobileServiceClient Client { get; set; }
        private IMobileServiceSyncTable<Person> localPeople;
        private IMobileServiceTable<Person> serverPeople;

        public async Task<MobileServiceUser> LoginAsync(MobileServiceAuthenticationProvider provider)
        {
            Client = new MobileServiceClient(appURL);

            var auth = DependencyService.Get<IAuthentication>();
            var user = await auth.LoginAsync(Client, provider);

            if (user == null)
            {
                Device.BeginInvokeOnMainThread(async () => await App.Current.MainPage.DisplayAlert("Ops!", "Não conseguimos efetuar seu login!", "OK"));
            }
            return user;
        }

        private async Task Initialize()
        {
            try
            {
                if (Client?.SyncContext?.IsInitialized ?? false)
                    return;

                await LoginAsync(MobileServiceAuthenticationProvider.Facebook);
                var dbName = "goodBuy.db";
                var store = new MobileServiceSQLiteStore(Path.Combine(MobileServiceClient.DefaultDatabasePath, dbName));
                DefineTables(store);
                if (Client.CurrentUser.MobileServiceAuthenticationToken == null)
                    await LoginAsync(MobileServiceAuthenticationProvider.Facebook);

                await Client.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());
                GeTables();
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }

        private void GeTables()
        {
            localPeople = Client.GetSyncTable<Person>(); //reflexao?
            serverPeople = Client.GetTable<Person>();
        }

        private void DefineTables(MobileServiceSQLiteStore store)
        {
            store.DefineTable<Person>();
        }
        

    }
}
