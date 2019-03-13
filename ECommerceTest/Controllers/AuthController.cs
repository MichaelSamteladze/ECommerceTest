using Core.Properties;
using ECommerceTest.Models;
using Reusables.Core;
using System.Web.Mvc;

namespace ECommerceTest.Controllers
{
    [RoutePrefix("auth")]
    public class AuthController : WebsiteController
    {        
        #region Sign In
        [Route("sign-in", Name = ControllerActionRouteNames.Website.Auth.SignIn)]
        public ActionResult SignIn()
        {
            if (IsUserLoggedIn)
            {
                return Redirect(Url.RouteUrl(ControllerActionRouteNames.Website.Account.Profile));
            }
            else
            {
                LocalUtilities.SetPageTitle<WebsiteLayoutModel>(ViewData: ViewData, PageTitle: Resources.TextSignIn);
                var Model = AuthModel.SignInModel.GetSingInPageViewModel(Url, ViewData);
                return View(ViewNames.Website.Auth.SignIn, Model);
            }
        }

        [HttpPost]
        [Route("sign-in")]
        public ActionResult SignIn(string ReturnUrl, AuthModel.SignInModel.PageViewModel Model)
        {
            LocalUtilities.SetPageTitle<WebsiteLayoutModel>(ViewData: ViewData, PageTitle: Resources.TextSignIn);
            var Result = default(ActionResult);
            AuthModel.SignInModel.GetSingInPageViewModel(Url, ViewData, Model);
            var HasReturnUrl = !string.IsNullOrWhiteSpace(ReturnUrl);
            
            var IsLoggedIn = AuthModel.SignInModel.SignIn(Session, Model.Email, Model.Password);
            if (IsLoggedIn)
            {
                Result = HasReturnUrl ? Redirect(ReturnUrl) : Redirect(Url.RouteUrl(ControllerActionRouteNames.Website.Home.Index));
            }
            else
            {                    
                Model.HasError = true;
                Result = View(ViewNames.Website.Auth.SignIn, Model);
            }

            return Result;
        }
        #endregion

        #region Sign Out
        [Route("sign-out", Name = ControllerActionRouteNames.Website.Auth.SignOut)]
        public ActionResult SignOut()
        {
            Session.Clear();
            Session.Abandon();
            return Redirect(Url.RouteUrl(ControllerActionRouteNames.Website.Home.Index));
        }
        #endregion

        #region Sign Up
        [Route("sign-up", Name = ControllerActionRouteNames.Website.Auth.SignUp)]
        public ActionResult SignUp()
        {
            if (IsUserLoggedIn)
            {
                return Redirect(Url.RouteUrl(ControllerActionRouteNames.Website.Account.Profile));
            }
            else
            {
                var Model = AuthModel.SignUpModel.GetSignUpPageViewModel();
                LocalUtilities.SetPageTitle<WebsiteLayoutModel>(ViewData, Model.PageTitle);
                return View(ViewNames.Website.Auth.SignUp, Model);
            }
        }

        [HttpPost]
        [Route("sign-up")]
        public ActionResult SignUp(AuthModel.SignUpModel.PageViewModel Model)
        {
            AuthModel.SignUpModel.GetSignUpPageViewModel(Model);
            LocalUtilities.SetPageTitle<WebsiteLayoutModel>(ViewData, Model.PageTitle);

            AuthModel.SignUpModel.ValidateSignUpForm(Model);
            if (!Model.Form.HasErrors)
            {
                Model.Form.IsSaved = AuthModel.SignUpModel.SaveUserToDB(Model, this);
                Model.Form.IsError = !Model.Form.IsSaved;
            }

            return View(ViewNames.Website.Auth.SignUp, Model);
        }
        #endregion

        #region Forgot Password
        [Route("forgot-password", Name = ControllerActionRouteNames.Website.Auth.ForgotPassword)]
        public ActionResult Recovery()
        {
            if (IsUserLoggedIn)
            {
                return Redirect(Url.RouteUrl(ControllerActionRouteNames.Website.Account.Profile));
            }
            else
            {
                var Model = AuthModel.PasswordRecoveryModel.GetPasswordRecoveryPageViewModel(Url);
                LocalUtilities.SetPageTitle<WebsiteLayoutModel>(ViewData, Model.PageTitle);
                return View(ViewNames.Website.Auth.ForgotPassword, Model);
            }
        }

        [HttpPost]
        [Route("forgot-password")]
        public ActionResult Recovery(AuthModel.PasswordRecoveryModel.PageViewModel Model)
        {
            var Result = default(ActionResult);

            AuthModel.PasswordRecoveryModel.GetPasswordRecoveryPageViewModel(Url, Model);
            LocalUtilities.SetPageTitle<WebsiteLayoutModel>(ViewData, Model.PageTitle);

            AuthModel.PasswordRecoveryModel.ValidateRecoveryForm(Model);
            if (Model.Form.HasErrors)
            {
                Result = View(ViewNames.Website.Auth.ForgotPassword, Model);
            }
            else
            {
                var IsEmailSent = AuthModel.PasswordRecoveryModel.SendRecoverEmail(Model, this);
                if (IsEmailSent)
                {
                    Result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Website.Auth.ForgotPasswordResetPassword));
                }
                else
                {
                    SuccessErrorPartialViewAssistance.InitErrorMessage<WebsiteLayoutModel>(ViewData: ViewData, Message: Resources.ValidateEmailNotRegistered);
                    Result = View(ViewNames.Website.Auth.ForgotPassword, Model);
                }
            }

            return Result;
        }

        [Route("forgot-password/reset-password", Name = ControllerActionRouteNames.Website.Auth.ForgotPasswordResetPassword)]
        public ActionResult RessetPassword()
        {
            var Model = AuthModel.PasswordRecoveryModel.GetPasswordRecoveryPageViewModel(Url);
            LocalUtilities.SetPageTitle<WebsiteLayoutModel>(ViewData, Model.PageTitle);
            return View(ViewNames.Website.Auth.ForgotPasswordResetPassword, Model);
        }

        [HttpPost]
        [Route("forgot-password/reset-password")]
        public ActionResult RessetPassword(AuthModel.PasswordRecoveryModel.PageViewModel Model)
        {
            var Result = default(ActionResult);

            AuthModel.PasswordRecoveryModel.GetPasswordRecoveryPageViewModel(Url, Model);
            AuthModel.PasswordRecoveryModel.ValidatePasswordResetForm(Model);

            LocalUtilities.SetPageTitle<WebsiteLayoutModel>(ViewData, Model.PageTitle);

            if (Model.Form.HasErrors)
            {
                Result = View(ViewNames.Website.Auth.ForgotPasswordResetPassword, Model);
            }
            else
            {
                AuthModel.PasswordRecoveryModel.ResetPassword(Session, Model);
                if (Model.Form.IsSaved)
                {
                    Result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Website.Home.Index));
                }
                else
                {
                    SuccessErrorPartialViewAssistance.InitErrorMessage<WebsiteLayoutModel>(ViewData: ViewData);
                    Result = View(ViewNames.Website.Auth.ForgotPasswordResetPassword, Model);
                }
            }

            return Result;
        }
        #endregion
    }
}