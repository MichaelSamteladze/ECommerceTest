using Core;
using Core.Enums;
using Core.Properties;
using Core.Services;
using Core.Utilities;
using Reusables.Core;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerceTest.Areas.Admin.Models
{
    public class CategoriesTreeModel
    {
        #region Methods
        public static AjaxResponse CategoryCreate(int? ParentID, string CategoryCaption, UrlHelper Url, User UserItem, Controller Controller)
        {
            var DAL = new CategoriesDataAccess();
            TreeNodeItem Node = null;

            var CategoryID = DAL.CategoriesIUD(
                DatabaseAction: DatabaseActions.CREATE,
                CategorySlug: Guid.NewGuid().ToString(),
                CategoryCaption: CategoryCaption,
                IsPublished: false
            );

            if (CategoryID > 0)
            {
                Node = new TreeNodeItem
                {
                    NodeID = CategoryID,
                    Caption = CategoryCaption,
                    ShowToggler1 = true,
                    NavigateUrl = Url.RouteUrl(ControllerActionRouteNames.Admin.Categories.Category.Properties, new { CategoryID }),
                    ShowAddNewButton = true,
                    ShowDeleteButton = true,
                    IsToggler1Checked = false
                };
            }

            var AR = new AjaxResponse();

            if (Node != null)
            {
                AR.IsSuccess = true;
                AR.Data = LocalUtilities.RenderPartialViewToString(Controller, ViewNames.Shared.FileTreeEditor.File, Node);
            }

            return AR;
        }

        public static AjaxResponse DeleteCategory(string Hash)
        {
            var AR = new AjaxResponse();
            var CategoryID = TreeNodeItem.DecodeHash(Hash)?.Key;
            var Category = CategoriesDataAccess.GetSingleCategoryByID(CategoryID);
            
            

            var DAL = new CategoriesDataAccess();
            DAL.CategoriesIUD(
                DatabaseAction: DatabaseActions.DELETE,
                CategoryID: CategoryID
            );

            if(DAL.IsError)
            {
                AR.Data = DAL.IsClient ? DAL.ErrorMessage : Resources.TextError;
            }
            else
            {
                AR.IsSuccess = true;
                Utility.DeleteUploadedFile(Category?.CategoryImageFilename);
            }

            return AR;
        }

        public static AjaxResponse CategoryUpdate(int? CategoryID, string CategoryCaption = null, bool? IsPublished = null)
        {
            var DAL = new CategoriesDataAccess();
            DAL.CategoriesIUD(
                DatabaseAction: DatabaseActions.UPDATE,
                CategoryID: CategoryID,
                CategoryCaption: CategoryCaption,
                IsPublished: IsPublished
            );
            return new AjaxResponse
            {
                IsSuccess = !DAL.IsError
            };
        }

        public static PageViewModel GetCategoriesPageViewModel(UrlHelper Url, User UserItem)
        {
            var Categories = CategoriesDataAccess.ListCategories()?.Select(Item => new TreeNodeItem
            {
                NodeID = Item.CategoryID,
                NavigateUrl = Url.RouteUrl(ControllerActionRouteNames.Admin.Categories.Category.Properties, new { Item.CategoryID }),
                Caption = Item.CategoryCaption,
                IsToggler1Checked = Item.IsPublished,
                ShowAddNewButton = true,
                ShowDeleteButton = true,
                ShowToggler1 = true
            }).ToList();

            return new PageViewModel
            {
                ShowAddNewButton = true,
                UrlCreateNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Categories.AddNew),
                UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Categories.Update),
                UrlSyncParentsAndSortIndexes = Url.RouteUrl(ControllerActionRouteNames.Admin.Categories.SyncParentsAndSortIndexes),
                UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Categories.Delete),
                Categories = TreeNodeItem.ConvertToRecursive(Categories)
            };
        }

        public static AjaxResponse SyncParentsAndSortIndexes(SyncSortIndexesModel SubmitModel)
        {            
            var DAL = new CategoriesDataAccess();
            DAL.CategoriesSyncParentsAndSortIndexes(SubmitModel.SortIndexes);

            return new AjaxResponse
            {
                IsSuccess = !DAL.IsError
            };
        }
        #endregion

        #region Sub Classes
        public class PageViewModel
        {
            #region Properties
            public bool HasCategories => Categories != null && Categories.Count > 0;
            public List<TreeNodeItem> Categories { get; set; }
            public bool ShowAddNewButton { get; set; }

            #region Urls
            public string UrlCreateNew { get; set; }
            public string UrlUpdate { get; set; }
            public string UrlSyncParentsAndSortIndexes { get; set; }
            public string UrlDelete { get; set; }
            #endregion

            #region Texts
            public string TextConfirmDeleteRecord { get; set; } = Resources.TextConfirmDelete;
            public string ValidationRequiredCategoryCaption { get; set; } = Resources.ValidationRequiredCategoryCaption;
            #endregion
            #endregion
        }
        #endregion
    }

    public class CategoryModel
    {
        #region Methods
        public static AjaxResponse DeleteImage(int? CategoryID)
        {
            var Category = CategoriesDataAccess.GetSingleCategoryByID(CategoryID);
            Utility.DeleteUploadedFile(Category.CategoryImageFilename);

            var DAL = new CategoriesDataAccess();
            DAL.CategoriesIUD(
                DatabaseAction: DatabaseActions.UPDATE,
                CategoryID: CategoryID,
                CategoryImageFilename: string.Empty
                );
            return new AjaxResponse
            {
                IsSuccess = !DAL.IsError
            };
        }

        public static PageViewModel GetPageViewModel(int? CategoryID, PageViewModel Model, UrlHelper Url, ViewDataDictionary ViewData)
        {
            var C = CategoriesDataAccess.GetSingleCategoryByID(CategoryID);
            if (C == null)
            {
                Model = null;
            }
            else
            {
                if (Model == null)
                {
                    Model = new PageViewModel
                    {
                        CategorySlug = C.CategorySlug,
                        IsPublished = C.IsPublished,
                        CategoryTheme = C.CategoryTheme,
                        CategoryCaption = C.CategoryCaption,
                        CategoryDescription = C.CategoryDescription,
                        CategoryImageFilename = C.CategoryImageFilename
                    };
                }

                Model.PageTitle = C.CategoryCaption;                    
                Model.UrlCategoryImageDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Categories.Category.DeleteImage);

                Model.CategoryThemeList = Enumerable.Range(0, 8).Select(Item => new PageViewModel.CategoryThemeItem
                {
                    ThemeCode = Item,
                    ThemeImageUrl = $"~/Content/Images/themes/g-{Item}.png"
                }).ToList();
            }

            return Model;
        }

        public static PageViewModel SaveCategory(int? CategoryID, PageViewModel Model)
        {
            var DBItem = CategoriesDataAccess.GetSingleCategoryByID(CategoryID);

            var CategoryImageFilenameNew = default(string);
            var HasPostedFile = Model.CategoryImagePostedFile?.ContentLength > 0;

            if(HasPostedFile)
            {
                CategoryImageFilenameNew= Utility.GetFilenameFromUploadedFile(Model.CategoryImagePostedFile);
                Utility.DeleteUploadedFile(DBItem.CategoryImageFilename);
            }
                
            var DAL = new CategoriesDataAccess();
            DAL.CategoriesIUD(
                DatabaseAction: DatabaseActions.UPDATE,
                CategoryID: CategoryID,
                CategoryTheme: Model.CategoryTheme,
                CategorySlug: Model.CategorySlug,
                IsPublished: Model.IsPublished,
                CategoryCaption: Model.CategoryCaption,
                CategoryDescription: Model.CategoryDescription,
                CategoryImageFilename: CategoryImageFilenameNew
            );

            Model.Form.IsSaved = !DAL.IsError;

            if (HasPostedFile)
            {
                Utility.SaveUploadedFile(Model.CategoryImagePostedFile, CategoryImageFilenameNew);
            }

            return Model;
        }

        public static void ValidateCategoryViewModel(int? CategoryID, PageViewModel Model)
        {
            Model.Form.Errors = new List<SimpleKeyValue<string, string>>
            {
                Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.CategoryCaption)}]", ValueToValidate: Model.CategoryCaption),
                Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.CategorySlug)}]", ValueToValidate:Model.CategorySlug),
                Validation.Validate(
                    ErrorAction:() =>
                    {
                        return !string.IsNullOrWhiteSpace(Model.CategorySlug) && !CategoriesDataAccess.IsCategorySlugUniq(Model.CategorySlug, CategoryID);
                    },
                    ErrorKey: $"[name={nameof(Model.CategorySlug)}]",
                    ErrorMessage:Resources.TextSlugNotUniq
                )
            };

            Model.Form.Errors.RemoveAll(Item => Item == null);
        }
        #endregion

        #region Sub Classes
        public class PageViewModel : Category
        {
            #region Properties                        
            public string PageTitle { get; set; }
            public FormViewModelBase Form { get; set; } = new FormViewModelBase();
            public HttpPostedFileBase CategoryImagePostedFile { get; set; }
            public string CategoryImageHttpPath => Utility.GetUploadedFileHttpPath(CategoryImageFilename);
            public bool HasCategoryImage => !string.IsNullOrWhiteSpace(CategoryImageFilename);
            public string UrlCategoryImageDelete { get; set; }
            public List<CategoryThemeItem> CategoryThemeList { get; set; }
            public int SelectedThemeIndex => CategoryTheme == null ? 0 : CategoryTheme.Value;
            #endregion

            #region Sub Classes
            public class CategoryThemeItem
            {
                #region Properties
                public int? ThemeCode { get; set; }
                public string ThemeImageUrl { get; set; }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}