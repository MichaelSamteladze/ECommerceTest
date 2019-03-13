using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Core.Utilities
{
    public class Breadcrumbs
    {
        #region Properties
        public int ItemsCount => Items?.Count ?? 0;
        public List<BreadCrumbItem> Items { get; set; }
        #endregion Properties

        #region Constructors
        public Breadcrumbs()
        {
            Items = new List<BreadCrumbItem>();
        }

        public Breadcrumbs(IEnumerable<BreadCrumbItem> Items)
        {
            this.Items = Items.ToList();
        }
        #endregion Constructors        

        #region Methods        
        public static Breadcrumbs GetBreadcrumbsByPageUrl(List<Permission> Permissions, string PageUrl)
        {
            var Items = new List<BreadCrumbItem>();
            //var Item = Permissions.Where(p => p.PagePath?.ToLower() == PageUrl || (!string.IsNullOrWhiteSpace(p.PagePath) && Regex.IsMatch(PageUrl, p.PagePath.ToLower()))).LastOrDefault();

            var Item = default(Permission);
            foreach (var Permission in Permissions)
            {
                var UrlToCompare = Permission.PagePath?.ToLower();
                if (UrlToCompare == PageUrl || (!string.IsNullOrWhiteSpace(Permission.PagePath) && Regex.IsMatch(PageUrl, $"{UrlToCompare}+$")))
                {
                    Item = Permission;
                }
            }

            if (Item != null)
            {
                Items.Add(new BreadCrumbItem { Caption = Item.Caption, IsLastItem = true });
            }

            while (Item != null)
            {
                Item = Permissions.Where(p => p.PermissionID == Item.ParentID).FirstOrDefault();
                if (Item != null)
                {
                    Items.Add(new BreadCrumbItem { Caption = Item.Caption, NavigateUrl = Item.PagePath });
                }
            }

            Items.Reverse();
            return new Breadcrumbs
            {
                Items = Items
            };
        }

        public void AddItem(BreadCrumbItem NewItem)
        {
            if (Items != null && NewItem != null)
            {
                foreach(var Item in Items)
                {
                    Item.IsLastItem = false;
                }                
                NewItem.IsLastItem = true;
                Items.Add(NewItem);
            }
        }

        public void UpdateItem(BreadCrumbItem NewItem, int Index)
        {
            if (Items != null && NewItem != null && Index < Items.Count)
            {
                if(Index == Items.Count-1)
                {
                    NewItem.IsLastItem = true;
                }
                Items[Index] = NewItem;
            }
        }

        public void DeleteItem(int Index)
        {
            if (Items?.Count > Index && Index >= 0)
            {
                Items[Index-1].IsLastItem = Index == Items.Count - 1;
                
                Items.RemoveAt(Index);
            }
        }
        #endregion Methods

        #region Sub Classes
        public class BreadCrumbItem
        {
            #region Properties
            public string Caption { get; set; }
            public bool HasNavigateUrl => !string.IsNullOrWhiteSpace(NavigateUrl);
            public string NavigateUrl { get; set; }
            public bool IsLastItem { get; set; }
            #endregion
        }
        #endregion
    }

    public class Pager
    {
        #region Methods
        public static List<Item> GetPager(int? ItemsCount, int? CurrentPageNumber, int? ItemsPerPage, string CurrentPageHttpPath, bool UseQueryStringStyle)
        {
            CurrentPageNumber = CurrentPageNumber.HasValue ? CurrentPageNumber : 1;

            if (UseQueryStringStyle)
            {
                CurrentPageHttpPath = CurrentPageHttpPath.Replace($"?page={CurrentPageNumber}", "").Replace($"&page={CurrentPageNumber}", "");
            }
            else
            {
                CurrentPageHttpPath = CurrentPageHttpPath.Replace($"/page-{CurrentPageNumber}", "");
            }

            var Parts = CurrentPageHttpPath.Split('?');
            var RootUrl = Parts[0].TrimEnd('/');
            var QueryString = Parts.Length > 1 ? Parts[1] : "";
            var QueryStringSeparator = "&";
            var IsQueryStringEmpty = string.IsNullOrEmpty(QueryString);

            if (!IsQueryStringEmpty) { QueryString = $"?{QueryString}"; }

            if (IsQueryStringEmpty && UseQueryStringStyle) { QueryStringSeparator = "?"; }


            if (ItemsCount.HasValue && ItemsPerPage.HasValue)
            {
                List<Item> Pager = new List<Item>();

                var PageCount = Convert.ToInt32(Math.Ceiling((decimal)(ItemsCount.Value) / (ItemsPerPage.Value)));
                if (PageCount < 2)
                {
                    return null;
                }
                else if (PageCount < 11)
                {
                    Pager = Enumerable.Range(1, PageCount).Select(PageNumber => new Item
                    {
                        Text = PageNumber.ToString(),
                        Url = GetPagerItemUrl(CurrentPageHttpPath, RootUrl, UseQueryStringStyle, QueryString, QueryStringSeparator, PageNumber),
                        IsActive = CurrentPageNumber == PageNumber
                    }).ToList();
                }
                else
                {
                    const int PagesOffset = 3;

                    for (var i = 1; (i <= PagesOffset && CurrentPageNumber - i > 0); i++)
                    {
                        var PageNumber = CurrentPageNumber - i;
                        Pager.Insert(0, new Item { Text = PageNumber.ToString(), Url = GetPagerItemUrl(CurrentPageHttpPath, RootUrl, UseQueryStringStyle, QueryString, QueryStringSeparator, PageNumber) });
                    }

                    if (CurrentPageNumber - PagesOffset > 1)
                    {
                        Pager.Insert(0, new Item { Text = "..." });
                        Pager.Insert(0, new Item { Text = "1", Url = CurrentPageHttpPath });
                    }

                    Pager.Add(new Item { Text = CurrentPageNumber.ToString(), IsActive = true });

                    for (var i = 1; (i <= PagesOffset && CurrentPageNumber + i <= PageCount); i++)
                    {
                        var PageNumber = CurrentPageNumber + i;
                        Pager.Add(new Item { Text = PageNumber.ToString(), Url = GetPagerItemUrl(CurrentPageHttpPath, RootUrl, UseQueryStringStyle, QueryString, QueryStringSeparator, PageNumber) });
                    }

                    if (CurrentPageNumber + PagesOffset < PageCount)
                    {
                        Pager.Add(new Item { Text = "..." });
                        Pager.Add(new Item { Text = PageCount.ToString(), Url = GetPagerItemUrl(CurrentPageHttpPath, RootUrl, UseQueryStringStyle, QueryString, QueryStringSeparator, PageCount) });
                    }
                }


                if (CurrentPageNumber > 1)
                {
                    Pager.Insert(0, new Item { Text = "&lt; Prev ", Url = GetPagerItemUrl(CurrentPageHttpPath, RootUrl, UseQueryStringStyle, QueryString, QueryStringSeparator, CurrentPageNumber - 1) });
                }

                if (CurrentPageNumber < PageCount)
                {
                    Pager.Add(new Item { Text = "Next &gt;", Url = GetPagerItemUrl(CurrentPageHttpPath, RootUrl, UseQueryStringStyle, QueryString, QueryStringSeparator, CurrentPageNumber + 1) });
                }

                return Pager;
            }
            else
            {
                return null;
            }
        }

        static string GetPagerItemUrl(string CurrentPageHttpPath, string RootUrl, bool UseQueryStringStyle, string QueryString, string QueryStringSeparator, int? PageNumber)
        {
            return PageNumber == 1 ?
            CurrentPageHttpPath :
            $"{RootUrl}{(UseQueryStringStyle ? $"{QueryString}{QueryStringSeparator}page={PageNumber}" : $"/page-{PageNumber}/{QueryString}")}";
        }
        #endregion

        #region Sub Classes
        public class Item
        {
            #region Properties
            public string Text { get; set; }
            public string Url { get; set; }
            public bool HasUrl => !string.IsNullOrWhiteSpace(Url);
            public bool IsActive { get; set; }
            #endregion Properties
        }
        #endregion
    }
}
