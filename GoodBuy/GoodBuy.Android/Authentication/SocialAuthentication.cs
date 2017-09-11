using System.Threading.Tasks;
using GoodBuy.Authentication;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Auth;
using Android.Content;
using GoodBuy.Log;
using GoodBuy.Models.Abstraction;

namespace goodBuy.Droid
{
    public class SocialAuthentication : IAuthentication
    {
        private static AccountStore storeAccount;
        public bool SignIn { get; set; }
        public LoginResultContent LoginResult { get; set; }

        public async Task<(MobileServiceUser azureUser, LoginResultContent appUser)> LoginClientFlowAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            try
            {
                SignIn = true;
                if (provider == MobileServiceAuthenticationProvider.Facebook)
                {
                    Intent facebookLogin = new Intent(MainActivity.CurrentActivity, typeof(Authentication.FacebookApi));
                    facebookLogin.PutExtra("LOGIN", true);
                    MainActivity.CurrentActivity.StartActivityForResult(facebookLogin, (int)ActivitiesRequestCode.RequestCodes.FACEBOOK_LOGIN);
                }
                while (SignIn) { await Task.Delay(500); }
                var azureUser = await LoginAzureAsync(client, provider);
                return (azureUser, LoginResult);
            }
            catch (Java.Lang.Exception err)
            {
                Log.Instance.AddLog(err);
            }
            catch (System.Exception err)
            {
                Log.Instance.AddLog(err);
            }
            return (null, null);
        }
        public void LogOut()
        {
            //SignIn = true;
            //Intent facebookLogin = new Intent(MainActivity.CurrentActivity, typeof(Authentication.FacebookApi));
            //facebookLogin.PutExtra("LOGOUT", true);
            //MainActivity.CurrentActivity.StartActivity(facebookLogin);
            Xamarin.Facebook.Login.LoginManager.Instance.LogOut();
            //while (SignIn)
            //{
            //    Task.Delay(500).Wait();
            //}
        }

        public async Task<MobileServiceUser> LoginAzureAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
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