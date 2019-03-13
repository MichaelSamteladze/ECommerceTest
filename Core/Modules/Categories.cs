using Core.DB;
using Core.Utilities;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class CategoriesDataAccess : SixtyThreeBitsDataObject
    {
        #region Mrthods
        public int? CategoriesIUD(byte? DatabaseAction, int? CategoryID = null, int? CategoryTheme = null, string CategoryCaption = null, string CategoryDescription = null, string CategoryImageFilename = null, string CategorySlug = null, bool? IsPublished = null)
        {
            return TryToReturn($"{nameof(CategoriesIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(CategoryID)} = {CategoryID}, {nameof(CategoryTheme)} = {CategoryTheme}, {nameof(CategoryCaption)} = {CategoryCaption}, {nameof(CategoryDescription)} = {CategoryDescription}, {nameof(CategoryImageFilename)} = {CategoryImageFilename}, {nameof(CategorySlug)} = {CategorySlug}, {nameof(IsPublished)} = {IsPublished}, )", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    db.CategoriesIUD(DatabaseAction, ref CategoryID, CategoryTheme, CategoryCaption, CategoryDescription, CategoryImageFilename, CategorySlug, IsPublished);
                    return CategoryID;
                }
            });
        }

        public void CategoriesSyncParentsAndSortIndexes(List<SyncSortIndexesItem> SortIndexes)
        {
            TryExecute($"{nameof(CategoriesSyncParentsAndSortIndexes)}({nameof(SortIndexes)} = {SortIndexes.ToXml()})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    db.CategoriesSyncParentsAndSortIndexes(SortIndexes.ToXElement());
                }
            });
        }

        public static Category GetSingleCategoryByID(int? CategoryID)
        {
            return TryToReturnStatic($"{nameof(GetSingleCategoryByID)}({nameof(CategoryID)} = {CategoryID})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.CategoriesGetSingleByID(CategoryID).DeserializeTo<Category>();
                }
            });
        }

        public static Category GetSingleCategoryBySlug(string CategorySlug)
        {
            return TryToReturnStatic($"{nameof(GetSingleCategoryBySlug)}({nameof(CategorySlug)} = {CategorySlug})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.CategoriesGetSingleBySlug(CategorySlug).DeserializeTo<Category>();
                }
            });
        }

        public static bool IsCategorySlugUniq(string Slug, int? CategoryID = null)
        {
            return TryToReturnStatic($"{nameof(IsCategorySlugUniq)}({nameof(Slug)} = {Slug}, {nameof(CategoryID)} = {CategoryID})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.CategoriesIsSlugUniq(Slug, CategoryID).Value;
                }
            });
        }

        public static List<CategoriesListResult> ListCategories(bool? IsPublished = null)
        {
            return TryToReturnStatic($"{nameof(ListCategories)}({nameof(IsPublished)} = {IsPublished})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.CategoriesList(IsPublished).OrderBy(c=>c.SortIndex).ToList();
                }
            });
        }

        public static List<SimpleKeyValue<int?, string>> ListCategoriesAsKeyValue()
        {
            return ListCategories(true).Select(Item => new SimpleKeyValue<int?, string> { Key = Item.CategoryID, Value = Item.CategoryCaption }).ToList();
        }
        #endregion
    }

    public class Category
    {
        #region Properties
        public int? CategoryID { get; set; }
        public int? CategoryTheme { get; set; }
        public string CategoryCaption { get; set; }
        public string CategoryDescription { get; set; }
        public string CategoryImageFilename { get; set; }
        public string CategorySlug { get; set; }
        public bool IsPublished { get; set; }
        public int? SortIndex { get; set; }
        public DateTime? CRTime { get; set; }
        #endregion
    }
}
