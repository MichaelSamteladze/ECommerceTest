using Core.Utilities;
using ECommerceTest.Areas.Admin.Controllers;
using ECommerceTest.Areas.Admin.Models;
using Reusables.Core;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace ECommerceTest.Areas.Admin.FilterAttributes
{
    public class BeforeAdminPageLoad : FilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuted(ActionExecutedContext FilterContext)
        {

        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext FilterContext)
        {
            var Controller = FilterContext.Controller as AdminController;
            var Model = new AdminLayoutModel
            {
                UrlCurrent = $"{Controller.Request.RawUrl.Split('?')[0].TrimEnd('/')}/"                
            };

            var IsAuthorized = AdminAuthorize(FilterContext, Controller, Model);
            if (IsAuthorized)
            {
                InitMenu(Controller.Url, Model);
                InitBreadCrumbs(Controller, Model);
                InitPageTitle(Model);

                SuccessErrorPartialViewAssistance.SetSuccessErrorMessageInLayoutModel(Controller.HttpContext.Session, Model);

                LocalUtilities.SetLayoutViewModel(ViewData: Controller.ViewData, Model: Model, Key: Constants.ViewData.LayoutViewModel);
            }
        }

        bool AdminAuthorize(ActionExecutingContext FilterContext, AdminController Controller, AdminLayoutModel Model)
        {
            var IsAuthorized = false;

            var UserItem = SessionAssistance.GetUser(FilterContext.HttpContext.Session);

            if (UserItem == null || !UserItem.IsAdmin)
            {
                FilterContext.Result = new RedirectResult(Controller.Url.RouteUrl(ControllerActionRouteNames.Admin.Auth.Logout));
            }            
            else
            {
                IsAuthorized = true;
                Model.UserItem =
                Controller.User = UserItem;
            }

            return IsAuthorized;
        }        

        void InitMenu(UrlHelper Url, AdminLayoutModel Model)
        {
            if (Model.UserItem != null && Model.UserItem.Permissions?.Count > 0)
            {                
                Model.Menu = Model.UserItem.Permissions
                .Where(Item => Item.IsMenuItem && Item.ParentID == null)
                .Select(Item => new ProjectMenuItem
                {
                    Caption = Item.Caption,
                    NavigateUrl = string.IsNullOrWhiteSpace(Item.PagePath) ? Item.PermissionCode : Item.PagePath,
                    Icon = Item.MenuIcon,
                    IsSelected = Item.PagePath == Model.UrlCurrent,
                    Children = Model.UserItem.Permissions.Where(SubItem => SubItem.IsMenuItem && SubItem.ParentID == Item.PermissionID).Select(SubItem => new ProjectMenuItem
                    {
                        Caption = SubItem.Caption,
                        NavigateUrl = SubItem.PagePath,
                        Icon = SubItem.MenuIcon,
                        IsSelected = SubItem.PagePath == Model.UrlCurrent
                    }).ToList()
                }).ToList();

                Model.Menu.ForEach(Item =>
                {
                    if(Item.HasChildren)
                    {
                        Item.IsSelected = Item.Children.Any(SubItem => SubItem.IsSelected);
                    }
                });
            }

            Model.UrlLogout = Url.RouteUrl(ControllerActionRouteNames.Admin.Auth.Logout);
            Model.UrlWebsite = Url.RouteUrl(ControllerActionRouteNames.Website.Home.Index);
        }

        void InitBreadCrumbs(AdminController Controller, AdminLayoutModel Model)
        {
            Model.Breadcrumbs = Breadcrumbs.GetBreadcrumbsByPageUrl(Model.UserItem.Permissions, Model.UrlCurrent);            
        }

        void InitPageTitle(AdminLayoutModel Model)
        {
            var PageTitle = Model.UserItem.Permissions.Where(Item => Item.PagePath == Model.UrlCurrent || (!string.IsNullOrWhiteSpace(Item.PagePath) && Regex.IsMatch(Model.UrlCurrent, $"{Item.PagePath}+$"))).LastOrDefault()?.Caption;
            LocalUtilities.SetPageTitle(Model, PageTitle);
        }
    }
}