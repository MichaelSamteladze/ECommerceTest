using ECommerceTest.Models;
using Reusables.Core;
using System.Web.Mvc;

namespace ECommerceTest.Controllers
{
    public class ProductsController : WebsiteController
    {
        const int ItemsPerPage = 4;

        [Route("products", Name = ControllerActionRouteNames.Website.Products.Page)]
        [Route("products/page-{PageNumber:int}")]
        public ActionResult Products(int? PageNumber)
        {
            var Model = ProductsModel.GetPageViewModel(PageNumber, ItemsPerPage, Url, this);
            LocalUtilities.SetPageTitle<WebsiteLayoutModel>(ViewData, Model.PageTitle);
            return View(ViewNames.Website.Products.Page, Model);
        }

        [Route("products/{CategorySlug}", Name = ControllerActionRouteNames.Website.Products.ProductsByCategory)]
        [Route("products/{CategorySlug}/page-{PageNumber:int}")]
        public ActionResult ProductsByCategory(string CategorySlug,int? PageNumber)
        {
            var Model = ProductsByCategoryModel.GetPageViewModel(CategorySlug, PageNumber, ItemsPerPage, Url, this);
            if (Model == null)
            {
                return NotFound(WithHttpStatusCode: true);
            }
            else
            {
                LocalUtilities.SetPageTitle<WebsiteLayoutModel>(ViewData, Model.PageTitle);
                return View(ViewNames.Website.Products.ProductsByCategory, Model);
            }
        }

        [Route("product/{ProductSlug}", Name = ControllerActionRouteNames.Website.Products.Product)]
        public ActionResult ProductDetails(string ProductSlug)
        {
            var Model = ProductModel.GetPageViewModel(Url, this, ProductSlug);
            if (Model == null)
            {
                return NotFound(WithHttpStatusCode: true);
            }
            else
            {
                LocalUtilities.SetPageTitle<WebsiteLayoutModel>(ViewData: ViewData, PageTitle: Model.ProductCaption);
                return  View(ViewNames.Website.Products.Product, Model);
            }
        }
    }
}