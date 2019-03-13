using Core.DB;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class ProductsDataAccess : SixtyThreeBitsDataObject
    {
        #region Methods
        public static Product GetSingleProductByID(int? ProductID)
        {
            return TryToReturnStatic($"{nameof(GetSingleProductByID)}({nameof(ProductID)} = {ProductID})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.ProductsGetSingleByID(ProductID).DeserializeTo<Product>();
                }
            });
        }

        public static Product GetSingleProductBySlug(string ProductSlug)
        {
            return TryToReturnStatic($"{nameof(GetSingleProductBySlug)}({nameof(ProductSlug)} = {ProductSlug})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.ProductsGetSingleBySlug(ProductSlug).DeserializeTo<Product>();
                }
            });
        }

        public static bool IsProductSlugUniq(string Slug, int? ProductID = null)
        {
            return TryToReturnStatic($"{nameof(IsProductSlugUniq)}({nameof(Slug)} = {Slug}, {nameof(ProductID)} = {ProductID})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.ProductsIsSlugUniq(Slug, ProductID).Value;
                }
            });
        }

        public static List<ProductsListResult> ListProducts(bool? IsPublished = null, string CategorySlug = null)
        {
            return TryToReturnStatic($"{nameof(ListProducts)}({nameof(IsPublished)} = {IsPublished}, {nameof(CategorySlug)} = {CategorySlug}, )", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.ProductsList(IsPublished, CategorySlug).OrderByDescending(Item => Item.CRTime).ToList();
                }
            });
        }

        public static List<ProductsListFilteredResult> ListProductsFiltered(string CategorySlug,int? PageNumber, int? ItemsPerPage)
        {
            return TryToReturnStatic($"{nameof(ListProductsFiltered)}({nameof(CategorySlug)} = {CategorySlug}, {nameof(PageNumber)} = {PageNumber}, {nameof(ItemsPerPage)} = {ItemsPerPage})", () =>
            {
                using(var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.ProductsListFiltered(CategorySlug, PageNumber, ItemsPerPage).ToList();
                }
            });
        }

        public int? ProductsIUD(byte? DatabaseAction, int? ProductID = null, string ProductSlug = null, int? CategoryID = null, string ProductCaption = null, decimal? ProductPrice = null, decimal? ProductOldPrice = null, string ProductDescription = null, string ProductImageFilename = null, bool? IsPublished = null)
        {
            return TryToReturn($"{nameof(ProductsIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(ProductID)} = {ProductID}, {nameof(ProductSlug)} = {ProductSlug}, {nameof(CategoryID)} = {CategoryID}, {nameof(ProductCaption)} = {ProductCaption}, {nameof(ProductPrice)} = {ProductPrice}, {nameof(ProductOldPrice)} = {ProductOldPrice}, {nameof(ProductDescription)} = {ProductDescription}, {nameof(ProductImageFilename)} = {ProductImageFilename}, {nameof(IsPublished)} = {IsPublished}, )", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    db.ProductsIUD(DatabaseAction, ref ProductID, ProductSlug, CategoryID, ProductCaption, ProductPrice, ProductOldPrice, ProductDescription, ProductImageFilename, IsPublished);
                    return ProductID;
                }
            });
        }


  
 

        #endregion
    }

    public class Product
    {
        #region Properties
        public int? ProductID { get; set; }
        public string ProductSlug { get; set; }
        public int? CategoryID { get; set; }
        public string CategoryCaption { get; set; }
        public string CategoryTheme { get; set; }
        public string ProductCaption { get; set; }        
        public decimal? ProductPrice { get; set; }
        public decimal? ProductOldPrice { get; set; }
        public string ProductMetaDescription { get; set; }
        public string ProductDescription { get; set; }
        
        
        public string ProductImageFilename { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? CRTime { get; set; }
        #endregion        
    }
}
