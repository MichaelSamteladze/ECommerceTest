using Core.DB;
using Core.Properties;
using Core.Services;
using Core.Utilities;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Core
{
    public class NotificationManager : SixtyThreeBitsDataObject
    {
        #region Methods
        public static bool SendContactFormEmail(string FirstName, string LastName, string Email, string Phone, int? CountryID, int? StateID, string City, bool? IsProfessional, int? TopicID, string Message)
        {
            var To = DictionariesDataAccess.GetDictionarySettingsStringCodeByIntCode(Enums.Settings.COMPANY_Email);
            var Subject = Resources.TextContactFormEmail;
            var Body = GetContactFormEmailBodyText(FirstName, LastName, Email, Phone, CountryID, StateID, City, IsProfessional, TopicID, Message);

            return Mail.Send(To: To, Subject: Subject, Body: Body);
        }

        public static bool SendPasswordRecoveryEmail(string RecoveryCode, string Firstname, string Email)
        {
            var Subject = Resources.TextPasswordRecovery;
            var Body = GetPasswordRecoveryEmailBodyText(RecoveryCode, Firstname);

            return Mail.Send(To: Email, Subject: Subject, Body: Body);
        }

        public static bool SendAccountActivationEmail(string Firstname, string Email, string ApplicationAddress)
        {
            var Subject = Resources.TextAccountActivation;
            var Body = GetAccountActivationEmailBodyText(Firstname, ApplicationAddress);

            return Mail.Send(To: Email, Subject: Subject, Body: Body);
        }

        public static bool SendAccountDeactivationEmail(string Firstname, string Email)
        {
            var Subject = Resources.TextAccountDeactivation;
            var Body = GetAccountDeactivationEmailBodyText(Firstname);

            return Mail.Send(To: Email, Subject: Subject, Body: Body);
        }

        public static bool SendConfirmationCode(string Firstname, string Email, string ConfirmationCode)
        {
            var Subject = Resources.TextConfirmationCode;
            var Body = GetConfirmationCodeEmailBodyText(Firstname, ConfirmationCode);

            return Mail.Send(To: Email, Subject: Subject, Body: Body);
        }
        
        public static bool SendPaymentSuccessEmailToCustomer(int? OrderID, string InvoicePath, string Email)
        {
            var Subject = Resources.TextPaymentSuccess;
            var Body = GetPaymentSucessEmailBodyText(OrderID);

            List<string> Attachments = new List<string> { InvoicePath };

            return Mail.Send(To: Email, Subject: Subject, Body: Body, Attachments: Attachments);
        }        

        public static bool SendSaleNotificationToAdmin(int? OrderID)
        {
            var To = DictionariesDataAccess.GetDictionarySettingsStringCodeByIntCode(Enums.Settings.COMPANY_Email);
            var Subject = Resources.TextSaleNotification;
            var Body = GetSaleNotificationEmailBodyText(OrderID);

            return Mail.Send(To: To, Subject: Subject, Body: Body);
        }
        #endregion

        #region Database
        static string GetAccountActivationEmailBodyText( string Firstname, string ApplicationAddress)
        {
            return TryToReturnStatic($"{nameof(GetAccountActivationEmailBodyText)}({nameof(Firstname)} = {Firstname}, {nameof(ApplicationAddress)} = {ApplicationAddress})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.NotificationManager_GetAccountActivationEmailBodyText(Firstname, ApplicationAddress);
                }
            });
        }

        static string GetAccountDeactivationEmailBodyText(string Firstname)
        {
            return TryToReturnStatic($"{nameof(GetAccountDeactivationEmailBodyText)}({nameof(Firstname)} = {Firstname})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.NotificationManager_GetAccountDeactivationEmailBodyText(Firstname);
                }
            });
        }

        static string GetConfirmationCodeEmailBodyText(string Firstname, string ConfirmationCode)
        {
            return TryToReturnStatic($"{nameof(GetAccountDeactivationEmailBodyText)}({nameof(Firstname)} = {Firstname},{nameof(ConfirmationCode)} = {ConfirmationCode})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.NotificationManager_GetConfirmationCodeEmailBodyText(Firstname, ConfirmationCode);
                }
            });
        }

        static string GetContactFormEmailBodyText(string FirstName, string LastName, string Email, string Phone, int? CountryID, int? StateID, string City, bool? IsProfessional, int? TopicID, string Message)
        {
            return TryToReturnStatic($"{nameof(GetAccountDeactivationEmailBodyText)}({nameof(FirstName)} = {FirstName}, {nameof(LastName)} = {LastName}, {nameof(Email)} = {Email}, {nameof(Phone)} = {Phone}, {nameof(CountryID)} = {CountryID}, {nameof(StateID)} = {StateID}, {nameof(City)} = {City}, {nameof(IsProfessional)} = {IsProfessional}, {nameof(TopicID)} = {TopicID}, {nameof(Message)} = {Message})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.NotificationManager_GetContactFormEmailBodyText(FirstName, LastName, Email, Phone, CountryID, StateID, City, IsProfessional, TopicID, Message);
                }
            });
        }

        static string GetPasswordRecoveryEmailBodyText(string RecoveryCode, string Firstname)
        {
            return TryToReturnStatic($"{nameof(GetPasswordRecoveryEmailBodyText)}({nameof(RecoveryCode)} = {RecoveryCode}, {nameof(Firstname)} = {Firstname})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.NotificationManager_GetPasswordRecoveryEmailBodyText(RecoveryCode, Firstname);
                }
            });
        }

        static string GetPaymentSucessEmailBodyText(int? OrderID)
        {
            return TryToReturnStatic($"{nameof(GetPaymentSucessEmailBodyText)}({nameof(OrderID)} = {OrderID})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.NotificationManager_GetPaymentSuccessEmailBodyText(OrderID);
                }
            });
        }



        static string GetSaleNotificationEmailBodyText(int? OrderID)
        {
            return TryToReturnStatic($"{nameof(GetSaleNotificationEmailBodyText)}({nameof(OrderID)} = {OrderID})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.NotificationManager_GetSaleNotificationEmailBodyText(OrderID);
                }
            });
        }
        #endregion

        #region Sub Classes
        public class EmailVariables
        {
            #region Properties            
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public DateTime? Date { get; set; }
            public DateTime? DateTime { get; set; }
            #endregion

            #region Serialization
            public bool ShouldSerializeFirstname() { return !string.IsNullOrWhiteSpace(Firstname); }
            public bool ShouldSerializeLastname() { return !string.IsNullOrWhiteSpace(Lastname); }
            public bool ShouldSerializeDate() { return Date.HasValue; }
            public bool ShouldSerializeDateTime() { return DateTime.HasValue; }
            #endregion
        }
        #endregion
    }
}
