using System.Threading.Tasks;
using GoodBuy.Authentication;
using Microsoft.WindowsAzure.MobileServices;
using goodBuy.Droid;
using Xamarin.Auth;
using Xamarin.Forms;
using System.Linq;
using Android.Content;
using GoodBuy.Log;
using GoodBuy.Models.Abstraction;

[assembly: Xamarin.Forms.Dependency(typeof(SocialAuthentication))]
namespace goodBuy.Droid
{
    public class SocialAuthentication : IAuthentication
    {
        private static AccountStore storeAccount;
        private MobileServiceClient client;
        private MobileServiceAuthenticationProvider provider;        
        
        //private SocialAuthentication(MobileServiceClient)
        //{

        //}

        public LoginResultContent LoginResult { get; set; }

        public void LoginClientFlow(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            try
            {
                this.client = client;
                this.provider = provider;                
                if (provider == MobileServiceAuthenticationProvider.Facebook)
                {
                    Intent facebookLogin = new Intent(MainActivity.CurrentActivity, typeof(Authentication.FacebookApi));
                    facebookLogin.PutExtra("LOGIN", true);
                    MainActivity.CurrentActivity.StartActivityForResult(facebookLogin, 1);
                }
            }
            catch (Java.Lang.Exception err)
            {
                Log.Instance.AddLog(err);
            }
            catch (System.Exception err)
            {
                Log.Instance.AddLog(err);
            }
        }
        public void LogOut()
        {
            Intent facebookLogin = new Intent(MainActivity.CurrentActivity, typeof(Authentication.FacebookApi));
            facebookLogin.PutExtra("LOGOUT", true);
            MainActivity.CurrentActivity.StartActivity(facebookLogin);
        }
        public void StoreTokenInSecureStore(MobileServiceUser user)
        {
            if (storeAccount == null)
                storeAccount = AccountStore.Create(Forms.Context);
            var account = new Account(user.UserId);
            account.Properties.Add("token", user.MobileServiceAuthenticationToken);
            storeAccount.Save(account, "tasklist");
        }

        public MobileServiceUser RetrieveTokenFromSecureStore()
        {
            if (storeAccount == null)
                storeAccount = AccountStore.Create(Forms.Context);
            var account = storeAccount.FindAccountsForService("user").FirstOrDefault();
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
                storeAccount = AccountStore.Create(Forms.Context);
            var account = storeAccount.FindAccountsForService("user").FirstOrDefault();
            storeAccount?.Delete(account, "user");
        }

        public async Task<MobileServiceUser> LoginAzureAsync()
        {
            var zumoPayload = new Newtonsoft.Json.Linq.JObject()
            {
                ["access_token"] = LoginResult.Token
            };
            var user = await client.LoginAsync(provider, zumoPayload);
            GcmService.Client = client;
            Gcm.Client.GcmClient.Register(MainActivity.CurrentActivity, PushHandlerBroadcastReceiver.SENDER_IDS);
            return user;
        }
    }
}