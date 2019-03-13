using Core;
using Core.Utilities;
using System.Web;

namespace Reusables.Core
{
    public class SessionAssistance
    {
        #region Methods
        public static void SetValue(HttpSessionStateBase Session, string Key, object Value)
        {
            Session[Key] = Value;
        }

        public static T GetValue<T>(HttpSessionStateBase Session, string Key)
        {
            return (T)Session[Key];
        } 

        public static User GetUser(HttpSessionStateBase Session)
        {
            return Session[Constants.Session.User.UserItem] as User;
        }

        public static void SetUser(HttpSessionStateBase Session, User User)
        {
            SetValue(Session, Constants.Session.User.UserItem, User);
        }
        #endregion
    }
}