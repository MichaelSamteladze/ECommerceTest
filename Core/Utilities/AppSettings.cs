using Core.Enums;
using SixtyThreeBits.Libraries;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace Core.Utilities
{
    public class AppSettings : ConfigAssistant
    {
        #region Properties        

        public static string AdnApiUrl { get { return GetConfigValue(); } }
        public static string AdnLoginID { get { return GetConfigValue(); } }
        public static string AdnTransactionKey { get { return GetConfigValue(); } }

        public static int CacheDurationInSecondsDefault => GetConfigValue().ToInt() ?? 60;

        
        public static string NoImagePath { get { return GetConfigValue(); } }


        public static string PaypalApiUrl { get { return GetConfigValue(); } }
        public static string PaypalClientID { get { return GetConfigValue(); } }
        public static string PaypalSecret { get { return GetConfigValue(); } }

        public const string ProjectName = "E-Commerce Test";

        public static string SMTPAddress { get { return GetConfigValue(); } }
        public static int SMTPPort { get { return GetConfigValue().ToInt() ?? 0; } }
        public static string SMTPUsername { get { return GetConfigValue(); } }
        public static string SMTPPassword { get { return GetConfigValue(); } }
        public static bool SMTPEnableSSL { get { return GetConfigValue().ToBoolean() ?? false; } }
        public static string SMTPFromName { get { return GetConfigValue(); } }

        public static string TaxJarApiToken { get { return GetConfigValue(); } }

        public static string UploadFolderPhysicalPath => GetConfigValue();
        public static string UploadFolderHttpPath => GetConfigValue();

        public static string WebsiteAddress { get { return GetConfigValue(); } }
        #endregion
    }

    public class ConfigAssistant
    {
        #region Methods
        protected static string GetConfigValue([CallerMemberName] string Key = "")
        {
            return ConfigurationManager.AppSettings[Key];
        } 
        #endregion
    }
}
