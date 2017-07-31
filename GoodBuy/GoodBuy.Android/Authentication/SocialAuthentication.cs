using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoodBuy.Authentication;
using Microsoft.WindowsAzure.MobileServices;
using GoodBuy.Droid;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(SocialAuthentication))]
namespace GoodBuy.Droid
{
    public class SocialAuthentication : IAuthentication
    {
        public async Task<MobileServiceUser> LoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            try
            {
                return await client.LoginAsync(Forms.Context, provider);
            }
            catch (Exception err)
            {
                return null;
            }
        }
    }
}