using GoodBuy.Authentication;
using GoodBuy.Model;
using GoodBuy.Models;
using GoodBuy.Models.Many_to_Many;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GoodBuy.Service
{
    public class AzureService
    {
        private const string appURL = "https://good-buy.azurewebsites.net";
        public MobileServiceClient Client { get; set; }
        public Dictionary<string, IMobileServiceSyncTable<IEntity>> Tables { get; set; }
        private IMobileServiceSyncTable<Person> localPeople;
        private IMobileServiceSyncTable<Produto> produtos;
        private IMobileServiceTable<Person> serverPeople;

        public async Task<MobileServiceUser> LoginAsync(MobileServiceAuthenticationProvider provider)
        {
            Tables = new Dictionary<string, IMobileServiceSyncTable<IEntity>>();

            Client = new MobileServiceClient(appURL);

            var auth = DependencyService.Get<IAuthentication>();
            var user = await auth.LoginAsync(Client, provider);

            if (user == null)
            {
                Device.BeginInvokeOnMainThread(async () => await App.Current.MainPage.DisplayAlert("Ops!", "Não conseguimos efetuar seu login!", "OK"));
            }
            await Initialize();
            return user;
        }

        private async Task Initialize()
        {
            try
            {
                if (Client?.SyncContext?.IsInitialized ?? false)
                    return;

                var dbName = "goodBuy.db";
                var store = new MobileServiceSQLiteStore(Path.Combine(MobileServiceClient.DefaultDatabasePath, dbName));
                DefineTables(store);
                if (Client.CurrentUser.MobileServiceAuthenticationToken == null)
                    await LoginAsync(MobileServiceAuthenticationProvider.Facebook);
                await Client.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }

        public IMobileServiceSyncTable<TEntity> GetTable<TEntity>() where TEntity : class, IEntity
        {
            try
            {
                var tableName = typeof(TEntity).Name;

                if (!Tables.ContainsKey(tableName))
                    Tables.Add(tableName, Client.GetSyncTable<TEntity>() as IMobileServiceSyncTable<IEntity>);

                return Tables[tableName] as IMobileServiceSyncTable<TEntity>;
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
            return null;
        }

        private void DefineTables(MobileServiceSQLiteStore store)
        {
            store.DefineTable<Categoria>();
            store.DefineTable<Estabelecimento>();
            store.DefineTable<HistoricoOferta>();
            store.DefineTable<Marca>();
            store.DefineTable<Produto>();
            store.DefineTable<Person>();
            store.DefineTable<Sabor>();
            store.DefineTable<UnidadeMedida>();
            store.DefineTable<CarteiraProduto>();
            store.DefineTable<Oferta>();
        }



    }
}
