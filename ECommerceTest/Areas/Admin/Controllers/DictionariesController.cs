using Core.Enums;
using DevExpress.Web.Mvc;
using Reusables.Core;
using ECommerceTest.Areas.Admin.Models;
using System.Web.Mvc;

namespace ECommerceTest.Areas.Admin.Controllers
{
    [RoutePrefix("dictionaries")]
    public class DictionariesController : AdminController
    {
        #region Dictionaries
        [Route("", Name = ControllerActionRouteNames.Admin.Dictionaries.Page)]
        public ActionResult Dictionaries()
        {
            var Model = DictionariesModel.GetDictionaryViewModel(Url, User);
            return View(ViewNames.Admin.Dictionaries.Page, Model);
        }

        [Route("tree", Name = ControllerActionRouteNames.Admin.Dictionaries.DictionariesTree)]
        public ActionResult DictionariesTree(string ErrorMessage = null)
        {
            var Model = DictionariesModel.GetDictionaryTreeModel(Url, User);
            return PartialView(ViewNames.Admin.Shared.DevexpressTree, Model);
        }

        [Route("tree/add", Name = ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeAdd)]
        public ActionResult DictionariesTreeAdd([ModelBinder(typeof(DevExpressEditorsBinder))] DictionariesModel.PageViewModel.TreeModel.TreeNode SubmitModel)
        {
            string ErrorMessage = DictionariesModel.CRUD(User,DatabaseActions.CREATE,SubmitModel);
            return DictionariesTree(ErrorMessage);
        }

        [Route("tree/update", Name = ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeUpdate)]
        public ActionResult DictionariesTreeUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] DictionariesModel.PageViewModel.TreeModel.TreeNode SubmitModel)
        {
            string ErrorMessage = DictionariesModel.CRUD(User, DatabaseActions.UPDATE, SubmitModel);
            return DictionariesTree(ErrorMessage);
        }

        [Route("tree/update-parent", Name = ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeUpdateParent)]
        public ActionResult DictionariesTreeUpdateParent([ModelBinder(typeof(DevExpressEditorsBinder))] DictionariesModel.PageViewModel.TreeModel.TreeNode SubmitModel)
        {
            string ErrorMessage = DictionariesModel.UpdateParent(User, SubmitModel);
            return DictionariesTree(ErrorMessage);
        }

        [Route("tree/delete", Name = ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeDelete)]
        public ActionResult DictionariesTreeDelete([ModelBinder(typeof(DevExpressEditorsBinder))] DictionariesModel.PageViewModel.TreeModel.TreeNode SubmitModel)
        {
            DictionariesModel.CRUD(User, DatabaseActions.DELETE, SubmitModel);
            return DictionariesTree();
        }
        #endregion        
    }
}