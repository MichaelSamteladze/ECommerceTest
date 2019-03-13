using Core;
using Core.Enums;
using Core.Properties;
using Core.Reporting;
using Core.Services;
using Core.Utilities;
using ECommerceTest.Controllers;
using Reusables.Core;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerceTest.Models
{
    public class AccountModel
    {
        #region Sub Classes
        public class Profile
        {
            #region Methods
            public static PageViewModel GetPageViewModel(HttpSessionStateBase Session, PageViewModel Model = null)
            {
                var User = SessionAssistance.GetUser(Session);
                Model = new PageViewModel
                {
                    PageTitle = Resources.TextProfile,
                    Firstname = User.Firstname,
                    Lastname = User.Lastname,
                    Email = User.Email
                };

                return Model;
            }

            public static void Update(PageViewModel Model, HttpSessionStateBase Session)
            {
                var User = SessionAssistance.GetUser(Session);
                if (User != null)
                {
                    var DAL = new UsersDataAccess();
                    DAL.UsersIUD(
                        DatabaseAction: DatabaseActions.UPDATE,
                        UserID: User.UserID,
                        Firstname: Model.Firstname,
                        Lastname: Model.Lastname,
                        Email: Model.Email
                    );

                    if (DAL.IsError)
                    {
                        Model.Form.IsError = true;
                    }
                    else
                    {
                        var U = UsersDataAccess.GetSingleUserByID(User.UserID);
                        SessionAssistance.SetUser(Session, U);
                        Model.Form.IsSaved = true;
                    }

                }                
            }            

            public static bool SendConfirmationCode(PageViewModel Model, HttpSessionStateBase Session)
            {
                var ConfirmationCode = (new Random().Next(100000, 999999)).ToString();
                var U = SessionAssistance.GetUser(Session);

                var User = new User
                {
                    UserID = U.UserID,
                    Firstname = Model.Firstname,
                    Lastname = Model.Lastname,                    
                    Email = Model.Email
                };

                SessionAssistance.SetValue(Session, ConfirmationCode, User);

                return NotificationManager.SendConfirmationCode(Model.Firstname, Model.Email, ConfirmationCode);
            }

            public static void ValidateAccountForm(PageViewModel Model, HttpSessionStateBase Session)
            {
                Model.Form.Errors = new List<SimpleKeyValue<string, string>>();
                var User = SessionAssistance.GetUser(Session);

                Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.Firstname)}]", ValueToValidate: Model.Firstname));
                Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.Lastname)}]", ValueToValidate: Model.Lastname));

                if (User.Email != Model.Email)
                {
                    Model.Form.Errors.Add(Validation.ValidateEmail(ErrorKey: $"[name={nameof(Model.Email)}]", Email: Model.Email, ValidateRequired: true, ValidateUnique: true, UserID: null));
                }

                Model.Form.Errors.RemoveAll(Item => Item == null);
            }

            public static void ValidateConfirmationCode(PageViewModel Model)
            {
                Model.Form.Errors = new List<SimpleKeyValue<string, string>>();
                Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.ConfirmationCode)}]", ValueToValidate: Model.ConfirmationCode));
                Model.Form.Errors.RemoveAll(Item => Item == null);
            }
            #endregion

            #region Sub Classes
            public class PageViewModel
            {
                #region Properties
                public string PageTitle { get; set; }
                public string Email { get; set; }
                public string Firstname { get; set; }
                public string Lastname { get; set; }
                public FormViewModelBase Form { get; set; } = new FormViewModelBase();
                public string ConfirmationCode { get; set; }
                public string ValidateConfirmationCodeNotValid { get; set; } = Resources.ValidateConfirmationCodeNotValid;
                #endregion
            }
            #endregion
        }

        public class PasswordChangeModel
        {
            #region Methods
            public static PageViewModel GetPageViewModel(PageViewModel Model = null)
            {
                if (Model == null)
                {
                    Model = new PageViewModel();
                }

                Model.PageTitle = Resources.TextChangePassword;

                return Model;
            }

            public static PageViewModel UpdatePassword(PageViewModel Model, HttpSessionStateBase Session)
            {
                var DAL = new UsersDataAccess();
                var User = SessionAssistance.GetUser(Session);

                DAL.UsersIUD(
                    DatabaseAction: DatabaseActions.UPDATE,
                    UserID: User.UserID,
                    Password: Model.NewPassword
                );

                if (!DAL.IsError)
                {
                    var U = UsersDataAccess.GetSingleUserByID(User.UserID);
                    SessionAssistance.SetUser(Session, U);
                    Model.Form.IsSaved = true;
                }

                return Model;
            }

            public static void ValidateUpdatePasswordForm(PageViewModel Model, string UserPassword)
            {
                Model.Form.Errors = new List<SimpleKeyValue<string, string>>();

                Model.Form.Errors.Add(Validation.ValidateOldPassword(ErrorKey: $"[name={nameof(Model.OldPassword)}]", UserPassword: UserPassword, OldPassword: Model.OldPassword));
                Model.Form.Errors.Add(Validation.ValidatePassword(ErrorKey: $"[name={nameof(Model.NewPassword)}]", Password: Model.NewPassword));
                Model.Form.Errors.Add(Validation.ValidatePasswordRepeat(ErrorKey: $"[name={nameof(Model.NewPassword)}]", Password: Model.NewPassword, PasswordRepeat: Model.NewPasswordRepeat));

                Model.Form.Errors.RemoveAll(Item => Item == null);
            }
            #endregion

            #region Sub Classes
            public class PageViewModel
            {
                #region Properties
                public string PageTitle { get; set; }
                public string OldPassword { get; set; }
                public string NewPassword { get; set; }
                public string NewPasswordRepeat { get; set; }
                public FormViewModelBase Form { get; set; } = new FormViewModelBase();
                #endregion
            }
            #endregion
        }

        public class OrderHistoryModel
        {
            #region Methods
            public static PageViewModel GetPageViewModel(UrlHelper Url, WebsiteController Controller)
            {
                var Orders = OrdersDataAccess.ListOrders(Controller.User.UserID, OrderStatuses.PAID);
                var OrderDetails = OrdersDataAccess.ListOrderDetails(Controller.User.UserID);
                return new PageViewModel
                {
                    PageTitle = Resources.TextOrders,
                    Orders = Orders?.Select(Item => new PageViewModel.OrderItem
                    {
                        OrderID = Item.OrderID,
                        OrderDate = string.Format(Constants.Formats.DateEval, Item.OrderPaymentDateTime),
                        OrderTotalPrice = LocalUtilities.FormatMoney(Item.OrderTotalPrice),
                        OrderDetailItems = OrderDetails.Where(O => O.OrderID == Item.OrderID).Select(SubItem => new PageViewModel.OrderItem.OrderItemDetail
                        {
                            ProductCaption = SubItem.ProductCaption,
                            ProductNavigateUrl = Url.RouteUrl(ControllerActionRouteNames.Website.Products.Product, new { SubItem.ProductSlug }),
                            ProductCount = SubItem.OrderDetailProductCount,
                            PricePaid = LocalUtilities.FormatMoney(SubItem.OrderDetailProductPricePaid)
                        }).ToList(),
                        InvoiceUrl = Url.RouteUrl(ControllerActionRouteNames.Website.Account.InvoiceExport, new { Item.OrderID })
                    }).ToList(),

                };
            }

            public static byte[] GetInvoiceReport(int? OrderID)
            {
                var Report = new InvoiceReport1(OrderID);
                MemoryStream ms = new MemoryStream();
                Report.ExportToPdf(ms);
                byte[] bytesInStream = ms.ToArray();
                ms.Close();

                return bytesInStream;
            }

            public static bool ValidateOrderIDByUserID(int? OrderID, User UserItem)
            {
                var Result = false;

                var Order = OrdersDataAccess.GetSingleOrderByID(OrderID);
                if (Order != null && Order.CustomerID == UserItem.UserID)
                {
                    Result = true;
                }

                return Result;
            }

            #endregion

            #region Sub Classes
            public class PageViewModel
            {
                #region Properties
                public string PageTitle { get; set; } 
                public List<OrderItem> Orders { get; set; }
                public bool HasOrders => Orders?.Count > 0;
                #endregion

                #region Sub Classes
                public class OrderItem 
                {
                    #region Properties
                    public int? OrderID { get; set; }
                    public string OrderDate { get; set; }
                    public string OrderTotalPrice { get; set; }
                    public string InvoiceUrl { get; set; }
                    public List<OrderItemDetail> OrderDetailItems { get; set; }
                    #endregion

                    #region Sub Classes
                    public class OrderItemDetail
                    {
                        #region Properties
                        public string ProductCaption { get; set; }
                        public string PricePaid { get; set; }
                        public int? ProductCount { get; set; }
                        public string ProductNavigateUrl { get; set; }
                        #endregion
                    }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}