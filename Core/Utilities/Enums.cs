using System.Collections.Generic;

namespace Core.Enums
{
    public class AreaNames
    {
        #region Properties
        public const string Key = "AreaNamesKey";
        public const string Admin = "Admin";
        public const string Api = "Api"; 
        #endregion
    }

    public class ApiServices
    {
        #region Sub Classes
        public class CadSoftware
        {
            #region Properties
            public const int ApiCode = 1;
            #endregion

            #region Sub Classes
            public class Actions
            {
                #region Properties
                public const int SearchMaterials = 1;
                #endregion
            }
            #endregion
        } 
        #endregion
    }

    public class CompanyTypes
    {
        #region Properties
        public const int PHYSICAL_PERSON = 100; 
        #endregion
    }

    public class CountryCodes
    {
        #region Properties
        public const int US = 1;
        public const int CANADA = 2;
        #endregion
    }

    public class Cultures
    {
        #region Properties        
        public static System.Globalization.CultureInfo CultureKA => new System.Globalization.CultureInfo("ka-ge");
        public static System.Globalization.CultureInfo CultureEN => new System.Globalization.CultureInfo("en-us");
        public static System.Globalization.CultureInfo CultureRU => new System.Globalization.CultureInfo("ru-ru");

        public static Dictionary<string, System.Globalization.CultureInfo> Culture = new Dictionary<string, System.Globalization.CultureInfo>
        {
            {Languages.GEORGIAN,CultureKA },
            {Languages.ENGLISH,CultureEN},
            {Languages.RUSSIAN,CultureRU},
        };
        #endregion
    }

    public class DatabaseActions
    {
        #region Properties
        public const int CREATE = 0;
        public const int UPDATE = 1;
        public const int DELETE = 2;
        public const int ARCHIVE = 3;
        #endregion
    }

    public class FileManagerResources
    {
        #region Properties
        public const string Categories = "categories";
        public const string News = "news";
        public const string Pages = "pages";
        public const string Products = "Products";
        public const string Projects = "projects";
        #endregion
    }

    public class Languages
    {
        #region Properties
        public const string GEORGIAN = "ka";
        public const string ENGLISH = "en";
        public const string RUSSIAN = "ru"; 
        #endregion
    }

    public class OrderStatuses
    {
        #region Properties
        public const int BASKET = 0;
        public const int PAID = 1;
        public const int PARTIALY_FINISHED = 2;
        public const int FINISHED = 3; 
        #endregion
    }

    public class PaymentOptions
    {
        #region Properties
        public const int PAYPAL = 0;
        public const int CARD = 1;
        #endregion
    }

    public class Settings
    {
        #region Properties
        public const int COMPANY_Email = 0;
        public const int COMPANY_NAME = 1;
        public const int CONTACT_PHONE = 2;
        public const int CONTACT_EMAIL = 3;
        public const int FROM_COUNTRY = 4;
        public const int FROM_STATE = 5;
        public const int FROM_ZIP = 6;
        public const int FROM_CITY = 7;
        public const int FROM_ADDRESS = 8;
        #endregion
    }

    public class TimeUnitCodes
    {
        #region Properties
        public const int MILLISECOND = 1;
        public const int SECOND = 2;
        public const int MINUTE = 3;
        public const int HOUR = 4;
        public const int DAY = 5;
        public const int WEEK = 6;
        public const int MONTH = 7;
        public const int YEAR = 8;
        #endregion
    }
}