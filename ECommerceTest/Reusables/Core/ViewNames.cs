namespace Reusables.Core
{
    public class ViewNames
    {
        public class Admin
        {
            #region Sub Classes
            public class Auth
            {
                #region Properties
                public const string Login = "~/Areas/Admin/Views/Auth/Login.cshtml";
                #endregion
            }

            public class Carousel
            {
                #region Properties
                public const string Grid = "~/Areas/Admin/Views/Carousel/CarouselGrid.cshtml";
                public const string Item = "~/Areas/Admin/Views/Carousel/CarouselItem.cshtml";
                #endregion                
            }

            public class Categories
            {
                #region Properties
                public const string Tree = "~/Areas/Admin/Views/Categories/CategoriesTree.cshtml";
                public const string Category = "~/Areas/Admin/Views/Categories/Category.cshtml";
                #endregion
            }

            public class Dictionaries
            {
                #region Properties
                public const string Page = "~/Areas/Admin/Views/Dictionaries/Dictionaries.cshtml";
                #endregion
            }     

            public class Faq
            {
                #region Properties
                public const string Page = "~/Areas/Admin/Views/Faq/Faq.cshtml";
                #endregion
            }

            public class Home
            {
                #region Properties
                public const string Index = "~/Areas/Admin/Views/Home/Index.cshtml";
                #endregion
            }

            public class Orders
            {
                #region Properties
                public const string Page = "~/Areas/Admin/Views/Orders/Orders.cshtml";
                public const string Details = "~/Areas/Admin/Views/Orders/OrderDetails/OrderDetails.cshtml";
                #endregion
            }

            public class Products
            {
                #region Properties
                public const string Grid = "~/Areas/Admin/Views/Products/ProductsGrid.cshtml";
                public const string Product = "~/Areas/Admin/Views/Products/Product.cshtml";
                #endregion
            }

            public class Shared
            {
                #region Properties
                public const string Layout = "~/Areas/Admin/Views/Shared/Layout.cshtml";
                public const string NotFound = "~/Areas/Admin/Views/Shared/NotFound.cshtml";
                public const string DevexpressGrid = "~/Areas/Admin/Views/Shared/DevexpressGrid.cshtml";
                public const string DevexpressGridLinqDataSource = "~/Areas/Admin/Views/Shared/DevexpressGridLinqDataSource.cshtml";
                public const string DevexpressTree = "~/Areas/Admin/Views/Shared/DevexpressTree.cshtml";
                public const string PopupLayout = "~/Areas/Admin/Views/Shared/PopupLayout.cshtml";
                public const string Tabs = "~/Areas/Admin/Views/Shared/Tabs.cshtml";
                #endregion
            }

            public class UserManagement
            {
                #region Properties
                public const string Users = "~/Areas/Admin/Views/UserManagement/Users.cshtml";
                public const string MyAccount = "~/Areas/Admin/Views/UserManagement/MyAccount.cshtml";
                public const string Roles = "~/Areas/Admin/Views/UserManagement/Roles.cshtml";
                public const string Permissions = "~/Areas/Admin/Views/UserManagement/Permissions.cshtml";
                public const string RolePermissions = "~/Areas/Admin/Views/UserManagement/RolePermissions.cshtml";
                #endregion
            } 
            #endregion
        }

        public class Website
        {
            #region Sub Classes
            public class Account
            {
                public const string Layout = "~/Views/Account/AccountLayout.cshtml";
                public const string Confirmation = "~/Views/Account/Confirmation.cshtml";
                public const string ChangePassword = "~/Views/Account/ChangePassword.cshtml";
                public const string Orders = "~/Views/Account/Orders.cshtml";
                public const string Profile = "~/Views/Account/Profile.cshtml";
            }
            public class Auth
            {
                #region Properties
                public const string SignIn = "~/Views/Auth/SignIn.cshtml";
                public const string SignUp = "~/Views/Auth/SignUp.cshtml";
                public const string ForgotPassword = "~/Views/Auth/ForgotPassword.cshtml";
                public const string ForgotPasswordResetPassword = "~/Views/Auth/ForgotPasswordResetPassword.cshtml";
                #endregion
            }

            public class Basket
            {
                #region Properties
                public const string Index = "~/Views/Basket/Index.cshtml";
                #endregion
            }

            public class Checkout
            {
                #region Properties
                public const string Page = "~/Views/Checkout/Checkout.cshtml";
                public const string CheckoutSuccess = "~/Views/Checkout/CheckoutSuccess.cshtml";
                public const string ChechoutFail = "~/Views/Checkout/CheckoutFail.cshtml"; 
                #endregion
            }

            public class Home
            {
                #region Properties
                public const string Index = "~/Views/Home/Index.cshtml";
                public const string About = "~/Views/Home/About.cshtml";                
                public const string FAQ = "~/Views/Home/FAQ.cshtml";
                public const string Gallery = "~/Views/Home/Gallery.cshtml";
                public const string GalleryAlbum = "~/Views/Home/GalleryAlbum.cshtml";
                public const string Contact = "~/Views/Home/Contact.cshtml";
                #endregion
            }

            public class Products
            {
                public const string Page = "~/Views/Products/Products.cshtml";
                public const string ProductsByCategory = "~/Views/Products/ProductsByCategory.cshtml";
                public const string Product = "~/Views/Products/Product.cshtml";
            }

            public class Shared
            {
                #region Properties                
                public const string Layout = "~/Views/Shared/WebsiteLayout.cshtml";
                public const string NotFound = "~/Views/Shared/NotFound.cshtml";
                public const string SuccessErrorPartialView = "~/Views/Shared/SuccessErrorPartialView.cshtml";
                public const string PagerPartial = "~/Views/Shared/PagerPartial.cshtml";
                public const string PopupLayout = "~/Views/Shared/PopupLayout.cshtml";
                #endregion
            } 
            #endregion
        }

        public class Shared
        {
            #region Properties            
            public const string SuccessErrorPartialView = "~/Views/Shared/SuccessErrorPartialView.cshtml";
            #endregion

            #region Sub Classes
            public class FileTree
            {
                #region Properties
                public const string Tree = "~/Views/Shared/FileTree/FileTree.cshtml";
                public const string Folder = "~/Views/Shared/FileTree/FileTreeFolder.cshtml";
                public const string File = "~/Views/Shared/FileTree/FileTreeFile.cshtml";
                #endregion
            }

            public class FileTreeEditor
            {
                #region Properties
                public const string Editor = "~/Views/Shared/FileTreeEditor/FileTreeEditor.cshtml";
                public const string Folder = "~/Views/Shared/FileTreeEditor/FileTreeEditorFolder.cshtml";
                public const string File = "~/Views/Shared/FileTreeEditor/FileTreeEditorFile.cshtml";
                #endregion
            }
            #endregion
        }
    }
}