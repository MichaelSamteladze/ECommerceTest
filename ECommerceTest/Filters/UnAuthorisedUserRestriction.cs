using ECommerceTest.Controllers;
using Reusables.Core;
using System.Web.Mvc;

namespace ECommerceTest.Filters
{
    public class UserAuthorization : FilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuted(ActionExecutedContext FilterContext)
        {

        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext FilterContext)
        {
            var Controller = FilterContext.Controller as WebsiteController;            
            if (Controller.User == null)
            {
                FilterContext.Result = new RedirectResult(Controller.Url.RouteUrl(ControllerActionRouteNames.Website.Auth.SignIn));
            }
        }
    }
}