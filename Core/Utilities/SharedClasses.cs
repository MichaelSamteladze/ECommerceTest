namespace Core.Utilities
{
    public class SyncSortIndexesItem
    {
        #region Properties
        public int? ID { get; set; }
        public int? ParentID { get; set; }
        public int? SortIndex { get; set; }
        #endregion

        #region Serialization
        public bool ShouldSerializeID() { return ID.HasValue; }
        public bool ShouldSerializeParentID() { return ParentID.HasValue; }
        public bool ShouldSerializeSortIndex() { return SortIndex.HasValue; }
        #endregion
    }
}
