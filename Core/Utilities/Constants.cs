namespace Core.Utilities
{
    public class Constants
    {
        #region Sub Classes
        public class Cache
        {
            
        }

        public class Decimal
        {
            #region Properties
            public const int NoTrailingZeros = -1; 
            #endregion
        }

        public class Formats
        {
            #region Properties

            #region Date
            public const string Date = "MMM dd, yyyy";
            public const string DateEval = "{0:MMM dd, yyyy}";
            public const string DateTime = "MMM dd, yyyy  HH:mm";
            public const string DateTimeEval = "{0:MMM dd, yyyy  HH:mm}";

            public const string DateGeo = "dd/MM/yyyy";
            public const string DateGeoEval = "{0:dd/MM/yyyy}";
            public const string DateTimeGeo = "dd/MM/yyyy HH:mm";
            public const string DateTimeGeoEval = "{0:dd/MM/yyyy HH:mm}";            
            #endregion

            #region Decimal
            public const string Decimal2Fractions = "n2";
            public const string Decimal2FractionsEval = "{0:n2}";
            public const string Decimal2FractionsNoThousandSeparator = "0.00";
            public const string Decimal2FractionsNoThousandSeparatorEval = "{0:0.00}";
            public const string Decimal4Fractions = "n4";
            public const string Decimal4FractionsEval = "{0:n4}";
            public const string DecimalNoFraction = "n0";
            public const string DecimalNoFractionEval = "{0:n0}";
            public const string DecimalNoTrailingZeros = "#,0.############";
            public const string DecimalNoTrailingZerosEval = "{0:#,0.############}";
            #endregion

            #region Time
            public const string Time = "HH:mm";
            public const string TimeEval = "{0:HH:mm}"; 
            #endregion

            #endregion
        }

        public class NullValueFor
        {
            #region Properties
            public const string String = "";
            public const int Int = -1;
            #endregion
        }

        public class RegularExpressions
        {
            #region Properties
            public const string Base64 = "^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$";
            public const string Email = @"^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$"; 
            #endregion
        }

        public class Session
        {
            #region Sub Classes
            public class Admin
            {
                public const string SideBarIsCollapsed = "EcommerceTestAdminSideBarIsCollapsed";
            }

            public class SuccessErrorMessage
            {
                #region Properties
                public const string Error = "EcommerceTestSessionSuccessErrorMessageError";
                public const string Success = "EcommerceTestSessionSuccessErrorMessageSuccess";
                #endregion
            }

            public class User
            {
                #region Properties
                public const string UserItem = "EcommerceTestSessionUserUserItem";
                public const string PasswordRecovery = "EcommerceTestSessionUserPasswordRecovery";
                #endregion
            }
            #endregion
        }

        public class ViewData
        {
            #region Properties
            public const string LayoutViewModel = "ConstantsViewDataLayoutViewModel";
            public const string TabsViewModel = "ConstantsViewDataTabsViewModel";
            public const string Title = "ConstantsViewDataTitle";
            #endregion
        }
        #endregion
    }
}
