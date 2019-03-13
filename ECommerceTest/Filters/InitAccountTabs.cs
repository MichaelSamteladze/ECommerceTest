using Core.Properties;
using Core.Utilities;
using ECommerceTest.Controllers;
using ECommerceTest.Models;
using Reusables.Core;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ECommerceTest.Filters
{
    public class InitAccountTabs : FilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuted(ActionExecutedContext FilterContext)
        {

        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext FilterContext)
        {
            var Controller = FilterContext.Controller as WebsiteController;
            var ActionName = FilterContext.ActionDescriptor.ActionName;
            var Model = new AccountLayoutModel
            {
                CurrentUrl = Controller.LayoutModel.CurrentUrl
            };

            InitTabs(Controller.Url, Model, ActionName);

            LocalUtilities.SetLayoutViewModel(ViewData: Controller.ViewData, Model: Model, Key: Constants.ViewData.TabsViewModel);
        }

        void InitTabs(UrlHelper Url, AccountLayoutModel Model, string ActionName)
        {
            Model.Tabs = new List<ProjectMenuItem>
            {
                new ProjectMenuItem
                {
                    Caption = Resources.TextProfile,
                    NavigateUrl = Url.RouteUrl(ControllerActionRouteNames.Website.Account.Profile),
                    IsSelected = ActionName == nameof(AccountController.Profile)
                },                
                new ProjectMenuItem
                {
                    Caption = Resources.TextChangePassword,
                    NavigateUrl = Url.RouteUrl(ControllerActionRouteNames.Website.Account.ChangePassword),
                    IsSelected = ActionName == nameof(AccountController.ProfileChangePassword)
                },
                new ProjectMenuItem
                {
                    Caption = Resources.TextOrders,
                    NavigateUrl = Url.RouteUrl(ControllerActionRouteNames.Website.Account.OrderHistory),
                    IsSelected = ActionName == nameof(AccountController.OrderHistory)
                }
            };
        }
    }
}