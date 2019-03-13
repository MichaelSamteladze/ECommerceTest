using Core.Utilities;
using Reusables.Core;
using System.Collections.Generic;

namespace ECommerceTest.Models
{
    public class WebsiteLayoutModel : LayoutModelBase
    {
        #region Properties
        public string WebsiteDomain { get; set; }
        public string CurrentUrl { get; set; }
        public string SignInUrl { get; set; }
        public string SignUpUrl { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhoneNumber { get; set; }        
        public string WebsiteAddress { get; set; } = AppSettings.WebsiteAddress;
        public string BasketUrl { get; set; }        
        public int? ProductsCountInBasket { get; set; }
        public bool HasProductsInBasket => ProductsCountInBasket > 0;
        public List<ProjectMenuItem> AccountMenu { get; set; }        
        #endregion
    }
}