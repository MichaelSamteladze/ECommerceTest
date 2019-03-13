using Core.DB;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Core
{
    public class UsersDataAccess : SixtyThreeBitsDataObject
    {
        #region Methods
        public void DeleteUser(int? UserID)
        {
            TryExecute($"{nameof(DeleteUser)}({nameof(UserID)} = {UserID})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    db.UsersDelete(UserID);
                }
            });
        }

        public static User GetSingleUserByEmail(string Email)
        {
            return TryToReturnStatic($"{nameof(GetSingleUserByEmail)}({nameof(Email)} = {Email})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.UsersGetSingleByEmail(Email).DeserializeTo<User>();
                }
            });
        }

        public static User GetSingleUserByID(int? UserID)
        {
            return TryToReturnStatic($"{nameof(GetSingleUserByID)}({nameof(UserID)} = {UserID})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.UsersGetSingleUserByUserID(UserID).DeserializeTo<User>();
                }
            });
        }        

        public static User GetSingleUserByEmailAndPassword(string Email,string Password)
        {
            return TryToReturnStatic($"{nameof(GetSingleUserByEmailAndPassword)}({nameof(Email)} = {Email}, {nameof(Password)} = {Password})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.UsersGetSingleUserByEmailAndPassword(Email, Password).DeserializeTo<User>();
                }
            });
        }
                
        public static bool IsEmailUniq(string Email,int? UserID = null)
        {
            return TryToReturnStatic($"{nameof(IsEmailUniq)}({nameof(Email)} = {Email}, {nameof(UserID)} = {UserID})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.UsersIsEmailUnique(Email,UserID).Value;
                }
            });
        }
        
        public static List<UsersListResult> ListUsers()
        {
            return TryToReturnStatic($"{nameof(ListUsers)}()", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.UsersList().OrderByDescending(Item => Item.CRTime).ToList();
                }
            });
        }                

        public int? UsersIUD(byte? DatabaseAction, int? UserID = null, string Email = null, string Password = null, string Firstname = null, string Lastname = null, string Fullname = null, DateTime? Birthdate = null, string Mobile = null, string Avatar = null, bool? IsActive = null)
        {
            return TryToReturn($"{nameof(UsersIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(UserID)} = {UserID}, {nameof(Email)} = {Email}, {nameof(Password)} = {Password}, {nameof(Firstname)} = {Firstname}, {nameof(Lastname)} = {Lastname}, {nameof(Fullname)} = {Fullname}, {nameof(Birthdate)} = {Birthdate}, {nameof(Mobile)} = {Mobile}, {nameof(Avatar)} = {Avatar}, {nameof(IsActive)} = {IsActive})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    db.UsersIUD(DatabaseAction, ref UserID, Email, Password, Firstname, Lastname, Birthdate, Mobile, Avatar, IsActive);
                    return UserID;
                }
            });
        }
        #endregion Methods
    }

    [Serializable]
    public class User
    {
        #region Properties
        public int? UserID { get; set; }
        public string Fullname { get { return $"{Firstname} {Lastname}"; } }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime? Birthdate { get; set; }
        public string FacebookID { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public int? RoleID { get; set; }
        public int? CompanyID { get; set; }
        public string Avatar { get; set; }       
        public List<Permission> Permissions { get; set; }  
        public int? ProductCountInBasket { get; set; }
        #endregion Properties

        #region Methods
        public List<Permission> GetChildPermissionsByParent(string ParentPermission)
        {
            List<Permission> ChildPermissions = null;
            var ParentPermissionItem = GetPermission(ParentPermission);

            if(ParentPermissionItem!=null)
            {
                ChildPermissions = Permissions.Where(Item => Item.ParentID == ParentPermissionItem.PermissionID).ToList();
            }                

            return ChildPermissions;
        }

        public Permission GetPermission(string Permission)
        {
            if (string.IsNullOrWhiteSpace(Permission))
            {
                return null;
            }
            else
            {
                return Permissions?.Where(P => P.CodeName == Permission || P.PermissionCode == Permission || (!string.IsNullOrWhiteSpace(P.PagePath) && Regex.IsMatch(Permission, $"^{P.PagePath}*$"))).LastOrDefault();
            }
        }      

        public string GetPermissionNameByPagePath(string PagePath)
        {
            return GetPermission(PagePath)?.Caption;
        }

        public bool HasPermission(string Permission)
        {
            if (IsAdmin || string.IsNullOrWhiteSpace(Permission))
            {
                return true;
            }
            else
            {
                return Permissions?.Any(P => P.CodeName == Permission || P.PermissionCode == Permission || P.PagePath == Permission || (!string.IsNullOrWhiteSpace(P.PagePath) && Regex.IsMatch(Permission, $"^{P.PagePath}*$"))) == true;
            }
        }
        #endregion Methods

        #region Sub Classes
        [Serializable]
        public class BusinessUnit
        {
            #region Properties
            public int? BusinessUnitID { get; set; }
            public string BusinessUnitCaption { get; set; }
            public string BusinessUnitCode { get; set; }
            #endregion
        }
        #endregion
    }    
}