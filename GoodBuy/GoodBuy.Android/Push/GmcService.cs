using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Content;
using Gcm.Client;
using Microsoft.WindowsAzure.MobileServices;
using System.Diagnostics;
using goodBuy.Droid.Push;
using System.Net.Http;
using GoodBuy.Service;
using GoodBuy;
using Autofac;
using System.Linq;

[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]
//GET_ACCOUNTS is only needed for android versions 4.0.3 and below
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
namespace goodBuy.Droid
{
    [BroadcastReceiver(Permission = Gcm.Client.Constants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "@PACKAGE_NAME@" })]
    public class PushHandlerBroadcastReceiver : GcmBroadcastReceiverBase<GcmService>
    {
        public static string[] SENDER_IDS = new string[] { "748291332149" };
    }
    [Service]
    public class GcmService : GcmServiceBase
    {
        public static MobileServiceClient Client { get; set; }
        public static string RegistrationID { get; private set; }
        public GcmService() : base(PushHandlerBroadcastReceiver.SENDER_IDS)
        { }

        protected override void OnRegistered(Context context, string registrationId)
        {
            Android.Util.Log.Verbose("PushHandlerBroadcastReceiver", "GCM Registered: " + registrationId);
            RegistrationID = registrationId;
            var push = Client.GetPush();
            MainActivity.CurrentActivity.RunOnUiThread(() => RegisterAsync(push, null));
        }
        public async void RegisterAsync(Microsoft.WindowsAzure.MobileServices.Push push, IEnumerable<string> tags)
        {
            try
            {
                var installation = new DeviceInstallation
                {
                    InstallationId = Client.InstallationId,
                    Platform = "gcm",
                    PushChannel = RegistrationID
                };


                using (var scope = App.Container?.BeginLifetimeScope())
                {
                    (await scope?.Resolve<GrupoOfertaService>()?.CarregarGrupoDeOfertasUsuarioLogadoAsync()).Select(x => { installation.Tags.Add(x.Id); return x; }).ToArray();
                    installation.Tags.Add("user:" + scope?.Resolve<AzureService>()?.CurrentUser.User.Id);
                }

                // Set up templates to request
                PushTemplate genericTemplate = new PushTemplate
                {
                    Body = @"{""data"":{""key"":""$(keyParam)"",""message"":""$(messageParam)""}}"
                };
                PushTemplate ofertaTemplate = new PushTemplate
                {
                    Body = @"{""data"":{""key"":""$(keyParam)"",""ofertaTitle"":""$(ofertaTitleParam)"",""ofertaDescription"":""$(ofertaDescriptionParam)"",""idOFerta"":""$(ofertaIdParam)""}}"
                };

                installation.Templates.Add("genericTemplate", genericTemplate);
                installation.Templates.Add("ofertaTemplate", ofertaTemplate);

                // Register with NH
                await Client.InvokeApiAsync<DeviceInstallation, DeviceInstallation>(
                    $"/push/installations/{Client.InstallationId}",
                    installation,
                    HttpMethod.Put,
                    new Dictionary<string, string>());
                await Client.InvokeApiAsync<string[], object>("refreshPushRegistration", installation.Tags.ToArray());

                //await push. RegisterAsync(RegistrationID, templates);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                Debugger.Break();
            }
        }
        enum MessageKind
        {
            GroupAdded = 700,
            GroupEdited = 701,
            GroupExcluded = 702,
            NewMember = 703,
            ExcludedMember = 704,
            SharedMember = 705,
            SharedGroup = 706,
            AlertCreated = 707,
            AlertUpdated = 708,
            AlertedDeleted = 709,
            PriceReached = 710,
        }
        protected override void OnMessage(Context context, Intent intent)
        {
            var msg = new StringBuilder();
            if (intent != null && intent.Extras != null)
            {
                foreach (var key in intent.Extras.KeySet())
                    msg.AppendLine(key + "=" + intent.Extras.Get(key).ToString());
            }
            //Store the message
            var prefs = GetSharedPreferences(context.PackageName, FileCreationMode.Private);
            var edit = prefs.Edit();
            edit.PutString("last_msg", msg.ToString());
            edit.Commit();

            var title = string.Empty;
            var description = string.Empty;
            var idOferta = string.Empty;

            switch (Enum.Parse(typeof(MessageKind), intent.Extras.GetString("key")))
            {
                case MessageKind.GroupAdded:
                case MessageKind.GroupEdited:
                case MessageKind.GroupExcluded:
                case MessageKind.NewMember:
                case MessageKind.ExcludedMember:
                    title = "Alteração nos grupos";
                    description = intent.Extras.GetString("message");
                    CreateNotification(title, description, "grupos");
                    break;
                case MessageKind.SharedMember:
                case MessageKind.SharedGroup:
                case MessageKind.PriceReached:
                    title = intent.Extras.GetString("ofertaTitle");
                    description = intent.Extras.GetString("ofertaDescription");
                    idOferta = intent.Extras.GetString("idOferta");
                    CreateNotification(title, description, idOferta);
                    break;
                case MessageKind.AlertCreated:
                case MessageKind.AlertedDeleted:
                case MessageKind.AlertUpdated:
                    title = "Alteração nos alertas de ofertas";
                    description = intent.Extras.GetString("message");
                    CreateAlertNotification(title, description);
                    break;
                default:
                    CreateNotification("Unknown message details", msg.ToString());
                    break;
            }
        }

        private void CreateAlertNotification(string title, string desc)
        {
            var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;
            //Create an intent to show ui

            var builder = new Notification.Builder(this)
              .SetContentTitle(title)
              .SetContentText(desc)
              .SetSmallIcon(Android.Resource.Drawable.IcMenuShare);

            Notification notification = new Notification.BigTextStyle(builder)
              .BigText(desc)
              .Build();
            // Put the auto cancel notification flag
            notification.Flags |= NotificationFlags.AutoCancel;
            notificationManager.Notify(0, notification);
        }

        private void CreateNotification(string title, string desc, string parameter = null)
        {
            //Create notification
            var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;
            //Create an intent to show ui
            var startupIntent = new Intent(this, typeof(MainActivity));
            startupIntent.PutExtra("param", parameter);

            var builder = new Notification.Builder(this)
              .SetContentTitle(title)
              .SetContentText(parameter == "grupos" ? "Grupo com alteração" : "Oferta compartilhada")
              .SetSmallIcon(Android.Resource.Drawable.IcMenuShare)
              .SetContentIntent(PendingIntent.GetActivity(this, 0, startupIntent, PendingIntentFlags.UpdateCurrent))
              .AddAction(Android.Resource.Drawable.IcMenuSend, parameter == "grupos" ? "Ver grupos" : "Ir para a oferta", PendingIntent.GetActivity(this, 0, startupIntent, PendingIntentFlags.UpdateCurrent));

            Notification notification = new Notification.BigTextStyle(builder)
              .BigText(desc)
              .Build();
            // Put the auto cancel notification flag
            notification.Flags |= NotificationFlags.AutoCancel;
            notificationManager.Notify(0, notification);
        }

        protected override void OnUnRegistered(Context context, string registrationId)
        {
            Android.Util.Log.Error("PushHandlerBroadcastReceiver", "Unregistered RegisterationId : " + registrationId);
        }
        protected override void OnError(Context context, string errorId)
        {
            Android.Util.Log.Error("PushHandlerBroadcastReceiver", "GCM Error: " + errorId);
        }

    }
}