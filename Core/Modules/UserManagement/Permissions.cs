using Core.DB;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class PermissionsDataAccess : SixtyThreeBitsDataObject
    {
        #region Methods
        public void DeleteRecursive(int? PermissionID)
        {
            TryExecute($"{nameof(DeleteRecursive)}({nameof(PermissionID)} = {PermissionID}, )", () =>
            {
                using(var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    db.PermissionsDeleteRecursive(PermissionID);
                }
            });
        }

        public static List<PermissionsListResult> ListPermissions()
        {
            return TryToReturnStatic($"{nameof(ListPermissions)}()", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.PermissionsList().OrderBy(P => P.SortIndex).ToList();
                }
            });
        }

        public int? PermissionsIUD(byte? DatabaseAction = null, int? PermissionID = null, int? ParentID = null, string Caption = null, string PagePath = null, string CodeName = null, string PermissionCode = null, int? SortIndex = null, bool? IsMenuItem = null,string MenuIcon = null)
        {
            return TryToReturn($"{nameof(PermissionsIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(PermissionID)} = {PermissionID}, {nameof(ParentID)} = {ParentID}, {nameof(Caption)} = {Caption}, {nameof(PagePath)} = {PagePath}, {nameof(CodeName)} = {CodeName}, {nameof(PermissionCode)} = {PermissionCode}, {nameof(SortIndex)} = {SortIndex}, {nameof(IsMenuItem)} = {IsMenuItem}, {nameof(MenuIcon)} = {MenuIcon})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    db.PermissionsIUD(DatabaseAction, ref PermissionID, ParentID, Caption, PagePath, CodeName, PermissionCode, IsMenuItem, MenuIcon, SortIndex);
                    return PermissionID;
                }
            });
        } 
        #endregion
    }

    [Serializable]
    public class Permission
    {
        #region Properties
        public int? PermissionID { get; set; }
        public int? ParentID { get; set; }
        public string Caption { get; set; }
        public string PagePath { get; set; }
        public string CodeName { get; set; }
        public string PermissionCode { get; set; }
        public bool IsMenuItem { get; set; }
        public string MenuIcon { get; set; }
        public bool IsSelected { get; set; }
        public int? SortIndex { get; set; } 
        #endregion
    }    
}