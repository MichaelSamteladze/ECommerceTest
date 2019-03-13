using Core.Utilities;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;
using System.Linq;

namespace Reusables.Core
{
    public class ProjectMenuItem
    {
        #region Properties
        public string Caption { get; set; }
        public string Icon { get; set; }
        public string NavigateUrl { get; set; }
        public bool IsHashTag => NavigateUrl == "#";
        public bool IsSelected { get; set; }
        public bool IsHomePage { get; set; }
        public bool HasChildren => Children?.Count > 0;
        public List<ProjectMenuItem> Children { get; set; } 
        #endregion
    }

    public class TreeNodeItem
    {
        #region Properties
        public int? NodeID { get; set; }
        public int? ParentID { get; set; }
        public string Filename { get; set; }
        public string NavigateUrl { get; set; }
        public bool HasNavigateUrl => !string.IsNullOrEmpty(NavigateUrl);
        public bool IsTargetBlank { get; set; }
        public string Caption { get; set; }
        public string CaptionEng { get; set; }
        public bool IsToggler1Checked { get; set; }
        public bool IsToggler2Checked { get; set; }
        public string LanguageCode { get; set; }
        public string Hash => new SimpleKeyValue<int?, string> { Key = NodeID, Value = Filename }.ToJSON().EncryptWeb();
        public bool IsFolder { get; set; }
        public bool HasChildren => Children?.Count > 0;
        public List<TreeNodeItem> Children { get; set; }

        public bool IsDisabled { get; set; }

        public bool ShowAddNewButton { get; set; }
        public bool ShowEditButton { get; set; }
        public bool ShowDeleteButton { get; set; }
        public bool ShowToggler1 { get; set; }
        public bool ShowToggler2 { get; set; }
        public bool ShowCustomButton { get; set; }
        public bool ShowCustomButtonFirst { get; set; }
        public bool ShowCustomButtonLast => !ShowCustomButtonFirst;

        public string CustomButtonIcon { get; set; }
        public string TextToggler1 { get; set; } = "Published?";
        public string TextToggler2 { get; set; } = "Menu?";
        #endregion

        #region Methods
        public static SimpleKeyValue<int?, string> DecodeHash(string Hash)
        {
            return Hash.DecryptWeb().FromJsonTo<SimpleKeyValue<int?, string>>();
        }

        public static List<TreeNodeItem> ConvertToRecursive(List<TreeNodeItem> TreeNodesFlat, int? ParentID = null)
        {
            var FileAttachmentsNode = new List<TreeNodeItem>();

            if (TreeNodesFlat?.Count > 0)
            {
                FileAttachmentsNode = TreeNodesFlat.Where(Item => Item.ParentID == ParentID).ToList();
                FileAttachmentsNode.ForEach(Item =>
                {
                    Item.Children = ConvertToRecursive(TreeNodesFlat, Item.NodeID);
                });
            }

            return FileAttachmentsNode;
        }
        #endregion
    }

    public class SyncSortIndexesModel
    {
        #region Properties
        public List<SyncSortIndexesItem> SortIndexes { get; set; }
        #endregion
    }
}