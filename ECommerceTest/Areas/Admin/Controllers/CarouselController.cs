using Core.Enums;
using DevExpress.Web.Mvc;
using ECommerceTest.Areas.Admin.Models;
using Reusables.Core;
using System.Web.Mvc;

namespace ECommerceTest.Areas.Admin.Controllers
{
    [RoutePrefix("carousel")]
    public class CarouselController : AdminController
    {
        #region Carousel Grid
        [Route("", Name = ControllerActionRouteNames.Admin.Carousel.Page)]
        public ActionResult Carousel()
        {
            var Model = CarouselGridModel.GetCarouselPageViewModel(Url, User);
            return View(ViewNames.Admin.Carousel.Grid, Model);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.Carousel.Grid)]
        public ActionResult CarouselGrid(string ErrorMessage = null)
        {
            var Model = CarouselGridModel.GetCarouselGridModel(Url, User, ErrorMessage);
            return PartialView(ViewNames.Admin.Shared.DevexpressGrid, Model);
        }

        [Route("grid/add", Name = ControllerActionRouteNames.Admin.Carousel.GridAdd)]
        public ActionResult CarouselGridAdd([ModelBinder(typeof(DevExpressEditorsBinder))] CarouselGridModel.PageViewModel.GridModel.GridItem SubmitModel)
        {
            var ErrorMessage = CarouselGridModel.CarouselIUD(DatabaseActions.CREATE, SubmitModel);
            return CarouselGrid(ErrorMessage);
        }

        [Route("grid/update", Name = ControllerActionRouteNames.Admin.Carousel.GridUpdate)]
        public ActionResult CarouselGridUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] CarouselGridModel.PageViewModel.GridModel.GridItem SubmitModel)
        {
            var ErrorMessage = CarouselGridModel.CarouselIUD(DatabaseActions.UPDATE, SubmitModel);
            return CarouselGrid(ErrorMessage);
        }

        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.Carousel.GridDelete)]
        public ActionResult CarouselGridDelete([ModelBinder(typeof(DevExpressEditorsBinder))] CarouselGridModel.PageViewModel.GridModel.GridItem SubmitModel)
        {
            var ErrorMessage = CarouselGridModel.CarouselIUD(DatabaseActions.DELETE,SubmitModel);
            return CarouselGrid(ErrorMessage);
        }

        [Route("grid/sync-sort-indexes", Name = ControllerActionRouteNames.Admin.Carousel.GridSyncSortIndexes)]
        public ActionResult CarouselGridSyncSortIndexes(SyncSortIndexesModel SubmitModel)
        {
            var Response = CarouselGridModel.SyncSortIndexes(SubmitModel);
            return Json(Response);
        }
        #endregion

        #region Carousel Properties
        [Route("{CarouselID:int}/properties", Name = ControllerActionRouteNames.Admin.Carousel.CarouselItem.Properties)]
        public ActionResult CarouselProperties(int? CarouselID)
        {
            var Result = NotFound();
            var Model = CarouselItemModel.Properties.GetPageViewModel(CarouselID, null, Url, ViewData);
            if (Model != null)
            {
                Result = View(ViewNames.Admin.Carousel.Item, Model);
            }
            return Result;
        }

        [HttpPost]
        [Route("{CarouselID:int}/properties")]
        [ValidateInput(false)]
        public ActionResult Properties(int? CarouselID, CarouselItemModel.Properties.PageViewModel SubmitModel)
        {
            var Result = NotFound();

            var Model = CarouselItemModel.Properties.GetPageViewModel(CarouselID, SubmitModel, Url, ViewData);
            if (Model != null)
            {
                CarouselItemModel.Properties.ValidateCarouselItemPageViewModel(Model);
                if (Model.Form.Errors.Count == 0)
                {
                    Model = CarouselItemModel.Properties.SaveCarouselItem(CarouselID, Model);
                    if (Model.Form.IsSaved)
                    {
                        SuccessErrorPartialViewAssistance.InitSuccessMessage(Session: Session);
                        Result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Carousel.CarouselItem.Properties));
                    }
                    else
                    {
                        SuccessErrorPartialViewAssistance.InitErrorMessage<AdminLayoutModel>(ViewData: ViewData);
                        Result = View(ViewNames.Admin.Carousel.Item, Model);
                    }
                }
                else
                {
                    Result = View(ViewNames.Admin.Carousel.Item, Model);
                }
            }
            return Result;
        }

        [Route("{CarouselID:int}/properties/delete-image", Name = ControllerActionRouteNames.Admin.Carousel.CarouselItem.DeleteImage)]
        public ActionResult CarouselItemDeleteImage(int? CarouselID)
        {
            var Response = CarouselItemModel.Properties.DeleteImage(CarouselID);
            return Json(Response);
        }
        #endregion
    }
}