using GoodBuy.Authentication;
using GoodBuy.Model;
using GoodBuy.Models;
using GoodBuy.Models.Many_to_Many;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GoodBuy.Service
{
    public class AzureService
    {
        private const string appURL = "https://good-buy.azurewebsites.net";
        public MobileServiceClient Client { get; set; }
        public MobileServiceSQLiteStore Store { get; private set; }
        public Dictionary<string, object> Tables { get; private set; }

        public async Task<MobileServiceUser> LoginAsync(MobileServiceAuthenticationProvider provider)
        {
            var auth = DependencyService.Get<IAuthentication>();

            Client.CurrentUser = auth.RetrieveTokenFromSecureStore();
            if (Client.CurrentUser != null)
            {
                // User has previously been authenticated - try to Refresh the token
                try
                {
                    var refreshed = await Client.RefreshUserAsync();
                    if (refreshed != null)
                    {
                        auth.StoreTokenInSecureStore(refreshed);
                        return refreshed;
                    }
                }
                catch (Exception refreshException)
                {
                    Log.Log.Instance.AddLog($"Could not refresh token: {refreshException.Message}");
                }
            }

            if (Client.CurrentUser != null && !IsTokenExpired(Client.CurrentUser.MobileServiceAuthenticationToken))
            {
                // User has previously been authenticated, no refresh is required
                return Client.CurrentUser;
            }

            // We need to ask for credentials at this point
            //auth.LoginClientFlow();
            if (Client.CurrentUser != null)
            {
                auth.StoreTokenInSecureStore(Client.CurrentUser);
            }
            await Client.SyncContext.InitializeAsync(Store, new MobileServiceSyncHandler());
            return Client.CurrentUser;
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
            if (Client.CurrentUser == null || Client.CurrentUser.MobileServiceAuthenticationToken == null)
                return;

            // Log out of the identity provider (if required)
            
            // Invalidate the token on the mobile backend
            var authUri = new Uri($"{Client.MobileAppUri}/.auth/logout");
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("X-ZUMO-AUTH", Client.CurrentUser.MobileServiceAuthenticationToken);
                await httpClient.GetAsync(authUri);
            }
            // Remove the token from the cache
            //DependencyService.Get<IAuthentication>().RemoveTokenFromSecureStore();
            // Remove the token from the MobileServiceClient
            await Client.LogoutAsync();
        }

        public AzureService() => Initialize();

        private async void Initialize()
        {
            try
            {
                Client = new MobileServiceClient(appURL, new ExpiredAzureRequestInterceptors(this));
                Tables = new Dictionary<string, object>();
                var dbName = "goodBuy5.db";
                Store = new MobileServiceSQLiteStore(Path.Combine(MobileServiceClient.DefaultDatabasePath, dbName));
                DefineTables(Store);
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
        }
    }
}
