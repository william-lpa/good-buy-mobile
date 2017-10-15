using System;
using Android.Content;
using GoodBuy.Models;
using GoodBuy.Service;
using Android.Provider;
using GoodBuy.Log;
using Android.Telephony;
using Xamarin.Forms;

namespace goodBuy.Droid.ContactList
{
    public class ContactList : IContactListService
    {
        private Action<User> userCallback;

        public string TryGetPhoneNumber
        {
            get
            {
                try
                {
                    var manager = (TelephonyManager)Forms.Context.GetSystemService(Context.TelephonyService);
                    string numberManager = manager.Line1Number;
                    if (!string.IsNullOrEmpty(numberManager))
                        return numberManager;

                    string[] PROJECTION =
                        {
                                ContactsContract.Contacts.InterfaceConsts.Id,
                                ContactsContract.CommonDataKinds.Phone.Number,
                                ContactsContract.Contacts.Data.InterfaceConsts.Mimetype
                };
                    ContentResolver content = MainActivity.CurrentActivity.ContentResolver;
                    var cursor = content.Query(
                            // Retrieves data rows for the device user's 'profile' contact
                            Android.Net.Uri.WithAppendedPath(
                                                 ContactsContract.Profile.ContentUri,
                                                 ContactsContract.Contacts.Data.ContentDirectory),
                                                 PROJECTION,
                                                 ContactsContract.Contacts.Data.InterfaceConsts.Mimetype + "= ?",
                                                 new string[] { ContactsContract.CommonDataKinds.Phone.ContentItemType, },
                                                  null);

                    if (cursor.MoveToFirst())
                    {
                        do
                        {
                            string mime_type = cursor.GetString(cursor.GetColumnIndex(ContactsContract.Contacts.Data.InterfaceConsts.Mimetype));
                            if (mime_type == (ContactsContract.CommonDataKinds.Phone.ContentItemType))
                                return cursor.GetString(1);

                        } while (cursor.MoveToNext());

                    }
                }
                catch (Exception err)
                {
                    Log.Instance.AddLog(err);
                }
                return "";
            }
        }

        public void PickContactList(Action<User> callback)
        {
            userCallback = callback;
            Intent intent = new Intent(Intent.ActionPick, ContactsContract.Contacts.ContentUri);
            MainActivity.CurrentActivity.StartActivityForResult(intent, (int)ActivitiesRequestCode.RequestCodes.PICK_CONTACT);
        }

        public User PickProfileUser()
        {
            var user = new User();
            try
            {
                var accounts = Android.Accounts.AccountManager.Get(MainActivity.CurrentActivity).GetAccountsByType("com.google");
                var manager = (TelephonyManager)Forms.Context.GetSystemService(Context.TelephonyService);

                user.Email = accounts.Length > 0 ? accounts[0].Name : null;
                user.Id = manager.Line1Number;

                String[] PROJECTION = {
                                ContactsContract.CommonDataKinds.Email.Address,
                                ContactsContract.Contacts.InterfaceConsts.Id,
                                ContactsContract.CommonDataKinds.StructuredName.FamilyName,
                                ContactsContract.CommonDataKinds.StructuredName.DisplayName,
                                ContactsContract.CommonDataKinds.Phone.Number,
                                ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Data15,
                                ContactsContract.CommonDataKinds.Photo.InterfaceConsts.PhotoUri,
                                ContactsContract.Contacts.Data.InterfaceConsts.Mimetype,
        };
                ContentResolver content = MainActivity.CurrentActivity.ContentResolver;
                var cursor = content.Query(
                        Android.Net.Uri.WithAppendedPath(
                                             ContactsContract.Profile.ContentUri,
                                             ContactsContract.Contacts.Data.ContentDirectory),
                                             PROJECTION,
                                             ContactsContract.Contacts.Data.InterfaceConsts.Mimetype + "=? OR "
                                             + ContactsContract.Contacts.Data.InterfaceConsts.Mimetype + "=? OR "
                                             + ContactsContract.Contacts.Data.InterfaceConsts.Mimetype + "=? OR "
                                             + ContactsContract.Contacts.Data.InterfaceConsts.Mimetype + "=?",
                                             new String[]{
                                             ContactsContract.CommonDataKinds.Email.ContentItemType,
                                             ContactsContract.CommonDataKinds.StructuredName.ContentItemType,
                                             ContactsContract.CommonDataKinds.Phone.ContentItemType,
                                             ContactsContract.CommonDataKinds.Photo.ContentItemType
                                             }, null);
                if (cursor.MoveToFirst())
                {
                    do
                    {
                        string mime_type = cursor.GetString(cursor.GetColumnIndex(ContactsContract.Contacts.Data.InterfaceConsts.Mimetype));
                        if (string.IsNullOrEmpty(user.Email) && mime_type == (ContactsContract.CommonDataKinds.Email.ContentItemType))
                            user.Email = cursor.GetString(0);
                        else if (mime_type == (ContactsContract.CommonDataKinds.StructuredName.ContentItemType))
                            user.FullName = cursor.GetString(3);
                        else if (string.IsNullOrEmpty(user.Id) && mime_type == (ContactsContract.CommonDataKinds.Phone.ContentItemType))
                            user.Id = cursor.GetString(4);
                        else if (mime_type == (ContactsContract.CommonDataKinds.Photo.ContentItemType))
                        {
                            user.Avatar = Convert.ToBase64String(cursor.GetBlob(cursor.GetColumnIndex("data15")));
                        }

                    } while (cursor.MoveToNext());
                }
                return user;
            }
            catch (Exception err)
            {
                Log.Instance.AddLog(err);
                return null;
            }
        }

        public void ResolveContact(object contactResource)
        {
            try
            {
                User user = new User();
                if (contactResource is Android.Net.Uri contactData)
                {
                    string[] PROJECTION =
                    {
                        ContactsContract.Contacts.InterfaceConsts.Id,
                        ContactsContract.Contacts.InterfaceConsts.DisplayNamePrimary,
                        ContactsContract.Contacts.InterfaceConsts.HasPhoneNumber
                    };

                    ContentResolver cr = MainActivity.CurrentActivity.ContentResolver;
                    Android.Database.ICursor cursor = cr.Query(contactData, PROJECTION, null, null, null);
                    if (cursor != null && cursor.MoveToFirst())
                    {
                        // get the contact's information
                        string id = cursor.GetString(cursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.Id));
                        user.FullName = cursor.GetString(cursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayNamePrimary));
                        int hasPhone = cursor.GetInt(cursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.HasPhoneNumber));

                        // get the user's email address
                        cursor = cr.Query(ContactsContract.CommonDataKinds.Email.ContentUri, null,
                                ContactsContract.CommonDataKinds.Email.InterfaceConsts.ContactId + " = ?", new String[] { id }, null);
                        if (cursor != null && cursor.MoveToFirst())
                            user.Email = cursor.GetString(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.Email.Address));

                        // get the user's phone number
                        if (hasPhone > 0)
                        {
                            cursor = cr.Query(ContactsContract.CommonDataKinds.Phone.ContentUri, null,
                                    ContactsContract.CommonDataKinds.Phone.InterfaceConsts.ContactId + " = ?", new String[] { id }, null);
                            if (cursor != null && cursor.MoveToFirst())
                                user.Id = cursor.GetString(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.Number));
                        }

                        ContentResolver resolver = cr;
                        cursor = resolver.Query(ContactsContract.Data.ContentUri, null, ContactsContract.Data.InterfaceConsts.ContactId + "=" + id + " AND " +
                            ContactsContract.Data.InterfaceConsts.Mimetype + "='" + ContactsContract.CommonDataKinds.Photo.ContentItemType + "'", null, null);

                        if (cursor != null && cursor.MoveToFirst())
                        {
                            var uri = ContentUris.WithAppendedId(ContactsContract.Contacts.ContentUri, long.Parse(id));
                            var file = (ContactsContract.Contacts.OpenContactPhotoInputStream(resolver, uri));
                            var bytes = new System.Collections.Generic.List<byte>();

                            if (file != null)
                            {
                                int b;
                                while ((b = file.ReadByte()) != -1)
                                    bytes.Add((byte)b);
                                user.Avatar = Convert.ToBase64String(bytes.ToArray());
                            }
                        }
                        cursor.Close();
                    }
                }

                userCallback?.Invoke(user);
            }
            catch (Exception e)
            {
                Log.Instance.AddLog(e);
            }
        }
    }
}