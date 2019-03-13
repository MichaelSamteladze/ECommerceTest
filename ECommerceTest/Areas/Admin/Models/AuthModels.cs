using Core;
using Core.Properties;
using Reusables.Core;
using System.Web;
using System.Web.Mvc;

namespace ECommerceTest.Areas.Admin.Models
{
    public class LoginModel
    {
        #region Methods
        public static bool IsUserLoggedIn(HttpSessionStateBase Session)
        {
            return SessionAssistance.GetUser(Session) != null;
        }

        public static void AuthenticateUser(UrlHelper Url, HttpSessionStateBase Session, PageViewModel Model)
        {
            var User = UsersDataAccess.GetSingleUserByEmailAndPassword(Email: Model.Username, Password: Model.Password);
            if (User == null || !User.IsActive || !User.IsAdmin)
            {
                Model.IsLoginFailed = true;
            }
            else
            {
                SessionAssistance.SetUser(Session, User);
            }
        }

        public static void ReloginUser(HttpSessionStateBase Session)
        {
            var User = SessionAssistance.GetUser(Session);
            User = UsersDataAccess.GetSingleUserByID(UserID: User?.UserID);
            if (User != null && User.IsActive)
            {
                SessionAssistance.SetUser(Session, User);
            }
        }
        #endregion

        #region Sub Classes
        public class PageViewModel
        {
            #region Properties
            public string Username { get; set; }
            public string Password { get; set; }            
            public string ErrorMessage { get; set; } = Resources.ValidationUserInvalidUsernameOrPassword;
            public bool IsLoginFailed { get; set; }
            #endregion
        } 
        #endregion
    }
}