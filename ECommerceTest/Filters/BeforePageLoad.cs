using Core;
using Core.Enums;
using Core.Properties;
using Core.Utilities;
using ECommerceTest.Controllers;
using ECommerceTest.Models;
using Reusables.Core;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ECommerceTest.Filters
{
    public class BeforePageLoad : FilterAttribute, IActionFilter
    {

        void IActionFilter.OnActionExecuted(ActionExecutedContext FilterContext)
        {

        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext FilterContext)
        {
            var Controller = FilterContext.Controller as WebsiteController;
            var ActionName = FilterContext.ActionDescriptor.ActionName;
            var SettingsList = DictionariesDataAccess.ListDictionaries(Level: 1, DictionaryCode: Dictionaries.Codenames.Settings.DictionaryCode);
            var CompanyEmail = SettingsList?.Where(Item => Item.IntCode == Settings.CONTACT_EMAIL)?.FirstOrDefault().StringCode;
            var CompanyPhoneNumber = SettingsList?.Where(Item => Item.IntCode == Settings.CONTACT_PHONE)?.FirstOrDefault().StringCode;

            var Model = new WebsiteLayoutModel();
            Model.WebsiteDomain = LocalUtilities.GetWebsiteDomain(Controller.Request);
            Model.CurrentUrl = $"{Model.WebsiteDomain}{FilterContext.RequestContext.HttpContext.Request.RawUrl.Split('?')[0].TrimEnd('/')}/";
            Model.SignInUrl = Controller.Url.RouteUrl(ControllerActionRouteNames.Website.Auth.SignIn, new { ReturnUrl = Model.CurrentUrl });
            Model.SignUpUrl = Controller.Url.RouteUrl(ControllerActionRouteNames.Website.Auth.SignUp);
            Model.BasketUrl = Controller.Url.RouteUrl(ControllerActionRouteNames.Website.Basket.Index);            
            Model.ProductsCountInBasket = 0;
            Model.CompanyEmail = CompanyEmail;
            Model.CompanyPhoneNumber = CompanyPhoneNumber;                        
            
            InitMenu(Model, Controller.Url, ActionName);
            UserAuthorize(FilterContext, Controller, Model);
            if (Controller.IsUserLoggedIn)
            {
                InitAccountMenu(Controller, Model, ActionName);
            }

            SuccessErrorPartialViewAssistance.SetSuccessErrorMessageInLayoutModel(Controller.HttpContext.Session, Model);

            Controller.LayoutModel = Model;
            LocalUtilities.SetLayoutViewModel(ViewData: Controller.ViewData, Model: Model, Key: Constants.ViewData.LayoutViewModel);
        }

        void InitMenu(WebsiteLayoutModel Model, UrlHelper Url, string ActionName)
        {
            Model.Menu = new List<ProjectMenuItem>
            {
                new ProjectMenuItem
                {
                    Caption = Resources.TextAboutUs,
                    NavigateUrl = $"{Model.WebsiteDomain}{Url.RouteUrl(ControllerActionRouteNames.Website.Home.About)}",
                    IsSelected = ActionName == nameof(HomeController.About)
                },
                new ProjectMenuItem
                {
                    Caption = Resources.TextProducts,
                    NavigateUrl = $"{Model.WebsiteDomain}{Url.RouteUrl(ControllerActionRouteNames.Website.Products.Page)}",
                    IsSelected = ActionName == nameof(ProductsController.Products)
                },
                new ProjectMenuItem
                {
                    Caption = Resources.TextFAQ,
                    NavigateUrl = $"{Model.WebsiteDomain}{Url.RouteUrl(ControllerActionRouteNames.Website.Home.FAQ)}",
                    IsSelected = ActionName == nameof(HomeController.FAQ)
                },
                new ProjectMenuItem
                {
                    Caption = Resources.TextContact,
                    NavigateUrl = $"{Model.WebsiteDomain}{Url.RouteUrl(ControllerActionRouteNames.Website.Home.Contact)}",
                    IsSelected = ActionName == nameof(HomeController.Contact)
                }
            };            
        }

        void InitAccountMenu(WebsiteController C, WebsiteLayoutModel Model, string ActionName)
        {
            Model.AccountMenu = new List<ProjectMenuItem>
            {
                new ProjectMenuItem
                {
                    Caption = Resources.TextProfile,
                    NavigateUrl = C.Url.RouteUrl(ControllerActionRouteNames.Website.Account.Profile),
                    IsSelected = ActionName == nameof(AccountController.Profile)
                },
                new ProjectMenuItem
                {
                    Caption = Resources.TextChangePassword,
                    NavigateUrl = C.Url.RouteUrl(ControllerActionRouteNames.Website.Account.ChangePassword),
                    IsSelected = ActionName == nameof(AccountController.ProfileChangePassword)
                },
                new ProjectMenuItem
                {
                    Caption = Resources.TextOrders,
                    NavigateUrl = C.Url.RouteUrl(ControllerActionRouteNames.Website.Account.OrderHistory),
                    IsSelected = ActionName == nameof(AccountController.OrderHistory)
                },
                new ProjectMenuItem
                {
                    Caption = Resources.TextSignOut,
                    NavigateUrl = C.Url.RouteUrl(ControllerActionRouteNames.Website.Auth.SignOut)
                }
            };
            if (C.User.IsAdmin)
            {
                Model.AccountMenu.Insert(0, new ProjectMenuItem
                {
                    Caption = Resources.TextAdministration,
                    NavigateUrl = C.Url.RouteUrl(ControllerActionRouteNames.Admin.Auth.Login)
                });
            }
        }

        bool UserAuthorize(ActionExecutingContext FilterContext, WebsiteController Controller, WebsiteLayoutModel Model)
        {
            var IsUserAuthorized = false;
            var User = SessionAssistance.GetUser(FilterContext.HttpContext.Session);
            if (User != null)
            {
                IsUserAuthorized = true;
                Model.UserItem =
                Controller.User = User;
                Model.ProductsCountInBasket = User.ProductCountInBasket;
            }

            return IsUserAuthorized;
        }
    }
}


