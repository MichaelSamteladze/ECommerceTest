using ECommerceTest.Models;
using Reusables.Core;
using System.Web.Mvc;

namespace ECommerceTest.Controllers
{
    public class HomeController : WebsiteController
    {
        #region Home
        [Route("", Name = ControllerActionRouteNames.Website.Home.Index)]
        public ActionResult Index()
        {
            LocalUtilities.SetPageTitle<WebsiteLayoutModel>(ViewData: ViewData, PageTitle: null);
            var Model = HomeModel.GetPageViewModel(Url, ViewData);
            return View(ViewNames.Website.Home.Index, Model);
        }
        #endregion

        #region About
        [Route("about", Name = ControllerActionRouteNames.Website.Home.About)]
        public ActionResult About()
        {            
            var Model = AboutModel.GetPageViewModel();
            LocalUtilities.SetPageTitle<WebsiteLayoutModel>(ViewData: ViewData, PageTitle: Model.PageTitle);
            return View(ViewNames.Website.Home.About, Model);
        }
        #endregion
        
        #region FAQ
        [Route("faq", Name = ControllerActionRouteNames.Website.Home.FAQ)]
        public ActionResult FAQ()
        {
            var Model = FaqModel.GetPageViewModel(ViewData);
            LocalUtilities.SetPageTitle<WebsiteLayoutModel>(ViewData: ViewData, PageTitle: Model.PageTitle);
            return View(ViewNames.Website.Home.FAQ, Model);
        }
        #endregion

        #region Contact
        [Route("contact", Name = ControllerActionRouteNames.Website.Home.Contact)]
        public ActionResult Contact()
        {
            var Model = ContactModel.GetPageViewModel(ViewData);
            LocalUtilities.SetPageTitle<WebsiteLayoutModel>(ViewData: ViewData, PageTitle: Model.PageTitle);
            return View(ViewNames.Website.Home.Contact, Model);
        }

        [HttpPost]
        [Route("contact")]
        public ActionResult Contact(ContactModel.PageViewModel SubmitModel)
        {            
            var Model = ContactModel.GetPageViewModel(ViewData, SubmitModel);
            ContactModel.ValidateContactForm(Model);
            if (!Model.Form.HasErrors)
            {
                ContactModel.SendContactEmail(Model);
            }
            return View(ViewNames.Website.Home.Contact, Model); 
        }
        #endregion
    }
}