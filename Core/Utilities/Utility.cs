using Core.Enums;
using Core.Properties;
using SixtyThreeBits.Libraries;
using System;
using System.IO;
using System.Web;

namespace Core.Utilities
{
    public class Utility
    {
        #region Properties
        public static System.Globalization.CultureInfo CultureInvariant = System.Globalization.CultureInfo.InvariantCulture;
        public static System.Globalization.CultureInfo CultureUS => new System.Globalization.CultureInfo("en-us");
        #endregion

        #region Methods
        public static string GetDatabaseErrorMessage(SixtyThreeBitsDataObject DALItem)
        {
            string ErrorMessage = null;
            if (DALItem != null)
            {
                if (DALItem.IsError)
                {
                    if (DALItem.IsClient)
                    {
                        ErrorMessage = DALItem.ErrorMessage;
                    }
                    else
                    {
                        ErrorMessage = Resources.TextError;
                    }
                }
            }

            return ErrorMessage;
        }

        public static string GetFilenameFromUploadedFile(HttpPostedFileBase PostedFile)
        {
            return PostedFile?.FileName.ToAZ09Dash(GuidInlcuded: true);
        }

        public static string GetUploadedFileHttpPath(string Filename)
        {
            return string.IsNullOrWhiteSpace(Filename) ? null : $"{AppSettings.UploadFolderHttpPath}{Filename}";
        }

        public static string GetUploadedFilePhysicalPath(string Filename)
        {
            return $"{AppSettings.UploadFolderPhysicalPath}{Filename}";
        }

        public static void SaveUploadedFile(HttpPostedFileBase PostedFile, string Filename)
        {
            PostedFile?.SaveAs($"{AppSettings.UploadFolderPhysicalPath}{Filename}");
        }

        public static void DeleteUploadedFile(string Filename)
        {
            if (File.Exists($"{AppSettings.UploadFolderPhysicalPath}{Filename}"))
            {
                File.Delete($"{AppSettings.UploadFolderPhysicalPath}{Filename}");
            }
        }
        
        public static void DeleteFolder(string FolderPath)
        {
            if (Directory.Exists(FolderPath))
            {
                try
                {
                    Directory.Delete(FolderPath, true);
                }
                catch (IOException)
                {
                    DeleteFolder(FolderPath);
                }
                catch (UnauthorizedAccessException)
                {
                    DeleteFolder(FolderPath);
                }
            }
        }
        #endregion
    }
    
}