using Core;
using Core.Enums;
using ECommerceTest.Areas.Admin.Models;
using Reusables.Core;
using System.Web.Mvc;

namespace ECommerceTest.Areas.Admin.Controllers
{
    [RoutePrefix("faq")]
    public class FaqController : AdminController
    {
        [Route("", Name = ControllerActionRouteNames.Admin.Faq.Page)]
        public ActionResult Faq()
        {
            var Model = FaqModel.GetFaqPageViewModel(Url);
            return View(ViewNames.Admin.Faq.Page, Model);
        }

        [HttpPost]
        [Route("create", Name = ControllerActionRouteNames.Admin.Faq.FaqCreate)]
        [ValidateInput(false)]
        public ActionResult FaqCreate(FaqItem SubmitModel)
        {
            var Response = FaqModel.FaqIUD(DatabaseActions.CREATE,SubmitModel);
            return Json(Response);
        }

        [HttpPost]
        [Route("udpate", Name = ControllerActionRouteNames.Admin.Faq.FaqUpdate)]
        [ValidateInput(false)]
        public ActionResult FaqUpdate(FaqItem SubmitModel)
        {
            var Response = FaqModel.FaqIUD(DatabaseActions.UPDATE,SubmitModel);
            return Json(Response);
        }

        [HttpPost]
        [Route("delete", Name = ControllerActionRouteNames.Admin.Faq.FaqDelete)]
        public ActionResult FaqDelete(FaqItem SubmitModel)
        {
            var Response = FaqModel.FaqIUD(DatabaseActions.DELETE, SubmitModel);
            return Json(Response);
        }

        [HttpPost]
        [Route("sync-srot-indexes", Name = ControllerActionRouteNames.Admin.Faq.FaqSyncSortIndexes)]
        public ActionResult FaqSyncSortIndexes(SyncSortIndexesModel SubmitModel)
        {
            var Response = FaqModel.FaqSyncSortIndexes(SubmitModel);
            return Json(Response);
        }
    }
}