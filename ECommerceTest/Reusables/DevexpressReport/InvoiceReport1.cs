using Core.Utilities;
using static Core.Utilities.Constants;

namespace Core.Reporting
{
    public partial class InvoiceReport1 : DevExpress.XtraReports.UI.XtraReport
    {
        public InvoiceReport1(int? OrderID)
        {
            InitializeComponent();
            var Order = OrdersDataAccess.GetSingleOrderByID(OrderID);
            InitInvoice(Order);
        }

        void InitInvoice(Order O)
        {
            if (O != null)
            {
                LogoPictureBox.ImageUrl = $"{AppSettings.WebsiteAddress}content/images/logo.png";

                CustomerCell.Text = O.Customer;
                InvoiceNumberCell.Text = O.OrderID.ToString();
                PurchaseTypeCell.Text = O.PaymentOption;
                PurchaseDateCell.Text = string.Format(Formats.DateEval, O.OrderPaymentDateTime);
                ShippingCountryCell.Text = O.ShippingCountry;
                ShippingRegionCell.Text = O.ShippingRegion;
                ShippingCityCell.Text = O.ShippingCity;
                ShippingZipCell.Text = O.ShippingZip;
                ShippingAddress1Cell.Text = O.ShippingAddress1;
                ShippingAddress2Cell.Text = O.ShippingAddress2;
                BillingCountryCell.Text = O.BillingCountry;
                BillingRegionCell.Text = O.BillingRegion;
                BillingCityCell.Text = O.BillingCity;
                BillingZipCell.Text = O.BillingZip;
                BillingAddress1Cell.Text = O.BillingAddress1;
                BillingAddress2Cell.Text = O.BillingAddress2;


                Order.OrderDetail OrderDetail;
                DataSource = O.OrderDetails;               
                
                OrderDetailProductCellText.DataBindings.Add(nameof(OrderDetailProductCellText.Text), DataSource, nameof(OrderDetail.OrderDetailProductCaption));
                OrderDetailProductCountCellText.DataBindings.Add(nameof(OrderDetailProductCountCellText.Text), DataSource, nameof(OrderDetail.OrderDetailProductCount));
                OrderDetailProductPriceCellText.DataBindings.Add(nameof(OrderDetailProductPriceCellText.Text), DataSource, nameof(OrderDetail.OrderDetailProductPricePaid), $"$ {Formats.Decimal2FractionsEval}");
                OrderDetailProductPriceSumCellText.DataBindings.Add(nameof(OrderDetailProductPriceSumCellText.Text), DataSource, nameof(OrderDetail.OrderDetailProductPricePaidTotal), $"$ {Formats.Decimal2FractionsEval}");
                                
                TotalCell.Text = $"$ {string.Format(Formats.Decimal2FractionsEval, O.OrderTotalPrice)}";
                PaidPicture.Visible = true;
            }

            
        }
        
        private void PagerLabel_PrintOnPage(object sender, DevExpress.XtraReports.UI.PrintOnPageEventArgs e)
        {
            PagerLabel.Text = (e.PageIndex + 1).ToString();
        }
    }
}
