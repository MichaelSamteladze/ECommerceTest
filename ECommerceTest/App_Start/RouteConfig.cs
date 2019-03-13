using System.Web.Mvc;
using System.Web.Routing;

namespace ECommerceTest
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection Routes)
        {
            Routes.IgnoreRoute("{resource}.ashx/{*pathInfo}");
            Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            Routes.MapMvcAttributeRoutes();

            Routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                namespaces: new string[] { "ECommerceTest.Controllers" },
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );           
        }
    }
}
