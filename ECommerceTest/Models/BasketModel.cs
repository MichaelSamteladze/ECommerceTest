using Core;
using Core.Enums;
using Core.Properties;
using Core.Utilities;
using ECommerceTest.Controllers;
using Reusables.Core;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;
using System.Linq;

namespace ECommerceTest.Models
{
    public class BasketModel
    {
        #region Methods

        public static AjaxResponse BasketAddItem(WebsiteController C, int? ProductID, int? ProductCount)
        {
            var AR = new AjaxResponse();
            var DAL = new OrdersDataAccess();
            var Product = ProductsDataAccess.GetSingleProductByID(ProductID);

            DAL.OrderDetailsInsert(
                CustomerID: C.User.UserID,
                ProductID: Product.ProductID,
                ProductCount: ProductCount,
                ProductPrice: Product.ProductPrice
            );

            if(!DAL.IsError)
            {
                var Order = OrdersDataAccess.GetCustomerBasket(C.User.UserID);
                var User = UsersDataAccess.GetSingleUserByID(C.User.UserID);
                SessionAssistance.SetUser(C.Session, User);

                AR.IsSuccess = true;
                AR.Data = new
                {
                    ProductCountInBasket = User.ProductCountInBasket,
                    TotalPrice = LocalUtilities.FormatMoney(Order.OrderDetails?.Sum(Item => Item.OrderDetailProductPricePaidTotal))
                };
            }

            return AR;
        }

        public static PageViewModel GetPageViewModel(WebsiteController C)
        {
            var Order = OrdersDataAccess.GetCustomerBasket(C.User.UserID);
            var Model = new PageViewModel
            {
                PageTitle = Resources.TextBasket,
                OrderDetailDeleteUrl = C.Url.RouteUrl(ControllerActionRouteNames.Website.Basket.Delete),
                OrderDetailsUpdateUrl = C.Url.RouteUrl(ControllerActionRouteNames.Website.Basket.Update),
                PaymentInfoUrl = C.Url.RouteUrl(ControllerActionRouteNames.Website.Checkout.Page)
            };

            if (Order != null && Order.OrderDetails?.Count > 0)
            {
                Model.BasketItems = Order.OrderDetails.Select(Item => new PageViewModel.BasketItem
                {
                    ItemID = Item.OrderDetailID,
                    ImagePath = Utility.GetUploadedFileHttpPath(Item.ProductImageFilename),
                    ProductCaption = Item.OrderDetailProductCaption,
                    ProductCount = Item.OrderDetailProductCount,
                    ProductPrice = LocalUtilities.FormatMoney(Item.ProductPrice),
                    ProductPriceSum = LocalUtilities.FormatMoney(Item.OrderDetailProductCount * Item.ProductPrice),
                    ProductDetailsUrl = C.Url.RouteUrl(ControllerActionRouteNames.Website.Products.Product, new { Item.ProductSlug }),
                    ProductID = Item.OrderDetailProductID,
                    ProductOptionID = Item.OrderDetailProductOptionID
                }).ToList();

                Model.PriceTotal = LocalUtilities.FormatMoney(Order.OrderDetails.Sum(Item => Item.OrderDetailProductCount * Item.ProductPrice));
            }
            else
            {
                Model.PriceTotal = LocalUtilities.FormatMoney(0);
            }

            return Model;
        }

        public static AjaxResponse BasketDeleteItem(WebsiteController C, int? OrderDetailID)
        {
            var DAL = new OrdersDataAccess();

            DAL.OrderDetailsIUD(
                DatabaseAction: DatabaseActions.DELETE,
                OrderDetailID: OrderDetailID
            );

            var Order = OrdersDataAccess.GetCustomerBasket(C.User.UserID);
            var User = UsersDataAccess.GetSingleUserByID(C.User.UserID);
            SessionAssistance.SetUser(C.Session, User);

            return new AjaxResponse
            {
                IsSuccess = !DAL.IsError,
                Data = new
                {
                    ProductCountInBasket = User.ProductCountInBasket,
                    TotalPrice = LocalUtilities.FormatMoney(Order.OrderDetails?.Sum(Item=>Item.ProductPrice))
                }
            };
        }

        public static AjaxResponse BasketSyncProductCount(PageViewModel.BasketSyncProductCountSubmitModel SubmitModel, WebsiteController C)
        {
            var DAL = new OrdersDataAccess();
            DAL.OrderDetailsSyncProductCount(SubmitModel.OrderDetails, C.User.UserID);

            var AR = new AjaxResponse();
            var Order = OrdersDataAccess.GetCustomerBasket(C.User.UserID);
            if (Order == null || Order.OrderTotalPrice == 0)
            {
                AR.IsSuccess = false;
                AR.Data = Resources.TextBasketEmpty;
            }
            else if (DAL.IsError)
            {
                AR.IsSuccess = false;                
                AR.Data = Resources.TextError;
            }
            else
            {
                Order = OrdersDataAccess.GetCustomerBasket(C.User.UserID);
                var User = UsersDataAccess.GetSingleUserByID(C.User.UserID);
                SessionAssistance.SetUser(C.Session, User);

                AR.IsSuccess = true;
                AR.Data = new
                {
                    ProductCountInBasket = User.ProductCountInBasket,
                    TotalPrice = LocalUtilities.FormatMoney(Order.OrderDetails?.Sum(Item => Item.OrderDetailProductPricePaidTotal))
                };
            }

            return AR;
        }
        #endregion

        #region Sub Classes
        public class PageViewModel
        {
            #region Properties
            public string PageTitle { get; set; }
            public List<BasketItem> BasketItems { get; set; }
            public bool HasBasketItems => BasketItems?.Count > 0;
            public string OrderDetailDeleteUrl { get; set; }
            public string OrderDetailsUpdateUrl { get; set; }
            public string PaymentInfoUrl { get; set; }
            public string PriceTotal { get; set; }
            public string NoImagePath { get; set; } = AppSettings.NoImagePath;
            public string TextBasketEmpty { get; set; } = Resources.TextBasketEmpty;
            #endregion

            #region Sub Classes
            public class BasketItem
            {
                #region Properties
                public int? ItemID { get; set; }
                public string ImagePath { get; set; }
                public bool HasImage => ImagePath?.Length > 0;
                public string ProductCaption { get; set; }
                public int? ProductCount { get; set; }
                public string ProductPrice { get; set; }
                public string ProductPriceSum { get; set; }
                public string ProductDetailsUrl { get; set; }
                public int? ProductID { get; set; }
                public int? ProductOptionID { get; set; }
                #endregion
            }

            public class BasketSyncProductCountSubmitModel
            {
                public List<SimpleKeyValue<int?,int?>> OrderDetails { get; set; }
            }
            #endregion
        }
        #endregion
    }
}