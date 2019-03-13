using Core.DB;
using Core.Utilities;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Core
{
    public class FaqDataAccess : SixtyThreeBitsDataObject
    {
        #region Methods
        public static List<FaqListResult> ListFaq(bool? IsPublished = null)
        {
            return TryToReturnStatic($"{nameof(ListFaq)}({nameof(IsPublished)} = {IsPublished})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.FaqList(IsPublished).OrderBy(f => f.SortIndex).ToList();
                }
            });
        }

        public int? FaqIUD(byte? DatabaseAction, int? FaqID = null, string Question = null, string Answer = null, int? SortIndex = null, bool? IsPublished = null)
        {
            return TryToReturn($"{nameof(FaqIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(FaqID)} = {FaqID}, {nameof(Question)} = {Question}, {nameof(Answer)} = {Answer}, {nameof(SortIndex)} = {SortIndex}, {nameof(IsPublished)} = {IsPublished}, )", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    db.FaqIUD(DatabaseAction, ref FaqID, Question, Answer, SortIndex, IsPublished);
                    return FaqID;
                }
            });
        }

        public void SyncSortIndexes(List<SyncSortIndexesItem> SortIndexes)
        {
            TryExecute($"{nameof(SyncSortIndexes)}({nameof(SortIndexes)} = {SortIndexes.ToXml()})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    db.FaqSyncSortIndexes(SortIndexes.ToXElement());
                }
            });
        }
        #endregion
    }

    public class FaqItem
    {
        #region Properties
        public int? FaqID { get; set; }
        public string Question { get; set; }        
        public string Answer { get; set; }        
        public bool IsPublished { get; set; }
        #endregion
    }
}
