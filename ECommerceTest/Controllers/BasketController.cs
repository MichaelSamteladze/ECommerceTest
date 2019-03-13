using ECommerceTest.Filters;
using ECommerceTest.Models;
using Reusables.Core;
using System.Web.Mvc;

namespace ECommerceTest.Controllers
{
    [UserAuthorization(Order = 1)]
    [RoutePrefix("basket")]
    public class BasketController : WebsiteController
    {
        [Route("", Name = ControllerActionRouteNames.Website.Basket.Index)]
        public ActionResult Index()
        {
            var Model = BasketModel.GetPageViewModel(this);
            LocalUtilities.SetPageTitle<WebsiteLayoutModel>(ViewData, Model.PageTitle);
            return View(ViewNames.Website.Basket.Index, Model);
        }

        [HttpPost]
        [Route("add", Name = ControllerActionRouteNames.Website.Basket.Add)]
        public ActionResult BasketAdd(int? ProductID, int? ProductCount = 1)
        {
            var Model = BasketModel.BasketAddItem(this, ProductID, ProductCount);
            return Json(Model);
        }

        [HttpPost]
        [Route("delete", Name = ControllerActionRouteNames.Website.Basket.Delete)]
        public ActionResult BasketDelete(int? OrderDetailID)
        {
            var Model = BasketModel.BasketDeleteItem(this,OrderDetailID);
            return Json(Model);
        }

        [HttpPost]
        [Route("update", Name = ControllerActionRouteNames.Website.Basket.Update)]
        public ActionResult BasketUpdate(BasketModel.PageViewModel.BasketSyncProductCountSubmitModel SubmitModel)
        {
            var Model = BasketModel.BasketSyncProductCount(SubmitModel, this);
            return Json(Model);
        }
    }
}