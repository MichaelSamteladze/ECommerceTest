using Core;
using Core.Enums;
using Core.Properties;
using Core.Utilities;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using Reusables.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace ECommerceTest.Areas.Admin.Models
{
    public class DictionariesModel
    {
        #region Methods

        public static PageViewModel GetDictionaryViewModel(UrlHelper Url, User User)
        {
            return new PageViewModel
            {
                ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeAdd),
                ShowTree = User.HasPermission(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTree),
                Tree = GetDictionaryTreeModel(Url, User)
            };
        }

        public static PageViewModel.TreeModel GetDictionaryTreeModel(UrlHelper Url, User User)
        {
            return new PageViewModel.TreeModel
            {
                ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeAdd),
                ShowUpdateButton = User.HasPermission(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeUpdate),
                ShowDeleteButton = User.HasPermission(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeDelete),

                UrlList = Url.RouteUrl(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTree),
                UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeAdd),
                UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeUpdate),
                UrlNodeDragDrop = Url.RouteUrl(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeUpdateParent),
                UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeDelete),
                TreeNodes = DictionariesDataAccess.ListDictionaries().Select(Item => new PageViewModel.TreeModel.TreeNode
                {
                    DictionaryID = Item.DictionaryID,
                    ParentID = Item.ParentID,
                    Caption = Item.Caption,
                    CaptionEng = Item.CaptionEng,
                    CaptionRus = Item.CaptionRus,
                    StringCode = Item.StringCode,
                    IntCode = Item.IntCode,
                    DecimalValue = Item.DecimalValue,
                    DictionaryCode = Item.DictionaryCode,
                    SortIndex = Item.SortIndex
                }).ToList()
            };
        }

        public static string CRUD(User UserItem,byte? DatabaseAction,PageViewModel.TreeModel.TreeNode Model)
        {
            var DAL = new DictionariesDataAccess();

            DAL.DictionariesIUD(
                DatabaseAction: DatabaseAction,
                DictionaryID:Model.DictionaryID,
                ParentID: Model.ParentID ?? 0,
                Caption: Model.Caption,
                CaptionEng: Model.CaptionEng ?? "",
                CaptionRus: Model.CaptionRus ?? "",
                StringCode: Model.StringCode ?? "",
                IntCode: Model.IntCode ?? -100,
                DecimalValue: Model.DecimalValue ?? -100,
                DictionaryCode: Model.DictionaryCode,
                SortIndex: Model.SortIndex ?? -100
            );

            return Utility.GetDatabaseErrorMessage(DAL);
        }

        public static string UpdateParent(User UserItem, PageViewModel.TreeModel.TreeNode Model)
        {
            var DAL = new DictionariesDataAccess();

            DAL.DictionariesIUD(
                DatabaseAction: DatabaseActions.UPDATE,
                DictionaryID: Model.DictionaryID,
                ParentID: Model.ParentID ?? 0
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
                const string TreeName = "DictionariesTree";
                public List<TreeNode> TreeNodes { get; set; }
                #endregion

                #region Methods
                public override void InitTreeSettings(TreeListSettings Settings, HtmlHelper Html, UrlHelper Url)
                {
                    InitTreeListDefaultSettings(Settings);
                    TreeNode TreeNode;
                    Settings.Name = TreeName;
                    Settings.KeyFieldName = nameof(TreeNode.DictionaryID);
                    Settings.ParentFieldName = nameof(TreeNode.ParentID);

                    Settings.CommandColumn.Visible = true;
                    Settings.CommandColumn.NewButton.Visible = ShowAddNewButton;
                    Settings.CommandColumn.DeleteButton.Visible = ShowDeleteButton;
                    Settings.CommandColumn.EditButton.Visible = ShowUpdateButton;
                    Settings.CommandColumn.Width = Unit.Pixel(100);
                    Settings.SettingsBehavior.AutoExpandAllNodes = false;


                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(TreeNode.Caption); column.Caption = "Caption"; column.ColumnType = MVCxTreeListColumnType.TextBox; column.Width = Unit.Pixel(400);
                        var Properties = column.PropertiesEdit as TextBoxProperties;
                        InitTextEditProperties(Properties: Properties, MaxLength: 100, MakeRequired: true);
                    });                   
                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(TreeNode.StringCode); column.Caption = "String Code"; column.ColumnType = MVCxTreeListColumnType.TextBox; column.Width = Unit.Pixel(150);
                        var Properties = column.PropertiesEdit as TextBoxProperties;
                        InitTextEditProperties(Properties: Properties, MaxLength: 100);
                    });

                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(TreeNode.IntCode); column.Caption = "Int Code"; column.ColumnType = MVCxTreeListColumnType.SpinEdit; column.Width = Unit.Pixel(70);
                        var Properties = column.PropertiesEdit as SpinEditProperties;
                        InitSpinEditProperties(Properties: Properties);
                    });

                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(TreeNode.DecimalValue); column.Caption = "Decimal Val"; column.ColumnType = MVCxTreeListColumnType.SpinEdit; column.Width = Unit.Pixel(100);
                        var Properties = column.PropertiesEdit as SpinEditProperties;
                        InitSpinEditProperties(Properties: Properties, DecimalPlaces: 2);
                    });

                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(TreeNode.DictionaryCode); column.Caption = "Dictionary Code"; column.ColumnType = MVCxTreeListColumnType.SpinEdit; column.Width = Unit.Pixel(100);
                        var Properties = column.PropertiesEdit as SpinEditProperties;
                        InitSpinEditProperties(Properties: Properties, MakeRequired: true);
                    });
                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(TreeNode.SortIndex); column.Caption = "Sort Index"; column.ColumnType = MVCxTreeListColumnType.SpinEdit; column.Width = Unit.Pixel(100);
                        var Properties = column.PropertiesEdit as SpinEditProperties;
                        InitSpinEditProperties(Properties: Properties);
                    });                  

                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(TreeNode.DictionaryID); column.Caption = "ID"; column.ColumnType = MVCxTreeListColumnType.SpinEdit; //column.Width = Unit.Pixel(50);
                        var Properties = column.PropertiesEdit as SpinEditProperties;
                        InitSpinEditProperties(Properties: Properties);
                        column.SetEditCellTemplateContent(" ");
                    });

                    Settings.CommandColumn.VisibleIndex = Settings.Columns.Count;
                }
                #endregion

                #region Sub Classes
                public class TreeNode
                {
                    #region Properties

                    public int? DictionaryID { get; set; }
                    public int? ParentID { get; set; }
                    public string Caption { get; set; }
                    public string CaptionEng { get; set; }
                    public string CaptionRus { get; set; }
                    public string StringCode { get; set; }
                    public int? IntCode { get; set; }
                    public decimal? DecimalValue { get; set; }
                    public short? DictionaryCode { get; set; }
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