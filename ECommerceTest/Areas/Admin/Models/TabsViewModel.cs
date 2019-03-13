using Reusables.Core;
using System.Collections.Generic;

namespace ECommerceTest.Areas.Admin.Models
{
    public class TabsViewModel : LayoutModelBase
    {
        #region Properties
        public List<ProjectMenuItem> Tabs { get; set; }
        public bool HasTabs => Tabs?.Count > 0;
        #endregion
    }
}