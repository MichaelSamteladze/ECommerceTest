using Reusables.Core;
using ECommerceTest.Areas.Admin.Models;
using System.Web.Mvc;

namespace ECommerceTest.Areas.Admin.Controllers
{    
    [RoutePrefix("admin")]
    public class AuthController : ProjectController
    {
        [HttpGet]        
        [Route("login", Name = ControllerActionRouteNames.Admin.Auth.Login)]
        public ActionResult Login()
        {
            if(LoginModel.IsUserLoggedIn(Session))
            {
                return Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Home.Index));
            }

            var Model = new LoginModel.PageViewModel();
            return View(ViewNames.Admin.Auth.Login, Model);
        }

        [HttpPost]
        [Route("login")]
        public ActionResult Login(LoginModel.PageViewModel Model)
        {
            LoginModel.AuthenticateUser(Url, Session, Model);
            if (Model.IsLoginFailed)
            {
                return View(ViewNames.Admin.Auth.Login, Model);                
            }
            else
            {
                return Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Home.Index));
            }
        }

        [Route("logout",Name =ControllerActionRouteNames.Admin.Auth.Logout)]
        public ActionResult Logout()
        {
            Session.Clear();
            return Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Auth.Login));
        }        
    }
}