namespace Reusables.Core
{
    public class ControllerActionRouteNames
    {
        public class Admin
        {
            #region Sub Classes
            public class Auth
            {
                #region Properties
                public const string Login = "AdminAuthLogin";
                public const string Logout = "AdminAuthLogout";                
                #endregion
            }

            public class Carousel
            {
                #region Properties
                public const string Page = "AdminCarousel";
                public const string Grid = "AdminCarouselGrid";
                public const string GridAdd = "AdminCarouselGridAdd";
                public const string GridUpdate = "AdminCarouselGridUpdate";
                public const string GridDelete = "AdminCarouselGridDelete";
                public const string GridSyncSortIndexes = "AdminCarouselGridSyncSortIndexes";
                #endregion

                #region Sub Classes
                public class CarouselItem
                {
                    #region Properties
                    public const string DeleteImage = "AdminCarouselCarouselItemDeleteImage";
                    public const string Properties = "AdminCarouselCarouselItemProperties";
                    #endregion
                }
                #endregion
            }

            public class Categories
            {
                #region Properties
                public const string Index = "AdminCategories";
                public const string AddNew = "AdminCategoriesAddNew";
                public const string Update = "AdminCategoriesUpdate";
                public const string Delete = "AdminCategoriesDelete";
                public const string SyncParentsAndSortIndexes = "AdminCategoriesSyncParentsAndSortIndexes";
                #endregion

                #region Sub Classes
                public class Category
                {
                    #region Properties
                    public const string Properties = "AdminCategoriesCategoryProperties";
                    public const string DeleteImage = "AdminCategoriesCategoryPropertiesDeleteImage";
                    #endregion
                }
                #endregion
            }

            public class Dictionaries
            {
                #region Properties
                public const string Page = "AdminDictionaries";
                public const string DictionariesTree = "AdminDictionariesTree";
                public const string DictionariesTreeAdd = "AdminDictionariesTreeAdd";
                public const string DictionariesTreeUpdate = "AdminDictionariesTreeUpdate";
                public const string DictionariesTreeUpdateParent = "AdminDictionariesTreeUpdateParent";
                public const string DictionariesTreeDelete = "AdminDictionariesTreeDelete";                
                #endregion
            }
       
            public class Faq
            {
                #region Properties
                public const string Page = "AdminFaq";
                public const string FaqCreate = "AdminFaqCreate";
                public const string FaqUpdate = "AdminFaqUpdate";
                public const string FaqDelete = "AdminFaqDelete";
                public const string FaqSyncSortIndexes = "AdminFaqSyncSortIndexes";
                #endregion
            }
            
            public class Home
            {
                #region Properties
                public const string Index = "AdminHomeIndex";
                #endregion
            }

            public class Orders
            {
                #region Properties
                public const string Index = "AdminOrders";
                public const string InviceReport = "AdminOrdersInviceReport";
                public const string Grid = "AdminOrdersGrid";
                public const string GridUpdate = "AdminOrdersGridUpdate";
                #endregion

                #region Sub Classes
                public class OrderDetails
                {
                    #region Properties
                    public const string Parent = "AdminOrderDetailsParent";
                    public const string Index = "AdminOrderDetails";
                    public const string Grid = "AdminOrderDetailsGrid";
                    public const string Recipient = "AdminOrderRecipient";
                    #endregion
                }
                #endregion
            }

            public class Products
            {
                #region Properties
                public const string Index = "AdminProducts";
                public const string ProductsGrid = "AdminProductsGrid";
                public const string ProductsGridAdd = "AdminProductsGridAdd";
                public const string ProductsGridUpdate = "AdminProductsGridUpdate";
                public const string ProductsGridDelete = "AdminProductsGridDelete";
                #endregion

                #region Sub Classes
                public class Product
                {
                    #region Properties
                    public const string Properties = "AdminProductsProductProperties";
                    public const string DeleteImage = "AdminProductsProductPropertiesDeleteImage";
                    #endregion
                }
                #endregion
            }

            public class UserManagement
            {
                #region Properties
                public const string UserActivation = "AdminUserManagementUserActivation";
                public const string Users = "AdminUserManagementUsers";
                public const string UsersGrid = "AdminUserManagementUsersGrid";
                public const string UsersGridAdd = "AdminUserManagementUsersGridAdd";
                public const string UsersGridUpdate = "AdminUserManagementUsersGridUpdate";
                public const string UsersGridDelete = "AdminUserManagementUsersGridDelete";

                public const string Roles = "AdminUserManagementRoles";
                public const string RolesGrid = "AdminUserManagementRolesGrid";
                public const string RolesGridAdd = "AdminUserManagementRolesGridAdd";
                public const string RolesGridUpdate = "AdminUserManagementRolesGridUpdate";
                public const string RolesGridDelete = "AdminUserManagementRolesGridDelete";

                public const string Permissions = "AdminUserManagementPermissions";
                public const string PermissionsTree = "AdminUserManagementPermissionsTree";
                public const string PermissionsTreeAdd = "AdminUserManagementPermissionsTreeAdd";
                public const string PermissionsTreeUpdate = "AdminUserManagementPermissionsTreeUpdate";
                public const string PermissionsTreeUpdateParent = "AdminUserManagementPermissionsTreeUpdateParent"; 
                public const string PermissionsTreeDelete = "AdminUserManagementPermissionsTreeDelete";

                public const string RolePermissions = "AdminUserManagementRolePermissions";
                public const string RolePermissionsRolesGrid = "AdminUserManagementRolePermissionsRolesGrid";
                public const string RolePermissionsPermissionsTree = "AdminUserManagementRolePermissionsPermissionsTree";
                public const string RolePermissionsSave = "AdminUserManagementRolePermissionsSave";
                #endregion
            }            
            #endregion
        }

        public class Website
        {
            #region Sub Classes
            public class Account
            {
                #region Properties
                public const string Profile = "WebsiteAccountProfile";
                public const string OrderHistory = "WebsiteAccountOrderHistory";
                public const string ChangePassword = "WebsiteAccountChangePassword";
                public const string InvoiceExport = "WebsiteAccountInvoiceExport";
                #endregion
            }

            public class Auth
            {
                #region Properties
                public const string ForgotPassword = "WebsiteAuthForgotPassword";
                public const string ForgotPasswordResetPassword = "WebsiteAuthForgotPasswordResetPassword";
                public const string SignIn = "WebsiteAuthSignIn";
                public const string SignOut = "WebsiteAuthSignOut";
                public const string SignUp = "WebsiteAuthSignUp";
                #endregion
            }

            public class Basket
            {
                #region Properties
                public const string Index = "WebsiteBasket";
                public const string Delete = "WebsiteBasketDelete";
                public const string Add = "WebsiteBasketAdd";
                public const string Update = "WebsiteBasketUpdate";
                #endregion
            }

            public class Checkout
            {
                #region Properties
                public const string Page = "WebsiteCheckout";
                public const string Success = "WebsiteCheckoutSucces";
                public const string Fail = "WebsiteCheckoutFail";
                public const string PaypalResponse = "WebsiteCheckoutPaypalResponse";                                
                #endregion
            }

            public class Home
            {
                #region Properties
                public const string Index = "Index";
                public const string About = "About";
                public const string FAQ = "FAQ";
                public const string Contact = "Contact";
                #endregion
            }


            public class Products
            {
                #region Properties
                public const string Page = "Products";
                //public const string FullCatalogueByPage = "FullCatalogueByPage";
                public const string ProductsByCategory = "ProductsByCategory";
                //public const string CategoryByPage = "CategoryByPage";
                public const string Product = "Product";
                #endregion
            }

            public class Gallery
            {
                #region Properties
                public const string Page = "Gallery";
                public const string Albums = "GalleryAlbums";
                public const string GalleryItem = "GalleryItem";
                public const string GalleryAlbumPhotos = "GalleryAlbumPhotos";
                #endregion
            }
            #endregion
        }
    }
}