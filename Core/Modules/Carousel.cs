using Core.DB;
using Core.Utilities;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class CarouselDataAccess : SixtyThreeBitsDataObject
    {
        #region Methods
        public int? CarouselIUD(byte? DatabaseAction, int? CarouselID = null, string CarouselCaption = null, string CarouselText = null, string CarouselUrl = null, string CarouselImageFilename = null, bool? IsPublished = null)
        {
            return TryToReturn($"{nameof(CarouselIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(CarouselID)} = {CarouselID}, {nameof(CarouselCaption)} = {CarouselCaption}, {nameof(CarouselText)} = {CarouselText}, {nameof(CarouselUrl)} = {CarouselUrl}, {nameof(CarouselImageFilename)} = {CarouselImageFilename}, {nameof(IsPublished)} = {IsPublished})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    db.CarouselIUD(DatabaseAction, ref CarouselID, CarouselCaption, CarouselText, CarouselUrl, CarouselImageFilename, IsPublished);
                    return CarouselID;
                }
            });
        }

        public static List<CarouselListResult> ListCarouselItems(bool? IsPublished = null)
        {
            return TryToReturnStatic($"{nameof(ListCarouselItems)}({nameof(IsPublished)} = {IsPublished})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.CarouselList(IsPublished).OrderBy(Item => Item.SortIndex).ToList();
                }
            });
        }

        public static CarouselItem GetSingleCarouselItemByID(int? CarouselID)
        {
            return TryToReturnStatic($"{nameof(GetSingleCarouselItemByID)}({nameof(CarouselID)} = {CarouselID})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.CarouselGetSingleItemByID(CarouselID).DeserializeTo<CarouselItem>();
                }
            });
        }

        public void SyncSortIndexes(List<SyncSortIndexesItem> SortIndexes)
        {
            TryExecute($"{nameof(SyncSortIndexes)}({nameof(SortIndexes)} = {SortIndexes.ToXml()})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    db.CarouselSyncSortIndexes(SortIndexes.ToXElement());
                }
            });
        }
        #endregion
    }

    public class CarouselItem
    {
        #region Properties
        public int? CarouselID { get; set; }
        public string CarouselCaption { get; set; }
        public string CarouselText { get; set; }
        public string CarouselUrl { get; set; }
        public string CarouselImageFilename { get; set; }
        public int? SortIndex { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? CRTime { get; set; }
        #endregion
    }
}
