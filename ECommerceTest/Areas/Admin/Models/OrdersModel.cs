using Core;
using Core.Reporting;
using Core.Utilities;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using Reusables.Core;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Core.Utilities.Constants;

namespace ECommerceTest.Areas.Admin.Models
{
    public class OrdersGridModel
    {
        #region Methods

        public static PageViewModel GetOrdersPageViewModel(UrlHelper Url, User User)
        {
            return new PageViewModel
            {
                ShowGrid = User.HasPermission(ControllerActionRouteNames.Admin.Orders.Grid),
                Grid = GetOrdersGridModel(Url, User)
            };
        }

        public static PageViewModel.GridModel GetOrdersGridModel(UrlHelper Url, User User, string ErrorMessage = null)
        {
            return new PageViewModel.GridModel
            {
                ErrorMessage = ErrorMessage,
                ShowUpdateButton = User.HasPermission(ControllerActionRouteNames.Admin.Orders.GridUpdate),

                UrlList = Url.RouteUrl(ControllerActionRouteNames.Admin.Orders.Grid),
                UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Orders.GridUpdate),
                GridItems = OrdersDataAccess.ListOrders()?.Select(Item => new PageViewModel.GridModel.GridItem
                {
                    OrderID = Item.OrderID,
                    OrderStatusID = Item.OrderStatusID,
                    OrderTotalPrice = Item.OrderTotalPrice,
                    OrderPaymentDateTime = Item.OrderPaymentDateTime,
                    IsOrderDelivered = Item.IsOrderDelivered,
                    PaymentOptionID = Item.PaymentOptionID,
                    Fullname = Item.Fullname
                }).ToList()
            };
        }

        public static string OrderUpdate(PageViewModel.GridModel.GridItem Model, UrlHelper Url)
        {
            var DAL = new OrdersDataAccess();
            var Order = OrdersDataAccess.GetSingleOrderByID(Model.OrderID);

            DAL.OrdersUpdate(
                OrderID: Model.OrderID,
                OrderStatusID: Model.OrderStatusID,
                IsOrderDelivered: Model.IsOrderDelivered
            );

            return Utility.GetDatabaseErrorMessage(DAL);
        }

        public static byte[] GetInvoiceReport(int? OrderID)
        {
            var Report = new InvoiceReport1(OrderID);
            MemoryStream MS = new MemoryStream();
            Report.ExportToPdf(MS);
            byte[] bytesInStream = MS.ToArray();
            MS.Close();

            return bytesInStream;
        }

        #endregion

        #region Sub Classes
        public class PageViewModel
        {
            #region Properties
            public bool ShowGrid { get; set; }
            public GridModel Grid { get; set; }
            #endregion

            #region Sub Classes
            public class GridModel : DevexpressGridViewModelBase
            {
                #region Properties
                const string GridName = "OrdersGrid";
                public List<GridItem> GridItems { get; set; }
                public bool ShowCommandColumn => ShowUpdateButton;
                #endregion

                #region Methods
                public override void InitGridSettings(GridViewSettings Settings, HtmlHelper Html, UrlHelper Url)
                {
                    //var OrderStatuses = DictionariesDataAccess.ListDictionaryItemsAsKeyValueByDictionaryCodename(Dictionaries.Codenames.OrderStatuses, new MemoryCacheAssistance()).ToList();
                    var PaymentMethods = DictionariesDataAccess.ListDictionaryItemsAsKeyValueByDictionaryCodename(Dictionaries.Codenames.PaymentOptions, new MemoryCacheAssistance()).ToList();

                    InitGridViewDefaultSettings(Settings);
                    GridItem GridItem;

                    Settings.Name = GridName;
                    Settings.KeyFieldName = nameof(GridItem.OrderID);
                    Settings.SettingsPager.PageSize = 100;

                    Settings.CommandColumn.Visible = ShowCommandColumn;
                    Settings.CommandColumn.Width = Unit.Pixel(80);
                    Settings.CommandColumn.ShowDeleteButton = ShowDeleteButton;
                    Settings.CommandColumn.ShowEditButton = ShowUpdateButton;
                    

                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(GridItem.OrderID); column.Caption = "Order ID"; column.Width = Unit.Pixel(80); column.ColumnType = MVCxGridViewColumnType.TextBox;
                        var TextBoxProperties = column.PropertiesEdit as TextBoxProperties;
                        InitTextEditProperties(Properties: TextBoxProperties, MaxLength: 50, MakeRequired: true);
                        
                        column.SetDataItemTemplateContent(c =>
                        {
                            var OrderID = DataBinder.Eval(c.DataItem, nameof(GridItem.OrderID));
                            var DetailsUrl = Url.RouteUrl(ControllerActionRouteNames.Admin.Orders.OrderDetails.Index, new { OrderID = OrderID });
                            Html.ViewContext.Writer.Write($"<a href=\"{DetailsUrl}\"> {OrderID} <i class=\"fa fa-info-circle\"></i> </a>");
                        });

                        column.SetEditItemTemplateContent(c =>
                        {
                            var Template = DataBinder.Eval(c.DataItem, nameof(GridItem.OrderID));
                            Html.ViewContext.Writer.Write($"{Template}");
                        });
                    });
                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(GridItem.Fullname); column.Caption = "Customer"; column.Width = Unit.Pixel(200); column.ColumnType = MVCxGridViewColumnType.TextBox;
                        var TextBoxProperties = column.PropertiesEdit as TextBoxProperties;
                        InitTextEditProperties(Properties: TextBoxProperties, MaxLength: 50, MakeRequired: true);
                        column.SetEditItemTemplateContent(c =>
                        {
                            var Template = DataBinder.Eval(c.DataItem, nameof(GridItem.Fullname));
                            Html.ViewContext.Writer.Write($"{Template}");
                        });
                    });                    
                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(GridItem.OrderTotalPrice); column.Caption = "Price"; column.ColumnType = MVCxGridViewColumnType.SpinEdit; column.Width = Unit.Pixel(100);
                        var SpinEditProperties = column.PropertiesEdit as SpinEditProperties;
                        InitSpinEditProperties(Properties: SpinEditProperties, DecimalPlaces: 2, MakeRequired: false);
                        column.SetEditItemTemplateContent(c =>
                        {
                            var Template = DataBinder.Eval(c.DataItem, nameof(GridItem.OrderTotalPrice));
                            Html.ViewContext.Writer.Write($"{Template}");
                        });
                    });
                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(GridItem.OrderPaymentDateTime); column.Caption = "Order Date"; column.Width = Unit.Pixel(150); column.ColumnType = MVCxGridViewColumnType.DateEdit;
                        var DateEditProperties = column.PropertiesEdit as DateEditProperties;
                        InitDateEditProperties(Properties: DateEditProperties, UseDateTimeFormat: true, MakeRequired: false);
                        column.SetEditItemTemplateContent(c =>
                        {
                            var Template = DataBinder.Eval(c.DataItem, nameof(GridItem.OrderPaymentDateTime));
                            Html.ViewContext.Writer.Write($"{string.Format(Formats.DateTimeEval, Template)}");
                        });
                    });
                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(GridItem.IsOrderDelivered); column.Caption = "Delivered"; column.Width = Unit.Pixel(80); column.ColumnType = MVCxGridViewColumnType.CheckBox;
                        var Properties = column.PropertiesEdit as CheckBoxProperties;
                        InitCheckBoxProperties(Properties: Properties, GridName: GridName);
                    });
                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(GridItem.PaymentOptionID); column.Caption = "Payment Option"; column.Width = Unit.Pixel(150); column.ColumnType = MVCxGridViewColumnType.ComboBox;
                        var ComboBoxProperties = column.PropertiesEdit as ComboBoxProperties;
                        InitComboBoxProperties(Properties: ComboBoxProperties, DataSource: PaymentMethods);
                        column.SetEditItemTemplateContent(c =>
                        {
                            var PaymentOptionID = DataBinder.Eval(c.DataItem, nameof(GridItem.PaymentOptionID));
                            var Template = PaymentMethods.Where(Item => Item.Key == Convert.ToInt32(PaymentOptionID)).FirstOrDefault().Value;
                            Html.ViewContext.Writer.Write($"{Template}");
                        });
                    });

                    Settings.Columns.Add(column =>
                    {
                        column.Width = Unit.Pixel(140);
                        column.SetEditItemTemplateContent(" ");

                        column.SetDataItemTemplateContent(c =>
                        {
                            var OrderID = DataBinder.Eval(c.DataItem, nameof(GridItem.OrderID));
                            var InvoiceUrl = Url.RouteUrl(ControllerActionRouteNames.Admin.Orders.InviceReport, new { OrderID = OrderID });
                            Html.ViewContext.Writer.Write($"<a href=\"{InvoiceUrl}\" target=\"_blank\"><i class=\"fa fa-file-pdf-o\"></i> invoice</a>");
                        });
                    });

                    Settings.Columns.Add(column => { column.SetEditItemTemplateContent(" "); });
                }
                #endregion

                #region Sub Classes
                public class GridItem : Order
                {
                    #region Properties
                    public string Fullname { get; set; }
                    #endregion
                }
                #endregion

            }
            #endregion
        }
        #endregion
    }

    public class OrderDetailsModel
    {
        #region Methods

        public static PageViewModel GetOrderDetailsPageViewModel(UrlHelper Url, User User, ViewDataDictionary ViewData, int? OrderID)
        {
            var Order = OrdersDataAccess.GetSingleOrderByID(OrderID);
            LocalUtilities.UpdateBreadCrumbsItem<AdminLayoutModel>(ViewData, OrderID.ToString(), true);
            return new PageViewModel
            {
                ShowGrid = User.HasPermission(ControllerActionRouteNames.Admin.Orders.OrderDetails.Grid),
                Grid = GetOrderDetailsGridModel(Url, User, Order),
                ShippingFirstname = Order.ShippingFirstname,
                ShippingLastname = Order.ShippingLastname,
                ShippingCompanyName = Order.ShippingCompanyName,
                ShippingAddress1 = Order.ShippingAddress1,
                ShippingAddress2 = Order.ShippingAddress2,
                ShippingCity = Order.ShippingCity,
                ShippingRegion = Order.ShippingRegion,
                ShippingCountry = Order.ShippingCountry,
                ShippingZip = Order.ShippingZip,
                BillingFirstname = Order.BillingFirstname,
                BillingLastname = Order.BillingLastname,
                BillingCompanyName = Order.BillingCompanyName,
                BillingAddress1 = Order.BillingAddress1,
                BillingAddress2 = Order.BillingAddress2,
                BillingCity = Order.BillingCity,
                BillingRegion = Order.BillingRegion,
                BillingCountry = Order.BillingCountry,
                BillingZip = Order.BillingZip,
                TotalPrice = LocalUtilities.FormatMoney(Order.OrderTotalPrice),
                PaymentDate = string.Format(Formats.DateEval, Order.OrderPaymentDateTime),
                PaymentOption = Order.PaymentOption
            };
        }

        public static PageViewModel.GridModel GetOrderDetailsGridModel(UrlHelper Url, User User, Order Order)
        {

            return new PageViewModel.GridModel
            {
                UrlList = Url.RouteUrl(ControllerActionRouteNames.Admin.Orders.OrderDetails.Grid),
                GridItems = Order.OrderDetails?.Select(Item => new PageViewModel.GridModel.GridItem
                {
                    OrderDetailID = Item.OrderDetailID,
                    OrderDetailProductID = Item.OrderDetailProductID,
                    OrderDetailProductCaption = Item.OrderDetailProductCaption,
                    OrderDetailProductCount = Item.OrderDetailProductCount,
                    OrderDetailProductPrice = Item.OrderDetailProductPrice,
                    OrderDetailProductPricePaid = Item.OrderDetailProductPricePaid
                }).ToList()
            };
        }
        #endregion

        #region Sub Classes
        public class PageViewModel : Order
        {
            #region Properties
            public bool ShowGrid { get; set; }
            public GridModel Grid { get; set; }
            public List<SimpleKeyValue<int?, string>> OrderStatuses { get; set; }
            public string PaymentDate { get; set; }
            public string TotalPrice { get; set; }
            #endregion

            #region Sub Classes
            public class GridModel : DevexpressGridViewModelBase
            {
                #region Properties
                const string GridName = "OrderDetailsGrid";
                public List<GridItem> GridItems { get; set; }
                #endregion

                #region Methods
                public override void InitGridSettings(GridViewSettings Settings, HtmlHelper Html, UrlHelper Url)
                {
                    InitGridViewDefaultSettings(Settings);
                    GridItem GridItem;

                    Settings.Name = GridName;
                    Settings.KeyFieldName = nameof(GridItem.OrderDetailID);
                    Settings.SettingsPager.PageSize = 100;

                    Settings.CommandColumn.Visible = false;
                    
                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(GridItem.OrderDetailProductCaption); column.Caption = "Product Name"; column.Width = Unit.Pixel(250); column.ColumnType = MVCxGridViewColumnType.TextBox;
                        var TextBoxProperties = column.PropertiesEdit as TextBoxProperties;
                        InitTextEditProperties(Properties: TextBoxProperties, MaxLength: 200, MakeRequired: true);
                        column.ReadOnly = true;
                    });
                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(GridItem.OrderDetailProductCount); column.Caption = "Product Count"; column.ColumnType = MVCxGridViewColumnType.SpinEdit; column.Width = Unit.Pixel(100);
                        var SpinEditProperties = column.PropertiesEdit as SpinEditProperties;
                        InitSpinEditProperties(Properties: SpinEditProperties, MakeRequired: false);
                    });
                    Settings.Columns.Add(column =>
                    {
                        column.FieldName = nameof(GridItem.OrderDetailProductPrice); column.Caption = "Product Price"; column.ColumnType = MVCxGridViewColumnType.SpinEdit; column.Width = Unit.Pixel(100);
                        var SpinEditProperties = column.PropertiesEdit as SpinEditProperties;
                        InitSpinEditProperties(Properties: SpinEditProperties, DecimalPlaces: 2, MakeRequired: false);
                    });

                    Settings.Columns.Add(column => { column.SetEditItemTemplateContent(" "); });
                }
                #endregion

                #region Sub Classes
                public class GridItem : OrderDetail
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
}