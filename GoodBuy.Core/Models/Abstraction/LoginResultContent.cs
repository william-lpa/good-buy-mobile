
using System;

namespace GoodBuy.Models.Abstraction
{
    public enum Result { OK = 1, Canceled = 2, Error = 3 }
    public class LoginResultContent
    {
        public User User { get; set; }
        public string Message { get; set; }
        public Result Result { get; set; }
        public string Token { get; set; }
        public LoginResultContent(User user, string message = "", Result result = Result.OK)
        {
            User = user;
            Message = message;
            Result = result;
        }

        internal LoginResultContent Merge(User profileUser)
        {
            if (profileUser != null)
            {

                User.Id = profileUser?.Id;
                if (string.IsNullOrEmpty(User.Email))
                    User.Email = profileUser.Email;

                if (string.IsNullOrEmpty(User.FullName))
                    User.FullName = profileUser.FullName;

                if (string.IsNullOrEmpty(User.Avatar))
                    User.Avatar = profileUser.Avatar;
            }
            return this;
        }
    }
}
