using Core.DB;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Core.Services.Payments
{
    public class CreditCardPayment : SixtyThreeBitsDataObject
    {
        #region Properties
        string LoginID;
        string TransactionKey;
        string ApiUrl;
        #endregion Properties

        #region Constructors
        public CreditCardPayment(string ApiUrl, string LoginID, string TransactionKey)
        {
            this.ApiUrl = ApiUrl;
            this.LoginID = LoginID;
            this.TransactionKey = TransactionKey;
        }
        #endregion Constructors

        #region Methods
        public async Task<CreditCardPaymentTransactionResult> SubmitPaymentTransaction(PaymentInfo PI)
        {
            try
            {
                var PostValues = new Dictionary<string, string>();

                PostValues.Add("x_login", LoginID);
                PostValues.Add("x_tran_key", TransactionKey);
                PostValues.Add("x_version", "3.0");
                PostValues.Add("x_amount", PI.Amount.ToString());
                PostValues.Add("x_description", PI.Description);
                PostValues.Add("x_card_num", PI.CardNumber);
                PostValues.Add("x_exp_date", PI.ExpDate);
                PostValues.Add("x_card_code", PI.CCV);
                PostValues.Add("x_first_name", PI.FName);
                PostValues.Add("x_last_name ", PI.LName);
                PostValues.Add("x_city", PI.City);
                PostValues.Add("x_address", PI.Address);
                PostValues.Add("x_state", PI.State);
                PostValues.Add("x_zip ", PI.Zip);
                PostValues.Add("x_country", PI.Country);
                PostValues.Add("x_customer_ip", PI.IP);

                PostValues.Add("x_delim_data", "TRUE");
                PostValues.Add("x_delim_char", "|");
                PostValues.Add("x_relay_response", "FALSE");

                var sb = new StringBuilder();
                foreach (KeyValuePair<string, string> item in PostValues)
                {
                    sb.AppendFormat("{0}={1}&", item.Key, HttpUtility.UrlEncode(item.Value));
                }
                var PostString = sb.ToString();

                var request = (HttpWebRequest)WebRequest.Create(ApiUrl);
                request.Method = "POST";
                request.ContentLength = PostString.Length;
                request.ContentType = "application/x-www-form-urlencoded";
                //request.ClientCertificates.Add(GetCert());

                var SW = new StreamWriter(request.GetRequestStream());
                SW.Write(PostString);
                SW.Close();

                var Result = string.Empty;

                var response = await request.GetResponseAsync();
                using (var responseStream = new StreamReader(response.GetResponseStream()))
                {
                    Result = responseStream.ReadToEnd();
                }

                // insert request and response values into the database
                if (PI.CardNumber.Length > 4)
                {
                    PostString = PostString.Replace(PI.CardNumber, "XXXX-XXXX-XXXX-" + PI.CardNumber.Substring(PI.CardNumber.Length - 4, 4));
                }

                var ADNTransactionID = string.Empty;
                var ADNResultArray = Result.Split('|');
                if (ADNResultArray.Length > 6)
                {
                    ADNTransactionID = ADNResultArray[6];
                }


                var Res = new CreditCardPaymentTransactionResult();
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    int? RecordID = null;
                    db.ADNTransactionsIUD(0, ref RecordID, ADNTransactionID, PostString, Result, PI.OrderID);
                    Res.TransactionID = RecordID;
                    Res.ADNResultArray = ADNResultArray;
                }

                return Res;
            }
            catch(Exception ex)
            {                
                ProcessException(string.Format("Core.Modules.AuthorizeNet.SubmitPaymentTransaction(PI = {0})", PI.ToXml()), ex);
            }
            return null;
        }
        #endregion Methods
    }
    
}
