using Core;
using Core.Properties;
using Core.Utilities;
using ECommerceTest.Controllers;
using Reusables.Core;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ECommerceTest.Models
{
    public class ProductModel
    {
        #region Methods
        public static PageViewModel GetPageViewModel(UrlHelper Url, WebsiteController Controller, string ProductSlug)
        {
            var Product = ProductsDataAccess.GetSingleProductBySlug(ProductSlug);

            if (Product == null)
            {
                return null;
            }
            else
            {
                return new PageViewModel
                {
                    ProductID = Product.ProductID,
                    ProductImageHttpPath = string.IsNullOrWhiteSpace(Product.ProductImageFilename) ? AppSettings.NoImagePath : Utility.GetUploadedFileHttpPath(Product.ProductImageFilename),
                    ProductCaption = Product.ProductCaption,
                    CategoryCaption = Product.CategoryCaption,
                    ProductPrice = LocalUtilities.FormatMoney(Product.ProductPrice),
                    ProductPriceOld = LocalUtilities.FormatMoney(Product.ProductOldPrice),
                    ProductDescription = Product.ProductDescription,
                    Theme = $"theme{Product.CategoryTheme}",
                    IsUserLoggedIn = Controller.IsUserLoggedIn,
                    SignInUrl = Controller.LayoutModel.SignInUrl,
                    AddProductInBasketUrl = Url.RouteUrl(ControllerActionRouteNames.Website.Basket.Add)
                };
            }
        }
        #endregion

        #region Sub Classes
        public class PageViewModel
        {
            #region Properties
            public int? ProductID { get; set; }
            public string ProductCaption { get; set; }
            public string ProductDescription { get; set; }
            public string ProductImageHttpPath { get; set; }
            public string CategoryCaption { get; set; }
            public string AddProductInBasketUrl { get; set; }
            public string ProductPrice { get; set; }
            public bool HasProductPriceOld { get; set; }
            public string ProductPriceOld { get; set; }
            public string Theme { get; set; }
            public bool IsUserLoggedIn { get; set; }
            public string SignInUrl { get; set; }
            public string TextBasketAddSuccess { get; set; } = Resources.TextBasketAddSuccess;            
            #endregion
        }
        #endregion
    }

    public class ProductsByCategoryModel
    {
        #region Methods
        public static PageViewModel GetPageViewModel(string CategorySlug,int? PageNumber, int? ItemsPerPage, UrlHelper Url, WebsiteController Controller)
        {
            var Model = default(PageViewModel);
            var Category = CategoriesDataAccess.GetSingleCategoryBySlug(CategorySlug);

            PageNumber = PageNumber < 1 ? 1 : PageNumber ?? 1;

            if (Category != null)
            {
                Model = new PageViewModel
                {
                    PageTitle = Category.CategoryCaption,
                    AddProductInBasketUrl = Url.RouteUrl(ControllerActionRouteNames.Website.Basket.Add),
                    ShowAddToCartButton = Controller.IsUserLoggedIn
                };

                var Products = ProductsDataAccess.ListProductsFiltered(CategorySlug: CategorySlug, PageNumber: PageNumber, ItemsPerPage: ItemsPerPage);
                if (Products?.Count > 0)
                {
                    var ItemsTotalCount = Products.First().ProductID;
                    Products.RemoveAt(0);

                    Model.PagerItems = Pager.GetPager(
                        ItemsCount: ItemsTotalCount,
                        CurrentPageNumber: PageNumber,
                        ItemsPerPage: ItemsPerPage,
                        CurrentPageHttpPath: Url.RouteUrl(ControllerActionRouteNames.Website.Products.ProductsByCategory),
                        UseQueryStringStyle: false
                    );
                    Model.Products = Products.Select(Item => new PageViewModel.ProductItem
                    {
                        ProductID = Item.ProductID,
                        ProductImageHttpPath = string.IsNullOrWhiteSpace(Item.ProductImageFilename) ? AppSettings.NoImagePath : Utility.GetUploadedFileHttpPath(Item.ProductImageFilename),
                        ProductCaption = Item.ProductCaption,
                        ProductPrice = LocalUtilities.FormatMoney(Item.ProductPrice),
                        CategoryCaption = Item.CategoryCaption,
                        NavigateUrl = Url.RouteUrl(ControllerActionRouteNames.Website.Products.Product, new { Item.ProductSlug }),
                        Theme = $"theme{Item.CategoryTheme}"
                    }).ToList();
                }
            }

            return Model;
        }
        #endregion

        #region Sub Clases
        public class PageViewModel
        {
            #region Properties
            public string PageTitle { get; set; }            
            public string AddProductInBasketUrl { get; set; }
            public string TextBasketAddSuccess { get; set; } = Resources.TextBasketAddSuccess;
            public List<ProductItem> Products { get; set; }
            public bool HasProducts => Products?.Count > 0;
            public bool ShowAddToCartButton { get; set; }
            public List<Pager.Item> PagerItems { get; set; }
            public bool HasPager => PagerItems?.Count > 0;
            #endregion

            #region Sub Classes
            public class ProductItem
            {
                #region Properties                
                public int? ProductID { get; set; }
                public string ProductCaption { get; set; }
                public string CategoryCaption { get; set; }
                public string ProductImageHttpPath { get; set; }
                public string NavigateUrl { get; set; }
                public string Theme { get; set; }
                public string ProductPrice { get; set; }
                #endregion
            }
            #endregion
        }
        #endregion
    }

    public class ProductsModel
    {
        #region Methods
        public static PageViewModel GetPageViewModel(int? PageNumber, int? ItemsPerPage,UrlHelper Url, WebsiteController Controller)
        {
            PageNumber = PageNumber < 1 ? 1 : PageNumber ?? 1;

            var Model = new PageViewModel
            {
                PageTitle = Resources.TextProducts,
                AddProductInBasketUrl = Url.RouteUrl(ControllerActionRouteNames.Website.Basket.Add),
                ShowAddToCartButton = Controller.IsUserLoggedIn                
            };

            var Products = ProductsDataAccess.ListProductsFiltered(CategorySlug: null, PageNumber: PageNumber, ItemsPerPage: ItemsPerPage);
            if (Products?.Count > 0)
            {
                var ItemsTotalCount = Products.First().ProductID;
                Products.RemoveAt(0);

                Model.PagerItems = Pager.GetPager(
                    ItemsCount: ItemsTotalCount,
                    CurrentPageNumber: PageNumber,
                    ItemsPerPage: ItemsPerPage,
                    CurrentPageHttpPath: Url.RouteUrl(ControllerActionRouteNames.Website.Products.Page),
                    UseQueryStringStyle: false
                );

                Model.Products = Products.Select(Item => new PageViewModel.ProductItem
                {
                    ProductID = Item.ProductID,
                    ProductImageHttpPath = string.IsNullOrWhiteSpace(Item.ProductImageFilename) ? AppSettings.NoImagePath : Utility.GetUploadedFileHttpPath(Item.ProductImageFilename),
                    ProductCaption = Item.ProductCaption,
                    ProductPrice = LocalUtilities.FormatMoney(Item.ProductPrice),
                    CategoryCaption = Item.CategoryCaption,
                    NavigateUrl = Url.RouteUrl(ControllerActionRouteNames.Website.Products.Product, new { Item.ProductSlug }),
                    Theme = $"theme{Item.CategoryTheme}"
                }).ToList();
            }

            return Model;
        }
        #endregion

        #region Sub Clases
        public class PageViewModel
        {
            #region Properties
            public string PageTitle { get; set; }
            public string AddProductInBasketUrl { get; set; }
            public string TextBasketAddSuccess { get; set; } = Resources.TextBasketAddSuccess;
            public string NoImagePath { get; set; } = AppSettings.NoImagePath;
            public List<ProductItem> Products { get; set; }
            public bool HasProducts => Products?.Count > 0;
            public bool ShowAddToCartButton { get; set; }
            public List<Pager.Item> PagerItems { get; set; }
            public bool HasPager => PagerItems?.Count > 0;
            #endregion

            #region Sub Classes
            public class ProductItem
            {
                #region Properties                
                public int? ProductID { get; set; }
                public string ProductCaption { get; set; }
                public string CategoryCaption { get; set; }
                public string ProductMetaDescription { get; set; }
                public string ProductImageHttpPath { get; set; }
                public string NavigateUrl { get; set; }
                public string Theme { get; set; }
                public string ProductPrice { get; set; }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}