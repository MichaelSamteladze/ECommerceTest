using Reusables.Core;
using System.Web.Mvc;

namespace ECommerceTest.Areas.Admin.Controllers
{
    public class HomeController : AdminController
    {
        [Route("", Name = ControllerActionRouteNames.Admin.Home.Index)]
        public ActionResult Index()
        {
            return Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Orders.Index));
        }
    }
}