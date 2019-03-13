using Reusables.Core;
using ECommerceTest.Filters;
using System.Web.Mvc;
using ECommerceTest.Models;

namespace ECommerceTest.Controllers
{
    [BeforePageLoad]
    public class WebsiteController : ProjectController
    {
        #region Properties
        public WebsiteLayoutModel LayoutModel { get; set; }
        #endregion

        #region Methods        
        public ActionResult NotFound(bool WithHttpStatusCode = false)
        {
            if (WithHttpStatusCode)
            {
                Response.StatusCode = 404;
                Response.TrySkipIisCustomErrors = true;
            }
            return View(ViewNames.Admin.Shared.NotFound);
        }
        #endregion
    }
}