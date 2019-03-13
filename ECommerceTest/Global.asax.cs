using System.Web.Mvc;
using System.Web.Routing;

namespace ECommerceTest
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {            
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AreaRegistration.RegisterAllAreas();
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
        }        
    }
}
