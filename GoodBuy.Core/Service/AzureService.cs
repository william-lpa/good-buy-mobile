﻿using Autofac;
using GoodBuy.Authentication;
using GoodBuy.Model;
using GoodBuy.Models;
using GoodBuy.Models.Abstraction;
using GoodBuy.Models.Many_to_Many;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;

namespace GoodBuy.Service
{
    public class AzureService
    {
        private const string appURL = "https://good-buy.azurewebsites.net";
        public MobileServiceClient Client { get; set; }
        private static AccountStore storeAccount;
        public MobileServiceSQLiteStore Store { get; private set; }
        public Dictionary<string, object> Tables { get; private set; }
        public LoginResultContent CurrentUser { get; private set; }
        public bool LoginIn { get; private set; }

        public AzureService() => Initialize();

        public async Task LoginAsync(MobileServiceAuthenticationProvider provider)
        {
            try
            {
                IAuthentication auth;
                using (var scope = App.Container.BeginLifetimeScope())
                    auth = scope.Resolve<IAuthentication>();

                Task initialized = null;
                if (!Client.SyncContext.IsInitialized)
                    initialized = Client.SyncContext.InitializeAsync(Store, new MobileServiceSyncHandler());

                // We need to ask for credentials at this point
                var result = await auth.LoginClientFlowAsync(Client, provider);
                Client.CurrentUser = result.azureUser;
                CurrentUser = result.appUser;
                if (initialized != null)
                    await initialized;
                
                var table = Client.GetSyncTable<User>();
                if (await table.LookupAsync(CurrentUser.User.Id) == null)
                {
                    await table.InsertAsync(CurrentUser.User);
                    await Client.SyncContext.PushAsync();
                }
                if (Client.CurrentUser != null)
                {
                    StoreTokenInSecureStore(Client.CurrentUser.UserId, "azure", Client.CurrentUser.MobileServiceAuthenticationToken);
                    StoreTokenInSecureStore(CurrentUser.User.Id, "facebook", CurrentUser.Token);
                }
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }
        bool IsTokenExpired(string token)
        {
            // Get just the JWT part of the token (without the signature).
            var jwt = token.Split(new Char[] { '.' })[1];

            // Undo the URL encoding.
            jwt = jwt.Replace('-', '+').Replace('_', '/');
            switch (jwt.Length % 4)
            {
                case 0: break;
                case 2: jwt += "=="; break;
                case 3: jwt += "="; break;
                default:
                    throw new ArgumentException("The token is not a valid Base64 string.");
            }

            // Convert to a JSON String
            var bytes = Convert.FromBase64String(jwt);
            string jsonString = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

            // Parse as JSON object and get the exp field value,
            // which is the expiration date as a JavaScript primative date.
            JObject jsonObj = JObject.Parse(jsonString);
            var exp = Convert.ToDouble(jsonObj["exp"].ToString());

            // Calculate the expiration by adding the exp value (in seconds) to the
            // base date of 1/1/1970.
            DateTime minTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var expire = minTime.AddSeconds(exp);
            return (expire < DateTime.UtcNow);
        }
        public async Task LogoutAsync()
        {
            IAuthentication auth;
            using (var scope = App.Container.BeginLifetimeScope())
                auth = scope.Resolve<IAuthentication>();

            if (Client.CurrentUser == null || Client.CurrentUser.MobileServiceAuthenticationToken == null)
                return;

            auth.LogOut();
            // Invalidate the token on the mobile backend
            var authUri = new Uri($"{Client.MobileAppUri}/.auth/logout");
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("X-ZUMO-AUTH", Client.CurrentUser.MobileServiceAuthenticationToken);
                var request = httpClient.GetAsync(authUri);

                // Remove the token from the cache
                RemoveTokenFromSecureStore();
                // Remove the token from the MobileServiceClient
                await Client.LogoutAsync();
                await request;
            }
        }
        private void Initialize()
        {
            try
            {
                //RemoveTokenFromSecureStore();
                Client = new MobileServiceClient(appURL, new ExpiredAzureRequestInterceptors(this));
                var dbName = "goodBuy9.db";
                Store = new MobileServiceSQLiteStore(Path.Combine(MobileServiceClient.DefaultDatabasePath, dbName));
                DefineTables(Store);

                Tables = new Dictionary<string, object>();
                Task.Run(() => DoSSOLogin());
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }

        public async void DoSSOLogin()
        {
            LoginIn = true;
            Client.CurrentUser = RetrieveAzureTokenFromSecureStore("azure");
            if (Client.CurrentUser != null && !IsTokenExpired(Client.CurrentUser.MobileServiceAuthenticationToken))
            {
                await Client.SyncContext.InitializeAsync(Store, new MobileServiceSyncHandler());
                CurrentUser = await RetrieveFacebookUserFromSecureStore("facebook");
            }
            else
                Client.CurrentUser = null;

            LoginIn = false;
        }

        public IMobileServiceSyncTable<TEntity> GetTable<TEntity>() where TEntity : class, IEntity
        {
            try
            {
                var tableName = typeof(TEntity).Name;

                if (!Tables.ContainsKey(tableName))
                {
                    Tables.Add(tableName, Client.GetSyncTable<TEntity>() as object);
                }
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
            store.DefineTable<Person>();
            store.DefineTable<Produto>();
            store.DefineTable<Marca>();
            store.DefineTable<Sabor>();
            store.DefineTable<UnidadeMedida>();
            store.DefineTable<CarteiraProduto>();
            store.DefineTable<Oferta>();
            store.DefineTable<User>();
        }

        public void StoreTokenInSecureStore(string userId, string key, string token = "")
        {
            if (storeAccount == null)
                storeAccount = AccountStore.Create();
            var account = new Account(userId);
            account.Properties.Add("token", token);
            storeAccount.Save(account, key);
        }

        public async Task<LoginResultContent> RetrieveFacebookUserFromSecureStore(string key)
        {
            if (storeAccount == null)
                storeAccount = AccountStore.Create();
            var account = storeAccount.FindAccountsForService(key).FirstOrDefault();
            string token = null;
            if ((account?.Properties?.TryGetValue("token", out token)).GetValueOrDefault())
            {
                var user = await GetTable<User>().LookupAsync(account.Username);
                return new LoginResultContent(user)
                {
                    Token = token,
                    Result = Result.OK,
                    Message = "Retrieved from Account Store"
                };
            }
            return null;
        }

        public MobileServiceUser RetrieveAzureTokenFromSecureStore(string key)
        {
            if (storeAccount == null)
                storeAccount = AccountStore.Create();
            var account = storeAccount.FindAccountsForService(key).FirstOrDefault();
            string token = null;
            if ((account?.Properties?.TryGetValue("token", out token)).GetValueOrDefault())
            {
                return new MobileServiceUser(account.Username)
                {
                    MobileServiceAuthenticationToken = token
                };
            }
            return null;
        }
        public void RemoveTokenFromSecureStore()
        {
            if (storeAccount == null)
                storeAccount = AccountStore.Create();
            var azure = storeAccount.FindAccountsForService("azure").FirstOrDefault();
            var facebook = storeAccount.FindAccountsForService("facebook").FirstOrDefault();
            storeAccount?.Delete(azure, nameof(azure));
            storeAccount?.Delete(facebook, nameof(facebook));
        }
    }
}
