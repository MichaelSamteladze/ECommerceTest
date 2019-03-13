using DevExpress.Web.Mvc;
using ECommerceTest.Areas.Admin.Models;
using Reusables.Core;
using System.Web.Mvc;

namespace ECommerceTest.Areas.Admin.Controllers
{
    [RoutePrefix("categories")]
    public class CategoriesController : AdminController
    {
        #region Tree
        [Route("", Name = ControllerActionRouteNames.Admin.Categories.Index)]
        public ActionResult Categories()
        {
            var Model = CategoriesTreeModel.GetCategoriesPageViewModel(Url, User);
            return View(ViewNames.Admin.Categories.Tree, Model);
        }

        [HttpPost]
        [Route("add", Name = ControllerActionRouteNames.Admin.Categories.AddNew)]
        public ActionResult CategoriesAdd(int? ParentID, string CategoryCaption)
        {
            var Model = CategoriesTreeModel.CategoryCreate(ParentID, CategoryCaption, Url, User, this);
            return Json(Model);
        }

        [HttpPost]
        [Route("update", Name = ControllerActionRouteNames.Admin.Categories.Update)]
        public ActionResult CategoriesEdit(int? CategoryID, string CategoryCaption = null, bool? IsPublished = null)
        {
            var Model = CategoriesTreeModel.CategoryUpdate(CategoryID, CategoryCaption, IsPublished);
            return Json(Model);
        }

        [HttpPost]
        [Route("sync-parents-sort-indexes", Name = ControllerActionRouteNames.Admin.Categories.SyncParentsAndSortIndexes)]
        public ActionResult CategoriesSyncParentsAndSortIndexes(SyncSortIndexesModel SubmitModel)
        {
            var Model = CategoriesTreeModel.SyncParentsAndSortIndexes(SubmitModel);
            return Json(Model);
        }

        [HttpPost]
        [Route("delete", Name = ControllerActionRouteNames.Admin.Categories.Delete)]
        public ActionResult CategoriesDelete(string Hash)
        {
            var Model = CategoriesTreeModel.DeleteCategory(Hash);
            return Json(Model);
        }
        #endregion Tree

        #region Category
        [Route("{CategoryID:int}/properties", Name = ControllerActionRouteNames.Admin.Categories.Category.Properties)]
        public ActionResult CategoryProperties(int? CategoryID)
        {
            var Model = CategoryModel.GetPageViewModel(CategoryID, null, Url, ViewData);
            if (Model == null)
            {
                return NotFound();
            }
            else
            {
                LocalUtilities.UpdateBreadCrumbsItem<AdminLayoutModel>(ViewData: ViewData, Caption: Model.PageTitle, RemovePrevious: true);
                LocalUtilities.SetPageTitle<AdminLayoutModel>(ViewData, Model.PageTitle);
                return View(ViewNames.Admin.Categories.Category, Model);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [Route("{CategoryID:int}/properties")]
        public ActionResult CategoryProperties([ModelBinder(typeof(DevExpressEditorsBinder))] CategoryModel.PageViewModel Model , int? CategoryID)
        {
            var Result = NotFound();

            Model = CategoryModel.GetPageViewModel(CategoryID, Model, Url, ViewData);
            if (Model != null)
            {
                CategoryModel.ValidateCategoryViewModel(CategoryID, Model);
                if (Model.Form.Errors.Count == 0)
                {
                    Model = CategoryModel.SaveCategory(CategoryID, Model);
                    if (Model.Form.IsSaved)
                    {
                        SuccessErrorPartialViewAssistance.InitSuccessMessage(Session: Session);
                        Result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Categories.Category.Properties));
                    }
                    else
                    {
                        SuccessErrorPartialViewAssistance.InitErrorMessage<AdminLayoutModel>(ViewData: ViewData);
                        Result = View(ViewNames.Admin.Categories.Category, Model);
                    }
                }
                else
                {
                    Result = View(ViewNames.Admin.Categories.Category, Model);
                }
            }

            return Result;
        }

        [HttpPost]
        [Route("{CategoryID:int}/properties/delete-image", Name = ControllerActionRouteNames.Admin.Categories.Category.DeleteImage)]
        public ActionResult CategoryDeleteImage(int? CategoryID)
        {
            var Response = CategoryModel.DeleteImage(CategoryID);
            return Json(Response);
        }
        #endregion                
    }
}