using Core.Enums;
using System.Web.Mvc;

namespace ECommerceTest.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return AreaNames.Admin;
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            var Routes = context.MapRoute(
                name: "Admin_default",
                url: "Admin/{controller}/{action}/{id}",
                namespaces: new string[] { "ECommerceTest.Areas.Admin.Controllers" },
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );            
        }
    }
}