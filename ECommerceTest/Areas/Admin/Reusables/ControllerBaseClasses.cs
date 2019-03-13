using Core.Enums;
using Reusables.Core;
using ECommerceTest.Areas.Admin.FilterAttributes;
using System.Web.Mvc;

namespace ECommerceTest.Areas.Admin.Controllers
{
    [RouteArea(AreaNames.Admin, AreaPrefix = "admin")]
    [BeforeAdminPageLoad(Order = 0)]    
    public class AdminController : ProjectController
    {
        #region Methods        
        public ActionResult NotFound()
        {
            return View(ViewNames.Admin.Shared.NotFound);
        }
        #endregion
    }
}