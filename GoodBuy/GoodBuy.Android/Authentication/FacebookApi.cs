using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Autofac;
using GoodBuy;
using GoodBuy.Authentication;
using GoodBuy.Models;
using GoodBuy.Models.Abstraction;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;

namespace goodBuy.Droid.Authentication
{
    [Activity(Label = "FacebookApi", Icon = "@drawable/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class FacebookApi : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IFacebookCallback
    {
        private ICallbackManager callbackManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var intent = Intent;
            LoginFacebook();
        }
        public void LoginFacebook()
        {
            callbackManager = CallbackManagerFactory.Create();
            LoginManager.Instance.RegisterCallback(callbackManager, this);
            LoginManager.Instance.LogInWithReadPermissions(this, new[] { "public_profile", "user_friends", "email", "user_birthday", "user_location" });
        }
        public void OnCancel()
        {
            using (var scope = App.Container.BeginLifetimeScope())
                scope.Resolve<SocialAuthentication>().LoginResult = new LoginResultContent(null, "User has canceled the login", GoodBuy.Models.Abstraction.Result.Canceled);
            //throw new NotImplementedException();
        }

        public void OnError(FacebookException error)
        {
            using (var scope = App.Container.BeginLifetimeScope())
                scope.Resolve<SocialAuthentication>().LoginResult = new LoginResultContent(null, $"error:{error.ToString()}", GoodBuy.Models.Abstraction.Result.Error);
            //throw new NotImplementedException();
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            var login = result as LoginResult;
            var token = login.AccessToken;
            var request = GraphRequest.NewMeRequest(token, null);
            Bundle parameters = new Bundle();
            parameters.PutString("fields", "id,name,gender,locale,birthday, location,email, picture.width(180),installed");
            request.Parameters = (parameters);
            var userInfo = Task.Run(() => (request.ExecuteAndWait().JSONObject.ToString())).Result;
            var definition = new { Id = 0L, Name = "", Gender = "", Locale = "", Birthday = "", Location = new { Name = "" }, Email = "", Picture = new { Data = new { Url = "" } } };
            var userConverted = JsonConvert.DeserializeAnonymousType(userInfo, definition);
            //me/friends? para pegar os amigos
            var user = new User()
            {
                FacebookId = token.UserId,
                FullName = userConverted.Name,
                Birthday = DateTime.ParseExact(userConverted.Birthday, "MM/dd/yyyy", CultureInfo.InvariantCulture),
                Email = userConverted.Email,
                Male = userConverted.Gender == "male",
                Avatar = userConverted.Picture.Data.Url,
                Locale = userConverted.Locale,
                Location = userConverted.Location.Name
            };
            using (var scope = App.Container.BeginLifetimeScope())
                scope.Resolve<IAuthentication>().LoginResult = new LoginResultContent(user) { Token = token.Token };
        }
        protected override void OnActivityResult(int requestCode, Android.App.Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
            SetResult(resultCode);
            Finish();
        }

    }
}