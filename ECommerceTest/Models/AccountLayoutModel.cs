using Reusables.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECommerceTest.Models
{
    public class AccountLayoutModel : LayoutModelBase
    {
        #region Properties
        public List<ProjectMenuItem> Tabs { get; set; }
        public bool HasTabs => Tabs?.Count > 0;
        public string CurrentUrl { get; set; }
        #endregion
    }
}