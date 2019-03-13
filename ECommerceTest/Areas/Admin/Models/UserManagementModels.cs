using Core;
using Core.Enums;
using Core.Properties;
using Core.Services;
using Core.Utilities;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using Reusables.Core;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Core.Utilities.Constants;

namespace ECommerceTest.Areas.Admin.Models
{
    public class UsersModel
    {
        #region Methods
        public static PageViewModel GetUsersViewModel(UrlHelper Url, User UserItem)
        {
            return new PageViewModel
            {
                ShowAddNewButton = UserItem.HasPermission(ControllerActionRouteNames.Admin.UserManagement.UsersGridAdd),
                ShowGrid = UserItem.HasPermission(ControllerActionRouteNames.Admin.UserManagement.UsersGrid),
                Grid = GetUsersGridViewModel(Url, UserItem)
            };
        }

        public static string ValidateEmail(string Email, int? UserID)
        {
            var Error = Validation.ValidateEmail(ErrorKey: nameof(Email), Email: Email, ValidateRequired: true, ValidateUnique: true, UserID: UserID);
            return Error?.Value;
        }

        public static PageViewModel.GridModel GetUsersGridViewModel(UrlHelper Url, User UserItem, string ErrorMessage = null)
        {
            return new PageViewModel.GridModel
            {
                ErrorMessage = ErrorMessage,
                ShowUpdateButton = UserItem.HasPermission(ControllerActionRouteNames.Admin.UserManagement.UsersGridUpdate),
                ShowDeleteButton = UserItem.HasPermission(ControllerActionRouteNames.Admin.UserManagement.UsersGridDelete),
                UrlList = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.UsersGrid),
                UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.UsersGridAdd),
                UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.UsersGridUpdate),
                UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.UsersGridDelete),
                GridItems = UsersDataAccess.ListUsers()?.Select(Item => new PageViewModel.GridModel.GridItem
                {
                    UserID = Item.UserID,
                    Email = Item.Email,
                    Firstname = Item.Firstname,
                    Lastname = Item.Lastname,
                    IsActive = Item.IsActive,
                    CRTime = Item.CRTime
                }).ToList()
            };
        }

        public static string CRUD(byte DatabaseAction, PageViewModel.GridModel.GridItem Model)
        {
            var DAL = new UsersDataAccess();
            DAL.UsersIUD(
                DatabaseAction: DatabaseAction,
                UserID: Model.UserID,
                Email: Model.Email,
                Password: Model.Password,
                Firstname: Model.Firstname ?? NullValueFor.String,
                Lastname: Model.Lastname ?? NullValueFor.String,
                IsActive: Model.IsActive
            );
            return Utility.GetDatabaseErrorMessage(DAL);
        }

        public static string DeleteUser(PageViewModel.GridModel.GridItem Model)
        {
            var DAL = new UsersDataAccess();
            DAL.DeleteUser(UserID: Model.UserID);
            return Utility.GetDatabaseErrorMessage(DAL);
        }
        #endregion

        #region Sub Classes
        public class PageViewModel
        {
            #region Properties
            public GridModel Grid { get; set; }
            public bool ShowAddNewButton { get; set; }
            public bool ShowGrid { get; set; }
            #endregion

            public class GridModel : DevexpressGridViewModelBase
            {
                #region Properties                        
                const string GridName = "UsersGrid";
                public bool ShowCommandColumn => ShowUpdateButton && ShowDeleteButton;
                public List<SimpleKeyValue<int?, string>> Roles { get; set; }
                public List<GridItem> GridItems { get; set; }
                #endregion

                #region Methods
                public override void InitGridSettings(GridViewSettings Settings, HtmlHelper Html, UrlHelper Url)
                {
                    InitGridViewDefaultSettings(Settings);
                    GridItem GridItem;

                    Settings.Name = GridName;
                    Settings.KeyFieldName = nameof(GridItem.UserID);

                    Settings.CommandColumn.Visible = ShowCommandColumn;
                    Settings.CommandColumn.Width = Unit.Pixel(80);
                    Settings.CommandColumn.ShowDeleteButton = ShowDeleteButton;
                    Settings.CommandColumn.ShowEditButton = ShowUpdateButton;

                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(GridItem.Firstname); column.Caption = "Firstname"; column.Width = Unit.Pixel(200); column.ColumnType = MVCxGridViewColumnType.TextBox;
                        var Properties = column.PropertiesEdit as TextBoxProperties;
                        InitTextEditProperties(Properties: Properties, MaxLength: 20, MakeRequired: true);
                    });
                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(GridItem.Lastname); column.Caption = "Lastname"; column.Width = Unit.Pixel(200); column.ColumnType = MVCxGridViewColumnType.TextBox;
                        var Properties = column.PropertiesEdit as TextBoxProperties;
                        InitTextEditProperties(Properties: Properties, MaxLength: 20, MakeRequired: true);
                    });
                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(GridItem.Email); column.Caption = "Email"; column.Width = Unit.Pixel(300); column.ColumnType = MVCxGridViewColumnType.TextBox;
                        var Properties = column.PropertiesEdit as TextBoxProperties;
                        InitTextEditProperties(Properties: Properties, MaxLength: 80, MakeRequired: true);
                    });
                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(GridItem.Password); column.Caption = "Password"; column.Width = Unit.Pixel(100); column.ColumnType = MVCxGridViewColumnType.TextBox;
                        column.SetDataItemTemplateContent("****");
                    });                    
                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(GridItem.IsActive); column.Caption = "Active"; column.Width = Unit.Pixel(80); column.ColumnType = MVCxGridViewColumnType.CheckBox;
                        var Properties = column.PropertiesEdit as CheckBoxProperties;
                        InitCheckBoxProperties(Properties: Properties, GridName: GridName);
                    });
                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(GridItem.CRTime); column.Caption = "Date Reg."; column.Width = Unit.Pixel(180); column.ColumnType = MVCxGridViewColumnType.DateEdit;
                        var Properties = column.PropertiesEdit as DateEditProperties;
                        InitDateEditProperties(Properties: Properties, UseDateTimeFormat: true);
                        column.SetEditItemTemplateContent(" ");
                    });

                    Settings.Columns.Add(column => { column.SetEditItemTemplateContent(" "); });
                }
                #endregion

                #region Sub Classes
                public class GridItem
                {
                    #region Properties
                    public int? UserID { get; set; }
                    public string Firstname { get; set; }
                    public string Lastname { get; set; }
                    public string Email { get; set; }
                    public string Password { get; set; }
                    public bool IsActive { get; set; }
                    public DateTime? CRTime { get; set; }
                    #endregion
                }
                #endregion
            }
        }
        #endregion
    }

    public class PermissionsModel
    {
        #region Methods
        public static PageViewModel GetPermissionsViewModel(UrlHelper Url, User UserItem)
        {
            return new PageViewModel
            {
                ShowAddNewButton = UserItem.HasPermission(ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeAdd),
                ShowTree = UserItem.HasPermission(ControllerActionRouteNames.Admin.UserManagement.PermissionsTree),
                Tree = GetPermissionsTreeViewModel(Url, UserItem)
            };
        }

        public static PageViewModel.TreeModel GetPermissionsTreeViewModel(UrlHelper Url, User UserItem, string ErrorMessage = null)
        {
            return new PageViewModel.TreeModel
            {
                ErrorMessage = ErrorMessage,
                ShowAddNewButton = UserItem.HasPermission(ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeAdd),
                ShowUpdateButton = UserItem.HasPermission(ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeUpdate),
                ShowDeleteButton = UserItem.HasPermission(ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeDelete),
                UrlList = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.PermissionsTree),
                UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeAdd),
                UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeUpdate),
                UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeDelete),
                UrlNodeDragDrop = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeUpdateParent),
                TreeNodes = PermissionsDataAccess.ListPermissions().Select(P => new PageViewModel.TreeModel.TreeNode
                {
                    PermissionID = P.PermissionID,
                    ParentID = P.ParentID,
                    Caption = P.Caption,
                    PagePath = P.PagePath,
                    CodeName = P.CodeName,
                    PermissionCode = P.PermissionCode,
                    SortIndex = P.SortIndex,
                    MenuIcon = P.MenuIcon,
                    IsMenuItem = P.IsMenuItem
                }).ToList()
            };
        }

        public static string AddPermission(PageViewModel.TreeModel.TreeNode Model)
        {
            var DAL = new PermissionsDataAccess();
            DAL.PermissionsIUD(
                DatabaseAction: DatabaseActions.CREATE,
                ParentID: Model.ParentID,
                Caption: Model.Caption,
                PagePath: Model.PagePath,
                CodeName: Model.CodeName,
                PermissionCode: Model.PermissionCode,
                SortIndex: Model.SortIndex,
                IsMenuItem: Model.IsMenuItem,
                MenuIcon: Model.MenuIcon
            );

            return Utility.GetDatabaseErrorMessage(DAL);
        }

        public static string UpdatePermission(PageViewModel.TreeModel.TreeNode Model)
        {
            var DAL = new PermissionsDataAccess();
            DAL.PermissionsIUD(
                DatabaseAction: DatabaseActions.UPDATE,
                PermissionID: Model.PermissionID,
                ParentID: Model.ParentID,
                Caption: Model.Caption ?? Constants.NullValueFor.String,
                PagePath: Model.PagePath ?? Constants.NullValueFor.String,
                CodeName: Model.CodeName ?? Constants.NullValueFor.String,
                PermissionCode: Model.PermissionCode ?? Constants.NullValueFor.String,
                SortIndex: Model.SortIndex,
                IsMenuItem: Model.IsMenuItem,
                MenuIcon: Model.MenuIcon ?? Constants.NullValueFor.String
            );

            return Utility.GetDatabaseErrorMessage(DAL);
        }

        public static string UpdateParent(PageViewModel.TreeModel.TreeNode Model)
        {
            var DAL = new PermissionsDataAccess();
            DAL.PermissionsIUD(
                DatabaseAction: DatabaseActions.UPDATE,
                PermissionID: Model.PermissionID,
                ParentID: Model.ParentID ?? NullValueFor.Int
            );

            return Utility.GetDatabaseErrorMessage(DAL);
        }

        public static string DeletePermission(PageViewModel.TreeModel.TreeNode Model)
        {
            var DAL = new PermissionsDataAccess();
            DAL.DeleteRecursive(
                PermissionID: Model.PermissionID
            );

            return Utility.GetDatabaseErrorMessage(DAL);
        }
        #endregion

        #region Sub Classes
        public class PageViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public bool ShowTree { get; set; }
            public TreeModel Tree { get; set; }
            #endregion

            #region Sub Classes
            public class TreeModel : DevexpressTreeListVeiwModelBase
            {
                #region Properties
                const string TreeName = "PermissionsTree";
                public List<TreeNode> TreeNodes { get; set; }
                #endregion

                #region Methods
                public override void InitTreeSettings(TreeListSettings Settings, HtmlHelper Html, UrlHelper Url)
                {
                    InitTreeListDefaultSettings(Settings);
                    TreeNode TreeNode;

                    Settings.Name = TreeName;
                    Settings.KeyFieldName = nameof(TreeNode.PermissionID);
                    Settings.ParentFieldName = nameof(TreeNode.ParentID);

                    Settings.SettingsEditing.AllowRecursiveDelete = true;

                    Settings.CommandColumn.Visible = true;
                    Settings.CommandColumn.NewButton.Visible = ShowAddNewButton;
                    Settings.CommandColumn.DeleteButton.Visible = ShowDeleteButton;
                    Settings.CommandColumn.EditButton.Visible = ShowUpdateButton;
                    Settings.SettingsBehavior.AutoExpandAllNodes = false;
                    Settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Visible;


                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(TreeNode.Caption); column.Caption = "Caption"; column.ColumnType = MVCxTreeListColumnType.TextBox; column.Width = Unit.Pixel(300);
                        var Properties = column.PropertiesEdit as TextBoxProperties;
                        InitTextEditProperties(Properties: Properties, MaxLength: 100, MakeRequired: true);
                    });
                    Settings.Columns.Add(column => { column.FieldName = nameof(TreeNode.PagePath); column.Caption = "Page Path"; column.ColumnType = MVCxTreeListColumnType.TextBox; column.Width = Unit.Pixel(300); });
                    Settings.Columns.Add(column => { column.FieldName = nameof(TreeNode.CodeName); column.Caption = "Code Name"; column.ColumnType = MVCxTreeListColumnType.TextBox; column.Width = Unit.Pixel(350); });
                    Settings.Columns.Add(column => { column.FieldName = nameof(TreeNode.PermissionCode); column.Caption = "Permission Code"; column.ColumnType = MVCxTreeListColumnType.TextBox; column.Width = Unit.Pixel(300); });
                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(TreeNode.SortIndex); column.Caption = "Sort Index"; column.ColumnType = MVCxTreeListColumnType.SpinEdit; column.Width = Unit.Pixel(100);
                        var Properties = column.PropertiesEdit as SpinEditProperties;
                        InitSpinEditProperties(Properties: Properties);
                    });
                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(TreeNode.IsMenuItem); column.Caption = "Is Menu?"; column.Width = Unit.Pixel(80); column.ColumnType = MVCxTreeListColumnType.CheckBox;
                        var Properties = column.PropertiesEdit as CheckBoxProperties;
                        Properties.ClientSideEvents.Init = $"function(s,e){{ Globals.Devexpress.OnTreeCheckBoxColumnEditorInit({TreeName},s,e); }}";
                    });
                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(TreeNode.MenuIcon); column.Caption = "Icon"; column.Width = Unit.Pixel(80); column.ColumnType = MVCxTreeListColumnType.TextBox;
                    });

                    Settings.CommandColumn.VisibleIndex = Settings.Columns.Count;
                }
                #endregion

                #region Sub Classes
                public class TreeNode
                {
                    #region Properties
                    public int? PermissionID { get; set; }
                    public int? ParentID { get; set; }
                    public string Caption { get; set; }
                    public string PagePath { get; set; }
                    public string CodeName { get; set; }
                    public string PermissionCode { get; set; }
                    public bool IsMenuItem { get; set; }
                    public string MenuIcon { get; set; }
                    public int? SortIndex { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}