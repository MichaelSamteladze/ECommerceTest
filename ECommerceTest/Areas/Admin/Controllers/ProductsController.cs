using Core.Enums;
using DevExpress.Web.Mvc;
using ECommerceTest.Areas.Admin.Models;
using Reusables.Core;
using System.Web.Mvc;

namespace ECommerceTest.Areas.Admin.Controllers
{
    [RoutePrefix("products")]
    public class ProductsController : AdminController
    {
        #region Products Grid
        [Route("", Name = ControllerActionRouteNames.Admin.Products.Index)]
        public ActionResult Products()
        {
            var Model = ProductsGridModel.GetProductsPageViewModel(Url, User);
            return View(ViewNames.Admin.Products.Grid, Model);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.Products.ProductsGrid)]
        public ActionResult ProductsGrid(string ErrorMessage = null)
        {
            var Model = ProductsGridModel.GetProductsGridModel(Url, User, ErrorMessage);
            return PartialView(ViewNames.Admin.Shared.DevexpressGrid, Model);
        }

        [Route("grid/add", Name = ControllerActionRouteNames.Admin.Products.ProductsGridAdd)]
        public ActionResult ProductsGridAdd([ModelBinder(typeof(DevExpressEditorsBinder))] ProductsGridModel.PageViewModel.GridModel.GridItem SubmitModel)
        {
            var ErrorMessage = ProductsGridModel.ProductsIUD(DatabaseActions.CREATE, SubmitModel);
            return ProductsGrid(ErrorMessage);
        }

        [Route("grid/update", Name = ControllerActionRouteNames.Admin.Products.ProductsGridUpdate)]
        public ActionResult ProductsGridUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] ProductsGridModel.PageViewModel.GridModel.GridItem SubmitModel)
        {
            var ErrorMessage = ProductsGridModel.ProductsIUD(DatabaseActions.UPDATE, SubmitModel);
            return ProductsGrid(ErrorMessage);
        }

        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.Products.ProductsGridDelete)]
        public ActionResult ProductsGridDelete([ModelBinder(typeof(DevExpressEditorsBinder))] ProductsGridModel.PageViewModel.GridModel.GridItem SubmitModel)
        {
            var ErrorMessage = ProductsGridModel.ProductsIUD(DatabaseActions.DELETE, SubmitModel);
            return ProductsGrid(ErrorMessage);
        }
        #endregion

        #region Product Properties        
        [Route("{ProductID:int}/properties", Name = ControllerActionRouteNames.Admin.Products.Product.Properties)]
        public ActionResult Properties(int? ProductID)
        {
            var Result = default(ActionResult);
            var Model = ProductModel.GetPageViewModel(ProductID, null, Url, ViewData);

            if (Model == null)
            {
                Result = NotFound();
            }
            else
            {
                LocalUtilities.SetPageTitle<AdminLayoutModel>(ViewData, Model.PageTitle);
                LocalUtilities.UpdateBreadCrumbsItem<AdminLayoutModel>(ViewData: ViewData, Caption: Model.PageTitle);
                Result = View(ViewNames.Admin.Products.Product, Model);
            }

            return Result;
        }

        [HttpPost]
        [Route("{ProductID:int}/properties")]
        [ValidateInput(false)]        
        public ActionResult Properties(int? ProductID, [ModelBinder(typeof(DevExpressEditorsBinder))] ProductModel.PageViewModel SubmitModel)
        {
            var Result = default(ActionResult);

            var Model = ProductModel.GetPageViewModel(ProductID, SubmitModel, Url, ViewData);
            if (Model == null)
            {
                Result = NotFound();
            }
            else
            {
                LocalUtilities.SetPageTitle<AdminLayoutModel>(ViewData, Model.PageTitle);
                LocalUtilities.UpdateBreadCrumbsItem<AdminLayoutModel>(ViewData: ViewData, Caption: Model.PageTitle, RemovePrevious: true);

                ProductModel.ValidatePageViewModel(ProductID, Model);
                if (Model.Form.HasErrors)
                {
                    Result = View(ViewNames.Admin.Products.Product, Model);
                }
                else
                {                    
                    Model = ProductModel.SaveProduct(ProductID, Model);
                    if (Model.Form.IsSaved)
                    {
                        SuccessErrorPartialViewAssistance.InitSuccessMessage(Session: Session);
                        Result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Products.Product.Properties));
                    }
                    else
                    {
                        SuccessErrorPartialViewAssistance.InitErrorMessage<AdminLayoutModel>(ViewData: ViewData);
                        Result = View(ViewNames.Admin.Products.Product, Model);
                    }
                }
            }
            return Result;
        }

        [Route("{ProductID:int}/properties/delete-image", Name = ControllerActionRouteNames.Admin.Products.Product.DeleteImage)]
        public ActionResult ProductDeleteImage(int? ProductID)
        {
            var Model = ProductModel.DeleteImage(ProductID);
            return Json(Model);
        }
        #endregion
    }
}