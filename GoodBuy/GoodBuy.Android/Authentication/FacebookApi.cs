using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using GoodBuy.Models;
using GoodBuy.Models.Abstraction;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;

namespace goodBuy.Droid.Authentication
{
    [Activity(Label = "FacebookApi", Icon = "@drawable/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class FacebookApi : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IFacebookCallback
    {
        private ICallbackManager callbackManager;
        public static LoginResultContent LoginResult { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var intent = Intent;
            if (intent.GetBooleanExtra("LOGIN", false))
                LoginFacebook();
            if (intent.GetBooleanExtra("LOGOUT", false))
                LogOut();
        }


        public void LoginFacebook()
        {
            callbackManager = CallbackManagerFactory.Create();
            LoginManager.Instance.RegisterCallback(callbackManager, this);
            LoginManager.Instance.LogInWithReadPermissions(this, new[] { "public_profile", "user_friends", "email", "user_birthday", "user_location" });
        }
        private async void LogOut()
        {
            if (AccessToken.CurrentAccessToken != null && Profile.CurrentProfile != null)
            {
                await System.Threading.Tasks.Task.Run(() => LoginManager.Instance.LogOut());
            }
        }

        public void OnCancel()
        {
            LoginResult = new LoginResultContent(null, "User has canceled the login", Result.Canceled);
            //throw new NotImplementedException();
        }

        public void OnError(FacebookException error)
        {
            LoginResult = new LoginResultContent(null, $"error:{error.ToString()}", Result.FirstUser);
            //throw new NotImplementedException();
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            var login = result as LoginResult;
            var token = login.AccessToken;
            var g = Profile.CurrentProfile;
            var t = GraphRequest.NewMeRequest(AccessToken.CurrentAccessToken, null);
            Bundle parameters = new Bundle();
            t.Parameters.PutString("fields", "id,name,link,picture");
            var gg = t.ExecuteAsync();
            var user = new User()
            {
                FacebookId = token.UserId

            };
            LoginResult = new LoginResultContent(user);
          
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
            SetResult(resultCode);
            Finish();
        }
        
    }
}