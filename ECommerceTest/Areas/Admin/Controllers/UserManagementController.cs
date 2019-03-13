using Core.Enums;
using DevExpress.Web.Mvc;
using ECommerceTest.Areas.Admin.Models;
using Reusables.Core;
using System.Web.Mvc;

namespace ECommerceTest.Areas.Admin.Controllers
{
    [RoutePrefix("um")]
    public class UserManagementController : AdminController
    {
        #region Users
        [Route("users", Name = ControllerActionRouteNames.Admin.UserManagement.Users)]
        public ActionResult UserManagemetUsers()
        {
            var Model = UsersModel.GetUsersViewModel(Url, User);
            return View(ViewNames.Admin.UserManagement.Users, Model);
        }

        [Route("users/grid", Name = ControllerActionRouteNames.Admin.UserManagement.UsersGrid)]
        public ActionResult UserManagemetUsersGrid(string ErrorMessage = null)
        {
            var Model = UsersModel.GetUsersGridViewModel(Url, User, ErrorMessage);
            return PartialView(ViewNames.Admin.Shared.DevexpressGrid, Model);
        }

        [HttpPost, ValidateInput(false)]
        [Route("users/grid/add", Name = ControllerActionRouteNames.Admin.UserManagement.UsersGridAdd)]
        public ActionResult UserManagemetUsersGridAdd([ModelBinder(typeof(DevExpressEditorsBinder))] UsersModel.PageViewModel.GridModel.GridItem Model)
        {
            var ErrorMessage = UsersModel.ValidateEmail(Email: Model.Email, UserID: null);
            if (string.IsNullOrWhiteSpace(ErrorMessage))
            {
                ErrorMessage = UsersModel.CRUD(DatabaseAction: DatabaseActions.CREATE, Model: Model);
            }
            return UserManagemetUsersGrid(ErrorMessage: ErrorMessage);
        }

        [Route("users/grid/update", Name = ControllerActionRouteNames.Admin.UserManagement.UsersGridUpdate)]
        public ActionResult UserManagemetUsersGridUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] UsersModel.PageViewModel.GridModel.GridItem Model)
        {
            var ErrorMessage = UsersModel.ValidateEmail(Email: Model.Email, UserID: Model.UserID);
            if (string.IsNullOrWhiteSpace(ErrorMessage))
            {
                ErrorMessage = UsersModel.CRUD(DatabaseAction: DatabaseActions.UPDATE, Model: Model);
            }
            return UserManagemetUsersGrid(ErrorMessage: ErrorMessage);
        }

        [Route("users/grid/delete", Name = ControllerActionRouteNames.Admin.UserManagement.UsersGridDelete)]
        public ActionResult UserManagemetUsersGridDelete([ModelBinder(typeof(DevExpressEditorsBinder))] UsersModel.PageViewModel.GridModel.GridItem Model)
        {
            var ErrorMessage = UsersModel.DeleteUser(Model: Model);
            return UserManagemetUsersGrid(ErrorMessage: ErrorMessage);
        }
        #endregion Users

        #region Permissions
        [Route("permissions", Name = ControllerActionRouteNames.Admin.UserManagement.Permissions)]
        public ActionResult UserManagementPermissions()
        {
            var Model = PermissionsModel.GetPermissionsViewModel(Url, User);
            return View(ViewNames.Admin.UserManagement.Permissions, Model);
        }

        [Route("permissions/tree", Name = ControllerActionRouteNames.Admin.UserManagement.PermissionsTree)]
        public ActionResult UserManagementPermissionsTree(string ErrorMessage = null)
        {
            var Model = PermissionsModel.GetPermissionsTreeViewModel(Url, User, ErrorMessage);
            return PartialView(ViewNames.Admin.Shared.DevexpressTree, Model);
        }

        [Route("permissions/tree/add", Name = ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeAdd)]
        public ActionResult UserManagementPermissionsTreeAdd([ModelBinder(typeof(DevExpressEditorsBinder))] PermissionsModel.PageViewModel.TreeModel.TreeNode SubmitModel)
        {
            var ErrorMessage = PermissionsModel.AddPermission(SubmitModel);
            return UserManagementPermissionsTree(ErrorMessage);
        }

        [Route("permissions/tree/update", Name = ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeUpdate)]
        public ActionResult UserManagementPermissionsTreeUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] PermissionsModel.PageViewModel.TreeModel.TreeNode SubmitModel)
        {
            var ErrorMessage = PermissionsModel.UpdatePermission(SubmitModel);
            return UserManagementPermissionsTree(ErrorMessage);
        }

        [Route("permissions/update-parent", Name = ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeUpdateParent)]
        public ActionResult UserManagementPermissionsTreeUpdateParent([ModelBinder(typeof(DevExpressEditorsBinder))] PermissionsModel.PageViewModel.TreeModel.TreeNode SubmitModel)
        {
            var ErrorMessage = PermissionsModel.UpdateParent(SubmitModel);
            return UserManagementPermissionsTree(ErrorMessage);
        }

        [Route("permissions/tree/delete", Name = ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeDelete)]
        public ActionResult UserManagementPermissionsTreeDelete([ModelBinder(typeof(DevExpressEditorsBinder))] PermissionsModel.PageViewModel.TreeModel.TreeNode SubmitModel)
        {
            var ErrorMessage = PermissionsModel.DeletePermission(SubmitModel);
            return UserManagementPermissionsTree(ErrorMessage);
        }
        #endregion Permissions
    }
}