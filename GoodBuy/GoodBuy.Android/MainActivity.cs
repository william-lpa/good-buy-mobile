using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Gcm.Client;
using goodBuy.Droid;
using GoodBuy;

[assembly: Xamarin.Forms.Dependency(typeof(MainActivity))]
namespace goodBuy.Droid
{
    [Activity(Label = "GoodBuy", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        static MainActivity instance = null;        

        // Return the current activity instance.
        public static MainActivity CurrentActivity
        {
            get
            {
                return instance;
            }

        }
        protected async override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            await new SocialAuthentication().LoginAzureAsync();
        }

        protected override void OnCreate(Bundle bundle)
        {
            Xamarin.Facebook.FacebookSdk.SdkInitialize(this);
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            SQLitePCL.Batteries.Init();

            LoadApplication(new App());
            instance = this;

            //PackageInfo info = this.PackageManager.GetPackageInfo("goodbuy.app", PackageInfoFlags.Signatures);
            //foreach (var item in info.Signatures)
            //{
            //    Java.Security.MessageDigest md = Java.Security.MessageDigest.GetInstance("SHA");
            //    md.Update(item.ToByteArray());

            //    string keyHash = Convert.ToBase64String(md.Digest());
            //}
            try
            {
                // Check to ensure everything's set up right
                GcmClient.CheckDevice(this);
                GcmClient.CheckManifest(this);
                // Register for push notifications
            }
            catch (Java.Net.MalformedURLException)
            {
                CreateAndShowDialog("There was an error creating the client. Verify the URL.", "Error");
            }
            catch (Java.Lang.Exception e)
            {
                CreateAndShowDialog(e.Message, "Error");
            }
            catch (System.Exception e)
            {
                CreateAndShowDialog(e.Message, "Error");
            }
        }

        private void CreateAndShowDialog(string message, string title)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetMessage(message);
            builder.SetTitle(title);
            builder.Create().Show();
        }



    }
}

