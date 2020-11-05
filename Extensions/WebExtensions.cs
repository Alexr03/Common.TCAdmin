using System.Web;
using TCAdmin.SDK.Objects;

namespace Alexr03.Common.TCAdmin.Extensions
{
    public static class WebExtensions
    {
        public static bool SetCurrentUser(User user)
        {
            return user.Find() && SetCurrentUser(user.UserId);
        }
        
        private static bool SetCurrentUser(int userId)
        {
            HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(
                new System.Security.Principal.GenericIdentity(userId.ToString()), new string[] { });

            return true;
        }
    }
}