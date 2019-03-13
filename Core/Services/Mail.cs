using Core.Utilities;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Core.Services
{
    public class Mail : SixtyThreeBitsDataObject
    {
        #region Properties

        static string SMTPUsername = AppSettings.SMTPUsername;
        static string SMTPPassword = AppSettings.SMTPPassword;
        static string SMTPAddress = AppSettings.SMTPAddress;        
        static int SMTPPort = AppSettings.SMTPPort;
        static bool SMTPEnableSSL = AppSettings.SMTPEnableSSL;
        static string SMTPFromName = AppSettings.SMTPFromName;
        #endregion Properties

        #region Methods        
        public static bool Send(string To, string Subject, string Body)
        {
            return Send<object>(To: To, Subject: Subject, Body: Body, From: null, FromName: null, ReplyTo: null, Attachments: null, DeleteAttachmentsAfterSend: false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Attachment type, Only String, System.IO.Stream and System.Net.Mail.Attachment types are supported. use any type when attachments are not provided</typeparam>
        /// <returns></returns>
        public static bool Send<T>(string To, string Subject, string Body,string From = null, string FromName = null, string ReplyTo = null, List<T> Attachments = null, bool DeleteAttachmentsAfterSend = true)
        {
            return TryToReturnStatic($"{nameof(Send)}({nameof(To)} = {To}, {nameof(Subject)} = {Subject}, {nameof(Body)} = {Body}, {nameof(From)} = {From}, {nameof(FromName)} = {FromName}, {nameof(ReplyTo)} = {ReplyTo}, {nameof(Attachments)} = {Attachments?.ToJSON()}, {nameof(DeleteAttachmentsAfterSend)} = {DeleteAttachmentsAfterSend})", () =>
            {
                using (var Message = new MailMessage())
                {
                    From = string.IsNullOrWhiteSpace(From) ? SMTPUsername : From;
                    FromName = string.IsNullOrWhiteSpace(FromName) ? SMTPFromName : FromName;

                    Message.From = new MailAddress(From, FromName);
                    Message.To.Add(To);
                    Message.Subject = Subject;
                    Message.Body = Body;
                    Message.IsBodyHtml = true;
                    Message.BodyEncoding = Encoding.UTF8;
                    Message.SubjectEncoding = Encoding.UTF8;

                    if (Attachments != null)
                    {
                        Attachments.ForEach(Item =>
                        {
                            var AttachmentType = typeof(T);
                            if (AttachmentType == typeof(string))
                            {
                                Message.Attachments.Add(new Attachment(Item as string));
                            }
                            else if (AttachmentType == typeof(SimpleKeyValue<string,System.IO.Stream>))
                            {
                                var Attachment = Item as SimpleKeyValue<string, System.IO.Stream>;
                                Message.Attachments.Add(new Attachment(Attachment.Value, Attachment.Key));
                            }
                            else if (AttachmentType == typeof(Attachment))
                            {
                                Message.Attachments.Add(Item as Attachment);
                            }
                        });
                    }

                    if (!string.IsNullOrWhiteSpace(ReplyTo))
                    {
                        Message.ReplyToList.Add(ReplyTo);
                    }

                    using (var Client = new SmtpClient(SMTPAddress, SMTPPort))
                    {
                        Client.EnableSsl = SMTPEnableSSL;
                        Client.Credentials = new NetworkCredential(SMTPUsername, SMTPPassword);
                        Client.Send(Message);
                        Client.SendCompleted += (object sender, System.ComponentModel.AsyncCompletedEventArgs e) =>
                        {
                            if (DeleteAttachmentsAfterSend && Attachments?.Count > 0 && typeof(T) == typeof(string))
                            {
                                Attachments.ForEach(Item =>
                                {
                                    System.IO.File.Delete(Item as string);
                                });
                            }
                        };
                    }                    
                }

                return true;
            });
        }

        public void SendErrorNotification(string Subject, string Body)
        {
            Subject = string.Format("StafansPro Error {0}", Subject);
            //Body = Utility.GetTplContent(AppSettings.EmailTemplateGeneral, new string[] { "[subject]", "[body]" }, new string[] { Subject, Body });
            Send("didebulidze88@gmail.com", Subject, Body);
        }
        #endregion Methods
    }
}
