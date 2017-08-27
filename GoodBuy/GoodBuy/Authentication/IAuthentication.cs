using GoodBuy.Models.Abstraction;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace GoodBuy.Authentication
{
    public interface IAuthentication
    {
        void LoginClientFlow(MobileServiceClient client, MobileServiceAuthenticationProvider provider);
        LoginResultContent LoginResult { get; }
        MobileServiceUser RetrieveTokenFromSecureStore();
        void StoreTokenInSecureStore(MobileServiceUser user);
        void RemoveTokenFromSecureStore();
    }
}
