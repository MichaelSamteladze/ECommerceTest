using ECommerceTest.Areas.Admin.Controllers;
using ECommerceTest.Areas.Admin.Models;
using Core.Utilities;
using Reusables.Core;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;


namespace ECommerceTest.Areas.Admin.Filters
{
    public class TabsInitialization : FilterAttribute, IActionFilter
    {
        public string ParentRoute { get; set; }

        void IActionFilter.OnActionExecuted(ActionExecutedContext FilterContext)
        {

        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext FilterContext)
        {
            var Controller = FilterContext.Controller as AdminController;
            var LayoutModel = LocalUtilities.GetLayoutViewModel<AdminLayoutModel>(Controller.ViewData, Constants.ViewData.LayoutViewModel);
            var Model = new TabsViewModel();

            InitTabs(LayoutModel, Controller, Model);

            LocalUtilities.SetLayoutViewModel(ViewData: Controller.ViewData, Model: Model, Key: Constants.ViewData.TabsViewModel);
        }

        void InitTabs(AdminLayoutModel LayoutModel, AdminController Controller, TabsViewModel Model)
        {
            var TabsParentID = LayoutModel.UserItem.Permissions.FindLast(Item =>Item.CodeName == ParentRoute)?.PermissionID;
            
            if (TabsParentID != null)
            {
                Model.Tabs = LayoutModel.UserItem.Permissions.OrderBy(Item => Item.SortIndex)
                .Where(Item => Item.IsMenuItem && Item.ParentID == TabsParentID)
                .Select(Item => new ProjectMenuItem
                {
                    Caption = Item.Caption,
                    NavigateUrl = Controller.Url.RouteUrl(Item.CodeName),
                    IsSelected = Regex.IsMatch(LayoutModel.UrlCurrent, Item.PagePath)
                }).ToList();
            }
        }
    }
}