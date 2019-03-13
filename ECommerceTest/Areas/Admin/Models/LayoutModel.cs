using Reusables.Core;

namespace ECommerceTest.Areas.Admin.Models
{
    public class AdminLayoutModel : LayoutModelBase
    {
        #region Properties                
        public string UrlCurrent { get; set; }
        public string UrlLogout { get; set; }        
        public string UrlWebsite { get; set; }
        #endregion
    }
}