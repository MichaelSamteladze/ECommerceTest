using Core;
using Core.Enums;
using Core.Properties;
using Core.Services;
using ECommerceTest.Controllers;
using Reusables.Core;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace ECommerceTest.Models
{
    public class AuthModel
    {
        #region Sub Classes
        public class PasswordRecoveryModel
        {
            #region Methods
            public static PageViewModel GetPasswordRecoveryPageViewModel(UrlHelper Url, PageViewModel Model = null)
            {
                if(Model == null)
                {
                    Model = new PageViewModel();
                }

                Model.PageTitle = Resources.TextResetPassword;
                Model.SignInUrl = Url.RouteUrl(ControllerActionRouteNames.Website.Auth.SignIn);

                return Model;
            }

            public static void ValidateRecoveryForm(PageViewModel Model)
            {
                Model.Form.Errors = new List<SimpleKeyValue<string, string>>();
                Model.Form.Errors.Add(Validation.ValidateEmail(ErrorKey: $"[name={nameof(Model.Email)}]", Email: Model.Email, ValidateRequired: true, ValidateUnique: false, UserID: null));
                Model.Form.Errors.RemoveAll(Item => Item == null);
            }

            public static bool SendRecoverEmail(PageViewModel Model, Controller C)
            {
                var IsEmailSent = false;
                var U = UsersDataAccess.GetSingleUserByEmail(Model.Email);

                if (U != null)
                {
                    var Firstname = U.Firstname;
                    var RecoveryCode = (new Random().Next(100000, 999999)).ToString();

                    IsEmailSent = NotificationManager.SendPasswordRecoveryEmail(RecoveryCode, U.Firstname, U.Email);

                    SessionAssistance.SetValue(C.Session, RecoveryCode, U);
                }
                return IsEmailSent;
            }

            public static void ResetPassword(HttpSessionStateBase Session, PageViewModel Model)
            {
                var User = SessionAssistance.GetValue<User>(Session, Model.RecoveryCode?.Trim()); 
                
                if (User != null)
                {
                    var DAL = new UsersDataAccess();
                    var UserID = DAL.UsersIUD(
                        DatabaseAction: DatabaseActions.UPDATE,
                        UserID: User.UserID,
                        Password: Model.Password
                    );
                    if (!DAL.IsError && UserID != null)
                    {
                        var U = UsersDataAccess.GetSingleUserByID(UserID);
                        SessionAssistance.SetUser(Session, U);
                        Model.Form.IsSaved = true;
                    }
                }
            }

            public static void ValidatePasswordResetForm(PageViewModel Model)
            {
                Model.Form.Errors = new List<SimpleKeyValue<string, string>>();

                Model.Form.Errors.Add(Validation.ValidatePassword(ErrorKey: $"[name={nameof(Model.Password)}]", Password: Model.Password));
                Model.Form.Errors.Add(Validation.ValidatePasswordRepeat(ErrorKey: $"[name={nameof(Model.Password)}]", Password: Model.Password, PasswordRepeat: Model.PasswordRepeat));
                Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.RecoveryCode)}]", ValueToValidate: Model.RecoveryCode));

                Model.Form.Errors.RemoveAll(Item => Item == null);
            }
            #endregion

            #region Properties
            public class PageViewModel
            {
                #region Properties
                public FormViewModelBase Form { get; set; } = new FormViewModelBase();
                public string PageTitle { get; set; }
                public string Email { get; set; }
                public string Password { get; set; }
                public string RecoveryCode { get; set; }
                public string ResetPasswordUrl { get; set; }
                public string PasswordRepeat { get; set; }
                public string SignInUrl { get; set; }
                public string TextRecoveryCodeNotValid { get; set; } = Resources.ValidateRecoveryCodeNotValid;
                #endregion
            }
            #endregion
        }

        public class SignInModel
        {
            #region Methods
            public static PageViewModel GetSingInPageViewModel(UrlHelper Url, ViewDataDictionary ViewData, PageViewModel Model = null)
            {
                if (Model == null)
                {
                    Model = new PageViewModel();
                }
                Model.SignUpUrl = Url.RouteUrl(ControllerActionRouteNames.Website.Auth.SignUp);
                Model.ForgotPasswordUrl = Url.RouteUrl(ControllerActionRouteNames.Website.Auth.ForgotPassword);
                
                return Model;
            }

            public static bool SignIn(HttpSessionStateBase Session, string Email, string Password)
            {
                var LogedIn = false;
                var User = UsersDataAccess.GetSingleUserByEmailAndPassword(Email, Password);
                if (User != null && User.IsActive)
                {
                    SessionAssistance.SetUser(Session, User);
                    LogedIn = true;
                }
                return LogedIn;
            }                                    
            #endregion

            #region Properties
            public class PageViewModel : User
            {
                #region Properties                
                public string SignUpUrl { get;set;}
                public string ForgotPasswordUrl { get; set; }
                public bool HasError { get; set; }
                public string TextInvalidUsernameOrPassword { get; set; } = Resources.ValidationUserInvalidUsernameOrPassword;
                #endregion
            }
            #endregion
        }

        public class SignUpModel
        {
            #region Methods
            public static PageViewModel GetSignUpPageViewModel(PageViewModel Model = null)
            {
                if (Model == null)
                {
                    Model = new PageViewModel();
                }

                Model.PageTitle = Resources.TextSignUp;
                                                
                return Model;
            }

            public static bool SaveUserToDB(PageViewModel Model, WebsiteController C)
            {         
                var DAL = new UsersDataAccess();
                var UserID = DAL.UsersIUD(
                    DatabaseAction: DatabaseActions.CREATE,
                    Email: Model.Email,
                    Password: Model.Password,
                    Firstname: Model.Firstname,
                    Lastname: Model.Lastname,
                    IsActive: true
                );

                var User = UsersDataAccess.GetSingleUserByID(UserID);
                if (User != null)
                {
                    SessionAssistance.SetUser(C.Session, User);
                }

                return !DAL.IsError;
            }

            public static void ValidateSignUpForm(PageViewModel Model)
            {
                Model.Form.Errors = new List<SimpleKeyValue<string, string>>();
                
                Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.Firstname)}]", ValueToValidate: Model.Firstname));
                Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.Lastname)}]", ValueToValidate: Model.Lastname));
                Model.Form.Errors.Add(Validation.ValidateEmail(ErrorKey: $"[name={nameof(Model.Email)}]", Email: Model.Email, ValidateRequired: true, ValidateUnique: true, UserID: null));
                Model.Form.Errors.Add(Validation.ValidatePassword(ErrorKey: $"[name={nameof(Model.Password)}]", Password: Model.Password));                
                
                Model.Form.Errors.RemoveAll(Item => Item == null);
            }
            #endregion

            #region Sub Classes
            public class PageViewModel
            {
                #region Properties
                public string PageTitle { get; set; }
                public string Email { get; set; }
                public string Password { get; set; }
                public string Firstname { get; set; }
                public string Lastname { get; set; }                
                public FormViewModelBase Form { get; set; } = new FormViewModelBase();                
                public string TextError { get; set; } = Resources.TextError;
                public string TextSuccess { get; set; } = Resources.TextSuccess;
                #endregion
            }
            #endregion
        }
        #endregion
    }
}