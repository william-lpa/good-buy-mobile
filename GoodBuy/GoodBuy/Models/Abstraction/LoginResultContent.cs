using Android.App;

namespace GoodBuy.Models.Abstraction
{
    public class LoginResultContent
    {
        public User User { get; set; }
        public string Message { get; set; }
        public Result Result { get; set; }
        public string Token { get; set; }
        public LoginResultContent(User user, string message = "", Result result = Result.Ok)
        {
            User = user;
            Message = message;
            Result = result;
        }
    }
}
