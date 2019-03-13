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
    public class CarouselGridModel
    {
        #region Methods
        public static PageViewModel GetCarouselPageViewModel(UrlHelper Url, User User)
        {
            return new PageViewModel
            {
                ShowGrid = User.HasPermission(ControllerActionRouteNames.Admin.Carousel.Grid),
                ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.Carousel.GridAdd),
                Grid = GetCarouselGridModel(Url, User),
                UrlSyncSortIndexes = Url.RouteUrl(ControllerActionRouteNames.Admin.Carousel.GridSyncSortIndexes)
            };
        }

        public static PageViewModel.GridModel GetCarouselGridModel(UrlHelper Url, User User, string ErrorMessage = null)
        {
            return new PageViewModel.GridModel
            {
                ErrorMessage = ErrorMessage,

                UrlList = Url.RouteUrl(ControllerActionRouteNames.Admin.Carousel.Grid),
                UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Carousel.GridAdd),
                UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Carousel.GridUpdate),
                UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Carousel.GridDelete),
                UrlCustomAction = Url.RouteUrl(ControllerActionRouteNames.Admin.Carousel.GridSyncSortIndexes),

                GridItems = CarouselDataAccess.ListCarouselItems()?.Select(Item => new PageViewModel.GridModel.GridItem
                {
                    CarouselID = Item.CarouselID,
                    CarouselCaption = Item.CarouselCaption,
                    IsPublished = Item.IsPublished
                }).ToList()
            };
        }

        public static string CarouselIUD(byte? DatabaseAction, PageViewModel.GridModel.GridItem Model)
        {
            if (DatabaseAction == DatabaseActions.DELETE)
            {
                var CarouselItem = CarouselDataAccess.GetSingleCarouselItemByID(Model.CarouselID);
                Utility.DeleteUploadedFile(CarouselItem.CarouselImageFilename);
            }

            var DAL = new CarouselDataAccess();
            DAL.CarouselIUD(
                DatabaseAction: DatabaseAction,
                CarouselID: Model.CarouselID,
                CarouselCaption: Model.CarouselCaption,
                IsPublished: Model.IsPublished
            );

            return Utility.GetDatabaseErrorMessage(DAL);
        }

        public static AjaxResponse SyncSortIndexes(SyncSortIndexesModel SubmitModel)
        {
            var DAL = new CarouselDataAccess();
            DAL.SyncSortIndexes(SubmitModel.SortIndexes);
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
            public bool ShowGrid { get; set; }
            public bool ShowAddNewButton { get; set; }
            public GridModel Grid { get; set; }
            public string UrlSyncSortIndexes { get; set; }
            #endregion

            #region Sub Classes
            public class GridModel : DevexpressGridViewModelBase
            {
                #region Properties
                const string GridName = "CarouselGrid";
                public List<GridItem> GridItems { get; set; }
                #endregion

                #region Methods
                public override void InitGridSettings(GridViewSettings Settings, HtmlHelper Html, UrlHelper Url)
                {
                    GridItem GridItem;
                    InitGridViewDefaultSettings(Settings);
                    Settings.Name = GridName;
                    Settings.KeyFieldName = nameof(GridItem.CarouselID);

                    Settings.CommandColumn.Visible = true;
                    Settings.CommandColumn.Width = Unit.Pixel(80);
                    Settings.CommandColumn.ShowDeleteButton = true;
                    Settings.CommandColumn.ShowEditButton = true;
                    Settings.CommandColumn.VisibleIndex = 1;

                    Settings.ClientSideEvents.EndCallback =
                    Settings.ClientSideEvents.Init = "function(s,e){ CarouselGridModel.ApplySortable(); }";

                    Settings.Columns.Add(column =>
                    {
                        column.Width = Unit.Pixel(30);
                        column.SetEditItemTemplateContent(" ");
                        column.SetDataItemTemplateContent(c => { Html.ViewContext.Writer.Write($"<span class=\"fas fa-arrows-alt js-drag-me\" data-id=\"{DataBinder.Eval(c.DataItem, nameof(GridItem.CarouselID))}\"></span>"); });
                    });

                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(GridItem.CarouselCaption); column.Caption = "Caption"; column.Width = Unit.Pixel(150); column.ColumnType = MVCxGridViewColumnType.TextBox;
                        var Properties = column.PropertiesEdit as TextBoxProperties;
                        InitTextEditProperties(Properties: Properties, MaxLength: 20, MakeRequired: true);
                    });                    

                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(GridItem.IsPublished); column.Caption = "Published"; column.Width = Unit.Pixel(80); column.ColumnType = MVCxGridViewColumnType.CheckBox;
                        var Properties = column.PropertiesEdit as CheckBoxProperties;
                        InitCheckBoxProperties(Properties: Properties, GridName: GridName);
                    });
                    Settings.Columns.Add(column =>
                    {
                        column.Width = Unit.Pixel(140);
                        column.SetEditItemTemplateContent(" ");

                        column.SetDataItemTemplateContent(c =>
                        {
                            var CarouselID = DataBinder.Eval(c.DataItem, nameof(GridItem.CarouselID));
                            var PropertiesUrl = Url.RouteUrl(ControllerActionRouteNames.Admin.Carousel.CarouselItem.Properties, new { CarouselID = CarouselID });
                            Html.ViewContext.Writer.Write($"<a href=\"{PropertiesUrl}\"><i class=\"fa fa-info-circle\"></i> Details </a>");
                        });
                    });
                    Settings.Columns.Add(column => { column.SetEditItemTemplateContent(" "); });
                    
                }
                #endregion

                #region Sub Classes
                public class GridItem : CarouselItem
                {
                    #region Porperties
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }

    public class CarouselItemModel
    {
        #region Sub Classes
        public class Properties
        {
            #region Methods
            public static AjaxResponse DeleteImage(int? CarouselID)
            {
                var CarouselItem = CarouselDataAccess.GetSingleCarouselItemByID(CarouselID);
                Utility.DeleteUploadedFile(CarouselItem.CarouselImageFilename);

                var AR = new AjaxResponse();
                var DAL = new CarouselDataAccess();
                DAL.CarouselIUD(
                    DatabaseAction: DatabaseActions.UPDATE,
                    CarouselID: CarouselID,
                    CarouselImageFilename: string.Empty
                    );

                AR.IsSuccess = !DAL.IsError;

                return AR;
            }

            public static PageViewModel GetPageViewModel(int? CarouselID, PageViewModel Model, UrlHelper Url, ViewDataDictionary ViewData)
            {
                var C = CarouselDataAccess.GetSingleCarouselItemByID(CarouselID);
                if (C == null)
                {
                    Model = null;
                }
                else
                {
                    LocalUtilities.UpdateBreadCrumbsItem<AdminLayoutModel>(ViewData, C.CarouselCaption, true);

                    if (Model == null)
                    {
                        Model = new PageViewModel
                        {
                            CarouselCaption = C.CarouselCaption,
                            CarouselText = C.CarouselText,
                            CarouselUrl = C.CarouselUrl,
                            CarouselImageFilename = C.CarouselImageFilename,
                            IsPublished = C.IsPublished
                        };
                    }
                    
                    Model.UrlCarouselImageDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Carousel.CarouselItem.DeleteImage);
                }

                return Model;
            }

            public static PageViewModel SaveCarouselItem(int? CarouselID, PageViewModel Model)
            {
                var DBItem = CarouselDataAccess.GetSingleCarouselItemByID(CarouselID);

                var HasPostedFile = Model.CarouselImagePostedFile?.ContentLength > 0;
                var CarouselImageFilenameNew = default(string);
                if (HasPostedFile)
                {
                    CarouselImageFilenameNew = Utility.GetFilenameFromUploadedFile(Model.CarouselImagePostedFile);
                    Utility.DeleteUploadedFile(DBItem.CarouselImageFilename);
                }

                var DAL = new CarouselDataAccess();
                DAL.CarouselIUD(
                    DatabaseAction: DatabaseActions.UPDATE,
                    CarouselID: CarouselID,
                    CarouselCaption: Model.CarouselCaption,
                    CarouselText: Model.CarouselText ?? string.Empty,
                    CarouselUrl: Model.CarouselUrl ?? string.Empty,
                    CarouselImageFilename: CarouselImageFilenameNew,
                    IsPublished: Model.IsPublished
                );

                Model.Form.IsSaved = !DAL.IsError;

                if (HasPostedFile)
                {
                    Utility.SaveUploadedFile(Model.CarouselImagePostedFile, CarouselImageFilenameNew);
                }

                return Model;
            }

            public static void ValidateCarouselItemPageViewModel(PageViewModel Model)
            {
                Model.Form.Errors = new List<SimpleKeyValue<string, string>>
                {
                    Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.CarouselCaption)}]", ValueToValidate: Model.CarouselCaption)
                };
                Model.Form.Errors.RemoveAll(Item => Item == null);
            }
            #endregion

            #region Sub Classes
            public class PageViewModel : CarouselItem
            {
                #region Properies
                public FormViewModelBase Form { get; set; } = new FormViewModelBase();
                public HttpPostedFileBase CarouselImagePostedFile { get; set; }
                public string CarouselImageHttpPath => Utility.GetUploadedFileHttpPath(CarouselImageFilename);
                public bool HasCarouselImage => !string.IsNullOrWhiteSpace(CarouselImageFilename);
                public string UrlCarouselImageDelete { get; set; }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}