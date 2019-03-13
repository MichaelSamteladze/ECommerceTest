using Core.Properties;
using System;
using System.Text.RegularExpressions;

namespace Core.Services.Payments
{
    public class PaymentInfo
    {
        #region Properties
        public string RefID { set; get; }
        public string Caption { set; get; }
        public string Description { set; get; }

        #region Card
        /// <summary>
        /// numbers only, no dash or whitespaces
        /// </summary>
        public string CardNumber { set; get; }
        public string CardType { set; get; }
        public string CCV { set; get; }
        /// <summary>
        /// <para>ARB: yyyy-mm. ex: 2018-02</para>
        /// <para>AIM: mm-yy.   ex: 02-18</para>
        /// </summary>
        public string ExpDate { set; get; }
        #endregion Card

        #region User
        public string FName { set; get; }
        public string LName { set; get; }
        public string Email { set; get; }
        public string Phone { set; get; }
        public string Address { set; get; }
        public string City { set; get; }
        public string Zip { set; get; }
        /// <summary>
        /// Only two symbols DE, CA, NY ...
        /// </summary>
        public string State { set; get; }
        public string Country { set; get; }
        public int? CustomerID { set; get; }
        public int? OrderID { set; get; }

        public string IP { set; get; }
        #endregion User

        #region ARB
        public short IntervalLength { set; get; }
        //public ARBSubscriptionUnitEnum IntervalUnit { set; get; }
        public DateTime ARBStartDate { set; get; }
        public short ARBTotalOccurrences { set; get; }

        short _ARBTrialOccurrences = 0;
        public short ARBTrialOccurrences
        {
            set { _ARBTrialOccurrences = value; }
            get { return _ARBTrialOccurrences; }
        }
        #endregion ARB

        #region Money
        /// <summary>
        /// Money amount in dollars. ex: 10.55
        /// </summary>
        public decimal Amount { set; get; }

        decimal _TrialAmount = 0;
        /// <summary>
        /// Money amount in dollars. ex: 10.55
        /// </summary>
        public decimal TrialAmount
        {
            set { _TrialAmount = value; }
            get { return _TrialAmount; }
        }
        #endregion Money

        #endregion Properties
    }

    public class CreditCardPaymentTransactionResult
    {
        public string[] ADNResultArray { set; get; }
        public long? TransactionID { set; get; }
    }

    public class CreditCardHepler
    {
        public static string GetCreditCardTypeByCardNumber(string CreditCardNumber)
        {
            if (Regex.IsMatch(CreditCardNumber, Resources.RegexCreditCardVisa))
            {
                return "Visa";
            }
            else if (Regex.IsMatch(CreditCardNumber, Resources.RegexCreditCardMasterCard))
            {
                return "MasterCard";
            }
            else if (Regex.IsMatch(CreditCardNumber, Resources.RegexCreditCardAmex))
            {
                return "American Express";
            }
            else if (Regex.IsMatch(CreditCardNumber, Resources.RegexCreditCardDinersClub))
            {
                return "Diners Club";
            }
            else if (Regex.IsMatch(CreditCardNumber, Resources.RegexCreditCardDiscover))
            {
                return "Discover";
            }
            else if (Regex.IsMatch(CreditCardNumber, Resources.RegexCreditCardJCB))
            {
                return "JCB";
            }
            else
            {
                return null;
            }
        }
    }
}
