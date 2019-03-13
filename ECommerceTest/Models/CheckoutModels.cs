using Core;
using Core.Enums;
using Core.Properties;
using Core.Reporting;
using Core.Services;
using Core.Services.Payments;
using Core.Utilities;
using ECommerceTest.Controllers;
using Reusables.Core;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using static Core.Order;

namespace ECommerceTest.Models
{
    public class CheckoutModel
    {
        #region Methods
        public static void CompletePayment(int? OrderID, WebsiteController C, string TransactionID = null)
        {
            var DAL = new OrdersDataAccess();
            DAL.OrdersUpdate(
                OrderID: OrderID,
                OrderStatusID: DictionariesDataAccess.GetDictionaryIDByDictionaryCodeAndIntCode(Dictionaries.Codenames.OrderStatuses.DictionaryCode, OrderStatuses.PAID),
                OrderPaymentDateTime: DateTime.Now,
                TransactionID: TransactionID
            );

            var User = SessionAssistance.GetUser(C.Session);
            User.ProductCountInBasket = 0;
            SessionAssistance.SetUser(C.Session, User);

            NotificationManager.SendSaleNotificationToAdmin(OrderID);
            var Report = new InvoiceReport1(OrderID);
            var InvoicePath = $"{AppSettings.UploadFolderPhysicalPath}invoice-{OrderID}.pdf";
            Report.ExportToPdf(InvoicePath);
            NotificationManager.SendPaymentSuccessEmailToCustomer(OrderID, InvoicePath, C.User.Email);
            System.IO.File.Delete(InvoicePath);
        }

        public static PageViewModel GetPageViewModel(WebsiteController C, PageViewModel Model = null)
        {            
            var Order = OrdersDataAccess.GetCustomerBasket(C.User.UserID);
            if (Model == null)
            {
                Model = new PageViewModel();
                Model.IsBillingSameAsShipping = true;
            }

            Model.PageTitle = Resources.TextCheckout;

            Model.ShippingCountries = DictionariesDataAccess.ListDictionaryItemsAsKeyValueByDictionaryCodename(Dictionaries.Codenames.Countries);
            Model.ShippingCountries?.ForEach(Item => { Item.IsSelected = Order.ShippingCountryID == Item.Key; });

            Model.BillingCountries = DictionariesDataAccess.ListDictionaryItemsAsKeyValueByDictionaryCodename(Dictionaries.Codenames.Countries);
            Model.BillingCountries?.ForEach(Item => { Item.IsSelected = Order.BillingCountryID == Item.Key; });

            Model.ShippingFirstname = Order.ShippingFirstname ?? C.User.Firstname;
            Model.ShippingLastname = Order.ShippingLastname ?? C.User.Lastname;
            Model.ShippingRegion = Order.ShippingRegion;
            Model.ShippingCity = Order.ShippingCity;
            Model.ShippingZip = Order.ShippingZip;
            Model.ShippingAddress1 = Order.ShippingAddress1;
            Model.ShippingAddress2 = Order.ShippingAddress2;

            Model.IsBillingSameAsShipping = Order.IsBillingSameAsShipping;

            Model.BillingFirstname = Order.BillingFirstname;
            Model.BillingLastname = Order.BillingLastname;
            Model.BillingRegion = Order.BillingRegion;
            Model.BillingCity = Order.BillingCity;
            Model.BillingZip = Order.BillingZip;
            Model.BillingAddress1 = Order.BillingAddress1;
            Model.BillingAddress2 = Order.BillingAddress2;


            Model.OrderDetails = Order.OrderDetails.Select(Item=>new SimpleKeyValue<string, string>
            {
                Key = $"{Item.OrderDetailProductCaption} x {Item.OrderDetailProductCount}",
                Value = LocalUtilities.FormatMoney(Item.OrderDetailProductPricePaidTotal)
            }).ToList();

            var PaymentOptions = DictionariesDataAccess.ListDictionaries(Level: 1, DictionaryCode: Dictionaries.Codenames.PaymentOptions.DictionaryCode);
            Model.PaymentOptionID = Order.PaymentOptionID;
            Model.PaymentOptionPaypalID = PaymentOptions?.FirstOrDefault(Item => Item.IntCode == Core.Enums.PaymentOptions.PAYPAL)?.DictionaryID;
            Model.PaymentOptionCardID = PaymentOptions?.FirstOrDefault(Item => Item.IntCode == Core.Enums.PaymentOptions.CARD)?.DictionaryID;

            Model.OrderPriceTotal = LocalUtilities.FormatMoney(Order.OrderTotalPrice);
            

            return Model;
        }

        public static Order UpdateOrder(PageViewModel SubmitModel, User User)
        {
            var Order = OrdersDataAccess.GetCustomerBasket(User.UserID);

            var DAL = new OrdersDataAccess();
            DAL.OrdersUpdate(
                OrderID: Order.OrderID,
                OrderStatusID: null,
                ShippingFirstname: SubmitModel.ShippingFirstname,
                ShippingLastname: SubmitModel.ShippingLastname,
                ShippingAddress1: SubmitModel.ShippingAddress1,
                ShippingAddress2: SubmitModel.ShippingAddress2 ?? string.Empty,
                ShippingCity: SubmitModel.ShippingCity,
                ShippingRegion: SubmitModel.ShippingRegion,
                ShippingCountryID: SubmitModel.ShippingCountryID,
                ShippingZip: SubmitModel.ShippingZip,
                IsBillingSameAsShipping: SubmitModel.IsBillingSameAsShipping,
                BillingFirstname: SubmitModel.BillingFirstname,
                BillingLastname: SubmitModel.BillingLastname,
                BillingAddress1: SubmitModel.BillingAddress1,
                BillingAddress2: SubmitModel.BillingAddress2 ?? string.Empty,
                BillingCity: SubmitModel.BillingCity,
                BillingRegion: SubmitModel.BillingRegion,
                BillingCountryID: SubmitModel.BillingCountryID,
                BillingZip: SubmitModel.BillingZip,
                Phone: null,
                OrderPaymentDateTime: null,
                IsOrderDelivered: null,
                PaymentOptionID: SubmitModel.PaymentOptionID,
                TransactionID: null
            );

            Order = OrdersDataAccess.GetSingleOrderByID(Order.OrderID);
            return Order;
        }

        public static void ValidatePageViewModel(PageViewModel Model)
        {
            Model.Form.Errors = new List<SimpleKeyValue<string, string>>();

            Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.ShippingFirstname)}]", ValueToValidate: Model.ShippingFirstname));
            Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.ShippingLastname)}]", ValueToValidate: Model.ShippingLastname));
            Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.ShippingAddress1)}]", ValueToValidate: Model.ShippingAddress1));
            Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.ShippingCity)}]", ValueToValidate: Model.ShippingCity));
            Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.ShippingRegion)}]", ValueToValidate: Model.ShippingRegion));
            Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.ShippingCountryID)}]", ValueToValidate: Model.ShippingCountryID));
            Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.ShippingZip)}]", ValueToValidate: Model.ShippingZip));


            if (!Model.IsBillingSameAsShipping)
            {
                Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.BillingFirstname)}]", ValueToValidate: Model.BillingFirstname));
                Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.BillingLastname)}]", ValueToValidate: Model.BillingLastname));
                Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.BillingAddress1)}]", ValueToValidate: Model.BillingAddress1));
                Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.BillingCity)}]", ValueToValidate: Model.BillingCity));
                Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.BillingRegion)}]", ValueToValidate: Model.BillingRegion));
                Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.BillingCountryID)}]", ValueToValidate: Model.BillingCountryID));
                Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.BillingZip)}]", ValueToValidate: Model.BillingZip));
            }

            if (Model.IsPaymentByCard)
            {
                Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.CreditCardNumber)}]", ValueToValidate: Model.CreditCardNumber));
                Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.CreditCardExpirationDate)}]", ValueToValidate: Model.CreditCardExpirationDate));
                Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.CreditCardCcv)}]", ValueToValidate: Model.CreditCardCcv));
            }

            Model.Form.Errors.RemoveAll(Item => Item == null);
        }        

        public static async Task<List<SimpleKeyValue<string, string>>> ProcessCreditCard(PageViewModel Model, Order Order, User User)
        {
            var Errors = new List<SimpleKeyValue<string, string>>();

            if (Order.OrderTotalPrice > 0)
            {
                var PI = new PaymentInfo
                {
                    OrderID = Order.OrderID,
                    CustomerID = Order.CustomerID,
                    Email = User.Email,
                    FName = User.Firstname,
                    LName = User.Lastname,
                    Address = $"{Order.BillingAddress1} {Order.BillingAddress2}".Trim(),
                    Zip = Order.BillingZip,
                    State = Order.BillingRegion,
                    City = Order.BillingCity,
                    Country = Order.BillingCountry,
                    Description = $"{AppSettings.ProjectName} order# {Order.OrderID}",

                    CardNumber = Model.CreditCardNumber,
                    CCV = Model.CreditCardCcv,
                    ExpDate = Model.CreditCardExpirationDate,
                    CardType = CreditCardHepler.GetCreditCardTypeByCardNumber(Model.CreditCardNumber),
                    Amount = Order.OrderTotalPrice.Value
                };

                var CCP = new CreditCardPayment(AppSettings.AdnApiUrl, AppSettings.AdnLoginID, AppSettings.AdnTransactionKey);
                var Result = await CCP.SubmitPaymentTransaction(PI);

                if (CCP.IsError || Result.ADNResultArray.Length == 0 || Result.ADNResultArray[0] != "1")
                {
                    if (Result.ADNResultArray.Length == 0)
                    {
                        Errors.Add(new SimpleKeyValue<string, string> { Key = null, Value = Resources.TextError });
                    }
                    else
                    {

                        switch (Result.ADNResultArray[2])
                        {
                            case "6":
                                {
                                    Errors.Add(new SimpleKeyValue<string, string> { Key = $"[name={nameof(Model.CreditCardNumber)}]", Value = Result.ADNResultArray[3] });
                                    break;
                                }
                            case "8":
                            case "7":
                                {
                                    Errors.Add(new SimpleKeyValue<string, string> { Key = $"[name={nameof(Model.CreditCardExpirationDate)}]", Value = Result.ADNResultArray[3] });
                                    break;
                                }
                            case "78":
                                {
                                    Errors.Add(new SimpleKeyValue<string, string> { Key = $"[name={nameof(Model.CreditCardCcv)}]", Value = Result.ADNResultArray[3] });
                                    break;

                                }
                            default:
                                {
                                    Errors.Add(new SimpleKeyValue<string, string> { Key = $"[name={nameof(Model.CreditCardNumber)}]", Value = Result.ADNResultArray[3] });
                                    break;
                                }
                        }
                    }
                }
            }
            else
            {
                Errors.Add(new SimpleKeyValue<string, string> { Key = $"[name={nameof(Model.CreditCardNumber)}]", Value = Resources.TextError });
            }

            return Errors;
        }

        public static async Task<string> ProcessPaypalPayment(Order Order, UrlHelper Url)
        {
            var P = new PayPalService();
            var ReturnURL = $"{AppSettings.WebsiteAddress}{Url.RouteUrl(ControllerActionRouteNames.Website.Checkout.PaypalResponse)}";
            var CancelUrl = $"{AppSettings.WebsiteAddress}{Url.RouteUrl(ControllerActionRouteNames.Website.Checkout.Page)}";

            if (Order.OrderTotalPrice > 0)
            {
                await P.CreatePayment($"ECommerceTest Order #{Order.OrderID}", Math.Round(Order.OrderTotalPrice.Value, 2).ToString(), ReturnURL, CancelUrl, Order.OrderID, Order.OrderDetails, Order.CustomerID);
                if (!string.IsNullOrWhiteSpace(P.CustomerAuthenticationRedirecLink))
                {
                    return P.CustomerAuthenticationRedirecLink;
                }
            }
            return null;
        }
        #endregion

        #region Sub Classes
        public class PageViewModel
        {
            #region Properties            
            public string PageTitle { get; set; }

            public int? OrderID { get; set; }
            public string ShippingFirstname { get; set; }
            public string ShippingLastname { get; set; }
            public string ShippingCompanyName { get; set; }
            public string ShippingAddress1 { get; set; }
            public string ShippingAddress2 { get; set; }
            public string ShippingCity { get; set; }
            public string ShippingRegion { get; set; }
            public List<SimpleKeyValue<int?, string>> ShippingCountries { get; set; }
            public int? ShippingCountryID { get; set; }
            public string ShippingZip { get; set; }
            public bool IsBillingSameAsShipping { get; set; }
            public string BillingFirstname { get; set; }
            public string BillingLastname { get; set; }
            public string BillingCompanyName { get; set; }
            public string BillingAddress1 { get; set; }
            public string BillingAddress2 { get; set; }
            public string BillingCity { get; set; }
            public string BillingRegion { get; set; }
            public List<SimpleKeyValue<int?, string>> BillingCountries { get; set; }
            public int? BillingCountryID { get; set; }
            public string BillingZip { get; set; }
            
            public string CreditCardNumber { get; set; }
            public string CreditCardExpirationDate { get; set; }
            public string CreditCardCcv { get; set; }

            
            public List<SimpleKeyValue<string,string>> OrderDetails { get; set; }
            public bool HasOrderDetails => OrderDetails?.Count > 0;

            public string OrderPriceTotal { get; set; }

            public int? PaymentOptionID { get; set; }            
            public string TransactionID { get; set; }

            public FormViewModelBase Form { get; set; } = new FormViewModelBase();
            public string PartialView { get; set; }
            public bool HasPartialView => PartialView?.Length > 0;
                                    
            public int? PaymentOptionPaypalID { get; set; }
            public int? PaymentOptionCardID { get; set; }
            public bool IsPaymentFailed { get; set; }
            public bool IsPaymentByCard { get; set; }
            public string TextPaymentFailed { get; set; } = Resources.TextPaymentFailed;            
            #endregion
        }

        public class Response
        {
            #region Properties
            public decimal? Subtotal { get; set; }
            public decimal? Shipping { get; set; }
            public decimal? SalesTax { get; set; }
            public decimal? Total { get; set; }
            #endregion
        }
        #endregion
    }
}