using Core.DB;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class OrdersDataAccess : SixtyThreeBitsDataObject
    {
        #region Methods
        public static Order GetSingleOrderByID(int? OrderID)
        {
            return TryToReturnStatic($"{nameof(GetSingleOrderByID)}({nameof(OrderID)} = {OrderID})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.OrdersGetSingleByID(OrderID).DeserializeTo<Order>();
                }
            });
        }

        public static Order GetCustomerBasket(int? CustomerID)
        {
            return TryToReturnStatic($"{nameof(GetCustomerBasket)}({nameof(CustomerID)} = {CustomerID})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.OrdersGetSingleOrderWithStatusBasketByCustomerID(CustomerID).DeserializeTo<Order>();
                }
            });
        }

        public static List<OrderDetailsListResult> ListOrderDetails(int? CustomerID = null, int? OrderID = null)
        {
            return TryToReturnStatic($"{nameof(ListOrderDetails)}({nameof(CustomerID)} = {CustomerID}, {nameof(OrderID)} = {OrderID})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.OrderDetailsList(CustomerID, OrderID).OrderBy(Item => Item.ProductCaption).ToList();
                }
            });
        }

        public static List<OrdersListResult> ListOrders(int? CustomerID = null, int? OrderStatusIntCode = null)
        {
            return TryToReturnStatic($"{nameof(ListOrders)}({nameof(CustomerID)} = {CustomerID}, {nameof(OrderStatusIntCode)} = {OrderStatusIntCode})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.OrdersList(CustomerID, OrderStatusIntCode).OrderByDescending(Item => Item.OrderPaymentDateTime).ToList();
                }
            });
        }

        public int? OrderDetailsIUD(byte? DatabaseAction, int? OrderDetailID = null, int? ProductCount = null)
        {
            return TryToReturnStatic($"{nameof(OrderDetailsIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(OrderDetailID)} = {OrderDetailID}, {nameof(ProductCount)} = {ProductCount})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    db.OrderDetailsIUD(DatabaseAction, OrderDetailID, ref ProductCount);
                    return ProductCount;
                }
            });
        }

        public OrderDetailsInsertResponse OrderDetailsInsert(int? OrderID = null, int? CustomerID = null, int? ProductID = null, int? ProductCount = null, decimal? ProductPrice = null)
        {
            return TryToReturnStatic($"{nameof(OrderDetailsInsert)}({nameof(OrderID)} = {OrderID}, {nameof(CustomerID)} = {CustomerID}, {nameof(ProductID)} = {ProductID}, {nameof(ProductCount)} = {ProductCount}, {nameof(ProductPrice)} = {ProductPrice})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    db.OrderDetailsInsert(ref OrderID, CustomerID, ProductID, ref ProductCount, ProductPrice);
                    return new OrderDetailsInsertResponse
                    {
                        OrderID = OrderID,
                        ProductCount = ProductCount
                    };
                }
            });
        }

        public void OrderDetailsSyncProductCount(List<SimpleKeyValue<int?,int?>> OrderDetailsCounts, int? UserID)
        {
            TryExecute($"{nameof(OrderDetailsSyncProductCount)}({nameof(OrderDetailsCounts)} = {OrderDetailsCounts.ToXml()},{nameof(UserID)} = {UserID})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {                    
                    db.OrderDetailsSyncProductCount(UserID, OrderDetailsCounts.ToXElement());
                }
            });
        }

        public int? OrdersUpdate(int? OrderID = null, int? OrderStatusID = null, string ShippingFirstname = null, string ShippingLastname = null, string ShippingAddress1 = null, string ShippingAddress2 = null, string ShippingCity = null, string ShippingRegion = null, int? ShippingCountryID = null, string ShippingZip = null, bool? IsBillingSameAsShipping = null, string BillingFirstname = null, string BillingLastname = null, string BillingAddress1 = null, string BillingAddress2 = null, string BillingCity = null, string BillingRegion = null, int? BillingCountryID = null, string BillingZip = null, string Phone = null, DateTime? OrderPaymentDateTime = null, bool? IsOrderDelivered = null, int? PaymentOptionID = null, string TransactionID = null)
        {
            return TryToReturnStatic($"{nameof(OrdersUpdate)}({nameof(OrderID)} = {OrderID}, {nameof(OrderStatusID)} = {OrderStatusID}, {nameof(ShippingFirstname)} = {ShippingFirstname}, {nameof(ShippingLastname)} = {ShippingLastname}, {nameof(ShippingAddress1)} = {ShippingAddress1}, {nameof(ShippingAddress2)} = {ShippingAddress2}, {nameof(ShippingCity)} = {ShippingCity}, {nameof(ShippingRegion)} = {ShippingRegion}, {nameof(ShippingCountryID)} = {ShippingCountryID}, {nameof(ShippingZip)} = {ShippingZip}, {nameof(IsBillingSameAsShipping)} = {IsBillingSameAsShipping}, {nameof(BillingFirstname)} = {BillingFirstname}, {nameof(BillingLastname)} = {BillingLastname}, {nameof(BillingAddress1)} = {BillingAddress1}, {nameof(BillingAddress2)} = {BillingAddress2}, {nameof(BillingCity)} = {BillingCity}, {nameof(BillingRegion)} = {BillingRegion}, {nameof(BillingCountryID)} = {BillingCountryID}, {nameof(BillingZip)} = {BillingZip}, {nameof(Phone)} = {Phone}, {nameof(OrderPaymentDateTime)} = {OrderPaymentDateTime}, {nameof(IsOrderDelivered)} = {IsOrderDelivered}, {nameof(PaymentOptionID)} = {PaymentOptionID}, {nameof(TransactionID)} = {TransactionID}, )", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    db.OrdersUpdate(ref OrderID, OrderStatusID, ShippingFirstname, ShippingLastname, ShippingAddress1, ShippingAddress2, ShippingCity, ShippingRegion, ShippingCountryID, ShippingZip, IsBillingSameAsShipping, BillingFirstname, BillingLastname, BillingAddress1, BillingAddress2, BillingCity, BillingRegion, BillingCountryID, BillingZip, Phone, OrderPaymentDateTime, IsOrderDelivered, PaymentOptionID, TransactionID);
                    return OrderID;
                }
            });
        }
        #endregion
    }

    public class Order
    {
        #region Properties
        public int? OrderID { get; set; }
        public int? OrderStatusID { get; set; }
        public int? CustomerID { get; set; }
        public string Customer { get; set; }        
        public string ShippingFirstname { get; set; }
        public string ShippingLastname { get; set; }
        public string ShippingCompanyName { get; set; }
        public string ShippingAddress1 { get; set; }
        public string ShippingAddress2 { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingRegion { get; set; }
        public int? ShippingCountryID { get; set; }
        public string ShippingCountry { get; set; }
        public string ShippingZip { get; set; }
        public bool IsBillingSameAsShipping { get; set; }
        public string BillingFirstname { get; set; }
        public string BillingLastname { get; set; }
        public string BillingCompanyName { get; set; }
        public string BillingAddress1 { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingRegion { get; set; }
        public int? BillingCountryID { get; set; }
        public string BillingCountry { get; set; }
        public string BillingZip { get; set; }
        public string Phone { get; set; }
        public decimal? OrderPrice { get; set; }
        public decimal? OrderPricePaid { get; set; }
        public decimal? ShippingPrice { get; set; }
        public decimal? OrderTotalPrice { get; set; }
        public DateTime? OrderPaymentDateTime { get; set; }
        public bool IsOrderDelivered { get; set; }
        public int? PaymentOptionID { get; set; }
        public string PaymentOption { get; set; }
        public string TransactionID { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        #endregion

        #region Sub Classes
        public class OrderDetail
        {
            #region Properties
            public int? OrderDetailID { get; set; }
            public int? OrderID { get; set; }
            public int? OrderDetailProductID { get; set; }
            public int? OrderDetailProductOptionID { get; set; }
            public string OrderDetailProductCaption { get; set; }
            public string ProductSlug { get; set; }
            public int? OrderDetailProductCount { get; set; }
            public decimal? OrderDetailProductPrice { get; set; }
            public decimal? OrderDetailProductPricePaid { get; set; }
            public decimal? OrderDetailProductPricePaidTotal { get; set; }
            public string ProductImageFilename { get; set; }
            public decimal? ProductPrice { get; set; }
            public bool ProductOptionIsArchived { get; set; }
            public DateTime? CRTime { get; set; }
            #endregion
        }
        #endregion
    }

    public class OrderDetailsInsertResponse
    {
        #region Properties
        public int? OrderID { get; set; }
        public int? ProductCount { get; set; }
        #endregion
    }
}
