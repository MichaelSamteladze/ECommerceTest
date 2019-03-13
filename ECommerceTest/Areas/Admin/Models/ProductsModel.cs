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
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ECommerceTest.Areas.Admin.Models
{
    public class ProductsGridModel
    {
        #region Methods
        public static PageViewModel GetProductsPageViewModel(UrlHelper Url, User User)
        {
            return new PageViewModel
            {
                ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.Products.ProductsGridAdd),
                ShowGrid = User.HasPermission(ControllerActionRouteNames.Admin.Products.ProductsGrid),
                Grid = GetProductsGridModel(Url, User)
            };
        }

        public static PageViewModel.GridModel GetProductsGridModel(UrlHelper Url, User User, string ErrorMessage = null)
        {
            return new PageViewModel.GridModel
            {
                ErrorMessage = ErrorMessage,
                UrlList = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.ProductsGrid),
                UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.ProductsGridAdd),
                UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.ProductsGridUpdate),
                UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.ProductsGridDelete),
                GridItems = ProductsDataAccess.ListProducts()?.Select(Item => new PageViewModel.GridModel.GridItem
                {
                    ProductID = Item.ProductID,
                    ProductPrice = Item.ProductPrice,
                    ProductCaption = Item.ProductCaption,
                    CategoryID = Item.CategoryID,
                    ProductImageFilename = Item.ProductImageFilename,
                    IsPublished = Item.IsPublished
                }).ToList()
            };
        }

        public static string ProductsIUD(byte? DatabaseAction, PageViewModel.GridModel.GridItem Model)
        {
            var DAL = new ProductsDataAccess();
            var Product = ProductsDataAccess.GetSingleProductByID(Model.ProductID);
            
            DAL.ProductsIUD(
                DatabaseAction: DatabaseAction,
                ProductID: Model.ProductID,
                ProductPrice: Model.ProductPrice,
                ProductCaption: Model.ProductCaption,
                CategoryID: Model.CategoryID,
                IsPublished: Model.IsPublished
            );

            if(!DAL.IsError && DatabaseAction == DatabaseActions.DELETE)
            {
                Utility.DeleteUploadedFile(Product.ProductImageFilename);
            }

            return Utility.GetDatabaseErrorMessage(DAL);
        }
        #endregion

        #region Sub Classes
        public class PageViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public bool ShowGrid { get; set; }
            public GridModel Grid { get; set; }            
            #endregion

            #region Sub Classes
            public class GridModel : DevexpressGridViewModelBase
            {
                #region Properties
                const string GridName = "ProductsGrid";
                public List<GridItem> GridItems { get; set; }
                public bool ShowCommandColumn => ShowUpdateButton && ShowDeleteButton;
                #endregion

                #region Methods
                public override void InitGridSettings(GridViewSettings Settings, HtmlHelper Html, UrlHelper Url)
                {
                    var Categories = CategoriesDataAccess.ListCategoriesAsKeyValue();

                    InitGridViewDefaultSettings(Settings);
                    GridItem GridItem;

                    Settings.Name = GridName;
                    Settings.KeyFieldName = nameof(GridItem.ProductID);
                    Settings.SettingsPager.PageSize = 30;

                    Settings.CommandColumn.Visible = ShowCommandColumn;
                    Settings.CommandColumn.VisibleIndex = 5;
                    Settings.CommandColumn.Width = Unit.Pixel(80);
                    Settings.CommandColumn.ShowDeleteButton =
                    Settings.CommandColumn.ShowEditButton = true;
                    
                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(GridItem.ProductCaption); column.Caption = "Product"; column.Width = Unit.Pixel(300); column.ColumnType = MVCxGridViewColumnType.TextBox;
                        var Properties = column.PropertiesEdit as TextBoxProperties;
                        InitTextEditProperties(Properties: Properties, MaxLength: 100, MakeRequired: true);
                    });                    
                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(GridItem.ProductPrice); column.Caption = "Price"; column.Width = Unit.Pixel(80); column.ColumnType = MVCxGridViewColumnType.SpinEdit;
                        var Properties = column.PropertiesEdit as SpinEditProperties;
                        InitSpinEditProperties(Properties: Properties, DecimalPlaces: 2);
                    });
                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(GridItem.CategoryID); column.Caption = "Category"; column.Width = Unit.Pixel(250); column.ColumnType = MVCxGridViewColumnType.ComboBox;
                        var Properties = column.PropertiesEdit as ComboBoxProperties;
                        InitComboBoxProperties(Properties: Properties, DataSource: Categories, MakeRequired: true);

                    });
                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(GridItem.IsPublished); column.Caption = "Published"; column.Width = Unit.Pixel(80); column.ColumnType = MVCxGridViewColumnType.CheckBox;
                        var Properties = column.PropertiesEdit as CheckBoxProperties;
                        InitCheckBoxProperties(Properties: Properties, GridName: GridName);
                    });
                    Settings.Columns.Add(column =>
                    {
                        column.Width = Unit.Pixel(100);
                        column.SetEditItemTemplateContent(" ");

                        column.SetDataItemTemplateContent(c =>
                        {
                            var ProductID = DataBinder.Eval(c.DataItem, nameof(GridItem.ProductID));
                            var PropertiesUrl = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.Product.Properties, new { ProductID = ProductID });
                            Html.ViewContext.Writer.Write($"<a href=\"{PropertiesUrl}\"><i class=\"fa fa-info-circle\"></i> Details </a>");
                        });
                    });


                    Settings.Columns.Add(column => { column.SetEditItemTemplateContent(" "); });
                }
                #endregion

                #region Sub Classes
                public class GridItem : Product
                {
                    #region Properties
                    #endregion
                }
                #endregion

            }
            #endregion
        }
        #endregion
    }

    public class ProductModel
    {
        #region Methods
        public static AjaxResponse DeleteImage(int? ProductID)
        {
            var Product = ProductsDataAccess.GetSingleProductByID(ProductID);
            Utility.DeleteUploadedFile(Product.ProductImageFilename);

            var AR = new AjaxResponse();
            var DAL = new ProductsDataAccess();
            DAL.ProductsIUD(
                DatabaseAction: DatabaseActions.UPDATE,
                ProductID: ProductID,
                ProductImageFilename: string.Empty
                );

            AR.IsSuccess = !DAL.IsError;

            return AR;
        }

        public static PageViewModel GetPageViewModel(int? ProductID, PageViewModel Model, UrlHelper Url, ViewDataDictionary ViewData)
        {
            var DBItem = ProductsDataAccess.GetSingleProductByID(ProductID);
            if (DBItem == null)
            {
                Model = null;
            }
            else
            {
                if (Model == null)
                {
                    Model = new PageViewModel
                    {
                        ProductID = DBItem.ProductID,
                        ProductSlug = DBItem.ProductSlug,
                        ProductCaption = DBItem.ProductCaption,
                        CategoryID = DBItem.CategoryID,
                        ProductPrice = DBItem.ProductPrice,
                        ProductOldPrice = DBItem.ProductOldPrice,
                        ProductDescription = DBItem.ProductDescription,
                        ProductImageFilename = DBItem.ProductImageFilename,
                        IsPublished = DBItem.IsPublished
                    };
                }
                Model.PageTitle = DBItem.ProductCaption;
                Model.UrlProductImageDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.Product.DeleteImage);
                Model.Categories = CategoriesDataAccess.ListCategoriesAsKeyValue();
                Model.Categories.ForEach(Item =>
                {
                    Item.IsSelected = Model.CategoryID == Item.Key;
                });
            }

            return Model;
        }

        public static PageViewModel SaveProduct(int? ProductID, PageViewModel Model)
        {
            var DBItem = ProductsDataAccess.GetSingleProductByID(ProductID);
            if (DBItem == null)
            {
                Model = null;
            }
            else
            {
                var ProductImageFilenameNew = default(string);
                var HasPostedFile = Model.ProductImagePostedFile?.ContentLength > 0;

                if (HasPostedFile)
                {
                    ProductImageFilenameNew = Utility.GetFilenameFromUploadedFile(Model.ProductImagePostedFile);
                    Utility.DeleteUploadedFile(DBItem.ProductImageFilename);
                }

                var DAL = new ProductsDataAccess();
                DAL.ProductsIUD(
                    DatabaseAction: DatabaseActions.UPDATE,
                    ProductID: ProductID,
                    ProductSlug: Model.ProductSlug,
                    ProductCaption: Model.ProductCaption,
                    CategoryID: Model.CategoryID,
                    ProductPrice: Model.ProductPrice,
                    ProductOldPrice: Model.ProductOldPrice,
                    ProductDescription: Model.ProductDescription ?? string.Empty,
                    ProductImageFilename: ProductImageFilenameNew,
                    IsPublished: Model.IsPublished
                );

                Model.Form.IsSaved = !DAL.IsError;

                if (HasPostedFile)
                {
                    Utility.SaveUploadedFile(Model.ProductImagePostedFile, ProductImageFilenameNew);
                }
            }
            return Model;
        }

        public static void ValidatePageViewModel(int? ProductID, PageViewModel Model)
        {
            Model.Form.Errors = new List<SimpleKeyValue<string, string>>
            {
                Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.ProductCaption)}]", ValueToValidate: Model.ProductCaption),
                Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.CategoryID)}]", ValueToValidate: Model.CategoryID),
                Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.ProductSlug)}]", ValueToValidate: Model.ProductSlug),
                Validation.Validate(
                    ErrorAction: () =>
                    {
                        return !string.IsNullOrWhiteSpace(Model.ProductSlug) && !ProductsDataAccess.IsProductSlugUniq(Model.ProductSlug, ProductID);
                    },
                    ErrorKey:$"[name={nameof(Model.ProductSlug)}]",
                    ErrorMessage: Resources.TextSlugNotUniq
                )
            };

            Model.Form.Errors.RemoveAll(Item => Item == null);
        }        
        #endregion

        #region Sub Classes
        public class PageViewModel : Product
        {
            #region Properties
            public string PageTitle { get; set; }
            public FormViewModelBase Form { get; set; } = new FormViewModelBase();
            public List<SimpleKeyValue<int?, string>> Categories { get; set; }
            public HttpPostedFileBase ProductImagePostedFile { get; set; }
            public string ProductImageHttpPath => Utility.GetUploadedFileHttpPath(ProductImageFilename);
            public bool HasProductImage => !string.IsNullOrWhiteSpace(ProductImageFilename);
            public string UrlProductImageDelete { get; set; }
            #endregion
        }
        #endregion
    }
}