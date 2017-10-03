using GoodBuy.Models.Abstraction;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace GoodBuy.Authentication
{
    public interface IAuthentication
    {
        Task<(MobileServiceUser azureUser, LoginResultContent appUser)> LoginClientFlowAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider);
        Task<MobileServiceUser> LoginAzureAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider);
        void RegisterForPushNotificaton(MobileServiceClient client);
        void LogOut();
        LoginResultContent LoginResult { get; set; }
        bool SignIn { get; set; }        
    }
}
