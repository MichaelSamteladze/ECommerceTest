using System;
using System.Collections.Generic;
using System.Linq;
using SixtyThreeBits.Libraries;
using Newtonsoft.Json;
using RestSharp;
using Core.DB;
using System.Threading.Tasks;
using RestSharp.Authenticators;
using Core.Utilities;

namespace Core.Services.Payments
{
    public class PayPalService : SixtyThreeBitsDataObject
    {
        #region Properties        
        static string AccessToken = null;
        static DateTime AccessTokenExpiration = DateTime.Now;

        public string CustomerAuthenticationRedirecLink { set; get; }
        public string ID { set; get; }
        public int? TransactionID { set; get; }
        #endregion Properties

        #region Methods                
                
        #region Single Payment
        /// <summary>
        /// Payment Step 1 - Create single payment for user
        /// </summary>
        /// <param name="Caption">Caption</param>
        /// <param name="Price">Price</param>
        /// <param name="ReturnUrl">Return URL</param>
        /// <param name="CancelUrl">Back from paypal url in case of cancel</param>
        /// <param name="UserID">User ID</param>
        public async Task CreatePayment(string Caption, string Price, string ReturnUrl, string CancelUrl,int? OrderID,List<Order.OrderDetail> Basket,int? UserID)
        {
            if (InitAccessToken())
            {
                var client = new RestClient();
                client.BaseUrl = new Uri($"{AppSettings.PaypalApiUrl}payments/payment");
                var request = new RestRequest();
                request.AddHeader("Authorization", "Bearer " + AccessToken);
                request.AddHeader("content-type", "application/json");
                request.Method = Method.POST;

                var Json = new
                {
                    intent = "sale",
                    redirect_urls = new
                    {
                        return_url = ReturnUrl,
                        cancel_url = CancelUrl
                    },
                    payer = new
                    {
                        payment_method = "paypal"
                    },
                    transactions = new object[]
                    {
                        new
                        {
                            amount = new
                            {
                                total = Price,
                                currency = "USD",
                            },
                            description = $"{Caption} - ${Price}",
                            custom = new{ UserID, OrderID}.ToJSON(),
                            item_list = new
                            {
                                items = Basket.Select(b=>new
                                {
                                    quantity = b.OrderDetailProductCount,
                                    name = b.OrderDetailProductCaption,
                                    price = string.Format("{0:n2}", b.OrderDetailProductPrice),
                                    currency = "USD"
                                }).ToArray()
                            }
                        }
                    }
                };

                var JsonString = JsonConvert.SerializeObject(Json, Formatting.None);
                request.AddParameter("application/json;", JsonString, ParameterType.RequestBody);

                try
                {
                    var response = await client.ExecuteTaskAsync(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        var R = JsonConvert.DeserializeObject<CreatePaymentResponse>(response.Content);
                        CustomerAuthenticationRedirecLink = R.Links.Where(l => l.rel == "approval_url").Select(l => l.Href).FirstOrDefault();                        
                        ID = R.ID;
                    }
                    else
                    {
                        throw new Exception(response.Content);
                    }
                    
                    TSP_PaypalTransactions(
                        iud: (byte)0,
                        TransactionTypeCode: 1,
                        RequestUrl: client.BaseUrl.ToString(),
                        RequestParameters: JsonString,
                        ResponseString: response.Content,
                        OrderID: OrderID
                    );
                }
                catch(Exception ex)
                {
                    ProcessException(string.Format("PaypalService.CreatePayment(Json = {0})", JsonString), ex);                    
                    new Mail().SendErrorNotification("Paypal CreateBillingPlan", ErrorMessage);
                };
            }
        }

        /// <summary>
        /// Payment Step 2 - Execute payment when paypal redirects user back
        /// </summary>
        /// <param name="PaymentID">Paypal payment ID</param>
        /// <param name="PayerID">Paypal PayerIR</param>
        /// <param name="UserID">User ID</param>
        /// <returns></returns>
        public async Task<ExecutePaymentResponse> ExecutePayment(string PaymentID, string PayerID, int? OrderID)
        {
            if (InitAccessToken())
            {
                var client = new RestClient();
                client.BaseUrl = new Uri($"{AppSettings.PaypalApiUrl}payments/payment/{PaymentID}/execute");
                var request = new RestRequest();
                request.AddHeader("Authorization", "Bearer " + AccessToken);
                request.AddHeader("content-type", "application/json");
                request.Method = Method.POST;

                var Json = new
                {
                    payer_id = PayerID
                };

                var JsonString = JsonConvert.SerializeObject(Json, Formatting.None);
                request.AddParameter("application/json;", JsonString, ParameterType.RequestBody);

                try
                {
                    var response = await client.ExecuteTaskAsync(request);

                    TSP_PaypalTransactions(
                        iud: (byte)0,
                        TransactionTypeCode: 2,
                        RequestUrl: client.BaseUrl.ToString(),
                        RequestParameters: JsonString,
                        ResponseString: response.Content,
                        OrderID: OrderID
                    );

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        throw new Exception(response.StatusDescription + Environment.NewLine + response.ErrorMessage + Environment.NewLine + response.Content);
                    }
                    var R = JsonConvert.DeserializeObject<ExecutePaymentResponse>(response.Content);
                    R.LocalTransactionID = TransactionID;

                    return R;
                }
                catch (Exception ex)
                {
                    ProcessException(string.Format("PaypalService.ExecutePayment(Json = {0})", JsonString), ex);
                    new Mail().SendErrorNotification("Paypal AgreeToBillingPlan", ErrorMessage);
                    return null;
                }
            }

            return null;
        }

        public async Task CreditCardPayment(string Caption, string Price, string ReturnUrl, string CancelUrl, int? OrderID)
        {
            if (InitAccessToken())
            {
                var client = new RestClient();
                client.BaseUrl = new Uri($"{AppSettings.PaypalApiUrl}payments/payment");
                var request = new RestRequest();
                request.AddHeader("Authorization", "Bearer " + AccessToken);
                request.AddHeader("content-type", "application/json");
                request.Method = Method.POST;

                var Json = new
                {
                    intent = "sale",
                    redirect_urls = new
                    {
                        return_url = ReturnUrl,
                        cancel_url = CancelUrl
                    },
                    payer = new
                    {
                        payment_method = "credit_card",
                        funding_instruments = new object[] 
                        { 
                            new 
                            {
                                credit_card = new
                                {
                                      number="4417119669820331",
                                      type="visa",
                                      expire_month=11,
                                      expire_year=2018,
                                      cvv2="874",
                                      first_name="Betsy",
                                      last_name="Buyer",
                                      billing_address = new 
                                      {
                                        line1="111 First Street",
                                        city="Saratoga",
                                        state="CA",
                                        postal_code="95070",
                                        country_code="US"
                                      }
                                }
                            }
                        }
                    },
                    transactions = new object[] 
                    { 
                        new
                        {
                            amount = new 
                            {
                                total = Price,
                                currency = "USD",
                            },
                            description = string.Format("{0} - ${1}",Caption,Price)
                        }
                    }
                };

                var JsonString = JsonConvert.SerializeObject(Json, Formatting.None);
                request.AddParameter("application/json;", JsonString, ParameterType.RequestBody);

                try
                {
                    var response = await client.ExecuteTaskAsync(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        var R = JsonConvert.DeserializeObject<CreatePaymentResponse>(response.Content);
                        CustomerAuthenticationRedirecLink = R.Links.Where(l => l.rel == "approval_url").Select(l => l.Href).FirstOrDefault();
                        ID = R.ID;
                    }

                    TSP_PaypalTransactions(
                        iud: (byte)0,
                        TransactionTypeCode: 1,
                        RequestUrl: client.BaseUrl.ToString(),
                        RequestParameters: JsonString,
                        ResponseString: response.Content,
                        OrderID: OrderID
                    );
                }
                catch (Exception ex)
                {
                    ProcessException(string.Format("PaypalService.CreatePayment(Json = {0})", JsonString), ex);
                    new Mail().SendErrorNotification("Paypal CreateBillingPlan", ErrorMessage);
                };
            }
        }
        #endregion Single Payment

        #region Recurring Payment
        /// <summary>
        /// Recurring Billing Step 1, create subscription plan
        /// </summary>
        /// <param name="Caption">Caption</param>
        /// <param name="InitialPrice">Initial price</param>
        /// <param name="RecurringPrice">Recurring price</param>
        /// <param name="SubscriptionFrequency">Charge customer every x DAY or MONTH</param>
        /// <param name="SubscriptionInterval">Every how many months or days should customer be charged</param>
        /// <param name="ReturnUrl">Back from paypal url in case of transaction</param>
        /// <param name="CancelUrl">Back from paypal url in case of cancel</param>
        /// <param name="UserID">User ID</param>
        //public void CreateBillingPlan(string Caption,string InitialPrice,string RecurringPrice,string SubscriptionFrequency, string SubscriptionInterval,string ReturnUrl,string CancelUrl,long? UserID, DateTime RecurringStartDate)
        //{            
        //    if (InitAccessToken())
        //    {                
        //        var client = new RestClient();
        //        client.BaseUrl = new Uri(Url + "payments/billing-plans");
        //        var request = new RestRequest();
        //        request.AddHeader("Authorization", "Bearer " + AccessToken);
        //        request.AddHeader("content-type", "application/json");
        //        request.Method = Method.POST;

        //        var Description = InitialPrice == RecurringPrice ? string.Format("{0} ${1} Subscription", Caption, InitialPrice) : string.Format("{0} ${1} subscription. ${2} for renewal after expiration", Caption, InitialPrice, RecurringPrice);

        //        var Json = new
        //        {
        //            name = Caption,
        //            description = Description,
        //            type = "INFINITE",
        //            payment_definitions = new object[]
        //            {
        //                new
        //                {
        //                    name = Description,
        //                    type ="REGULAR",
        //                    frequency = SubscriptionFrequency,
        //                    frequency_interval = SubscriptionInterval,
        //                    amount = new 
        //                    {
        //                        value= RecurringPrice,
        //                        currency= "USD"
        //                    },
        //                    cycles = "0"
        //                }
        //            },
        //            merchant_preferences = new
        //            {
        //                //setup_fee = new
        //                //{
        //                //    value = InitialPrice,
        //                //    currency = "USD"
        //                //},
        //                return_url = ReturnUrl,
        //                cancel_url = CancelUrl,
        //                max_fail_attempts = "5"
        //            }
        //        };

        //        var JsonString = JsonConvert.SerializeObject(Json, Formatting.None);
        //        request.AddParameter("application/json;", JsonString, ParameterType.RequestBody);

        //        TryExecute(string.Format("Core.Modules.Payment.PayPal.PaypalService.CreateBillingPlan(Json = {0})", JsonString), () =>
        //        {
        //            var response = client.Execute(request);

        //            TSP_PaypalTransactions(
        //                iud: (byte)0,
        //                TransactionTypeCode:3,
        //                RequestUrl: client.BaseUrl,
        //                RequestParameters: JsonString,
        //                ResponseString: response.Content,
        //                UserID: UserID
        //            );

        //            if (response.StatusCode == System.Net.HttpStatusCode.Created)
        //            {
        //                var R = JsonConvert.DeserializeObject<CreateBillingPlanResponse>(response.Content);
        //                ActivateBillingPlan(R.ID, Description, UserID, R.CreateTime.AddMinutes(2));
        //                //ActivateBillingPlan(R.ID, Description, UserID, RecurringStartDate);
        //            }
        //        }, () =>
        //        {
        //            IsError = true;
        //            ErrorMessage = ExceptionObject.Message;
        //            ErrorMessage.LogString();
        //            new Mail().SendErrorNotification("Paypal CreateBillingPlan", ErrorMessage);
        //        });
        //        //System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "file.txt", s);
        //    }
        //}

        /// <summary>
        /// Recurring Billing Step 2, activate subscription plan
        /// </summary>
        /// <param name="PlanID">Plan ID</param>
        /// <param name="Description">Plan description</param>
        /// <param name="UserID">User ID</param>
        //void ActivateBillingPlan(string PlanID, string Description, long? UserID, DateTime RecurringStartDate)
        //{
        //    var client = new RestClient();
        //    client.BaseUrl = new Uri(Url + "payments/billing-plans/" + PlanID);
        //    var request = new RestRequest();
        //    request.AddHeader("Authorization", "Bearer " + AccessToken);
        //    request.AddHeader("content-type", "application/json");
        //    request.Method = Method.PATCH;

        //    var Json = new object[]
        //    {
        //        new
        //        {
        //            path = "/",
        //            value = new
        //            {
        //                state = "ACTIVE"
        //            },
        //            op = "replace"
        //        }
        //    };

        //    var JsonString = JsonConvert.SerializeObject(Json, Formatting.None);
        //    request.AddParameter("application/json;", JsonString, ParameterType.RequestBody);

        //    TryExecute(string.Format("Core.Modules.Payment.PayPal.PaypalService.ActivateBillingPlan(PlanID = {0}, Json = {1})", PlanID, JsonString), () =>
        //    {
        //        var response = client.Execute(request);

        //        TSP_PaypalTransactions(
        //            iud: (byte)0,
        //            TransactionTypeCode:4,
        //            RequestUrl: client.BaseUrl,
        //            RequestParameters: JsonString,
        //            ResponseString: response.Content,
        //            OrderID: OrderID
        //        );

        //        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            AgreeToBillingPlan(PlanID, Description, UserID, RecurringStartDate);
        //        }
        //    }, () =>
        //    {
        //        IsError = true;
        //        ErrorMessage = ExceptionObject.Message;
        //        ErrorMessage.LogString();
        //        new Mail().SendErrorNotification("Paypal ActivateBillingPlan", ErrorMessage);
        //    });
        //}

        /// <summary>
        /// Recurring Billing Step 3, subscribe user to plan, by redirecting him/her to paypal page
        /// </summary>
        /// <param name="PlanID">Paypal Plan ID</param>
        /// <param name="Description">Plan description</param>
        /// <param name="UserID">User ID</param>
        //void AgreeToBillingPlan(string PlanID, string Description, long? UserID, DateTime RecurringStartDate)
        //{
        //    var client = new RestClient();
        //    client.BaseUrl = new Uri(Url + "payments/billing-agreements");
        //    var request = new RestRequest();
        //    request.AddHeader("Authorization", "Bearer " + AccessToken);
        //    request.AddHeader("content-type", "application/json");
        //    request.Method = Method.POST;

        //    var Json = new
        //    {
        //        name = Description,
        //        description = Description,
        //        //start_date = easternTime.ToString("s") + "Z",
        //        //start_date = RecurringStartDate.AddMinutes(10).ToString("s") + "Z",                
        //        start_date = RecurringStartDate.ToString("s") + "Z",
        //        plan = new
        //        {
        //            id = PlanID
        //        },
        //        payer = new
        //        {
        //            payment_method = "paypal"
        //        }
        //    };

        //    var JsonString = JsonConvert.SerializeObject(Json, Formatting.None);
        //    request.AddParameter("application/json;", JsonString, ParameterType.RequestBody);

        //    TryExecute(string.Format("Core.Modules.Payment.PayPal.PaypalService.AgreeToBillingPlan(PlanID = {0}, Json = {1})", PlanID, JsonString), () =>
        //    {
        //        var response = client.Execute(request);

        //        TSP_PaypalTransactions(
        //            iud: (byte)0,
        //            TransactionTypeCode:5,
        //            RequestUrl: client.BaseUrl,
        //            RequestParameters: JsonString,
        //            ResponseString: response.Content,
        //            UserID: UserID
        //        );

        //        var R = JsonConvert.DeserializeObject<AgreeToBillingPlan>(response.Content);
        //        CustomerAuthenticationRedirecLink = R.Links.Where(l => l.Method == "REDIRECT").Select(l => l.Href).FirstOrDefault();
        //    }, () =>
        //    {
        //        IsError = true;
        //        ErrorMessage = ExceptionObject.Message;
        //        ErrorMessage.LogString();
        //        new Mail().SendErrorNotification("Paypal AgreeToBillingPlan", ErrorMessage);
        //    });
        //}

        /// <summary>
        /// Step 4, execute user - subscription, when paypal redirects user back
        /// </summary>
        /// <param name="AgreementID">Agreement ID</param>
        /// <param name="UserID">User ID</param>
        /// <param name="CoursePriceID">Course price ID</param>
        /// <param name="CouponID">Discount coupon ID</param>
        /// <param name="IP">IP Address</param>
        /// <returns></returns>
        //public ExecuteAgreementResponse ExecuteAgreement(string AgreementID, long? UserID,long? CoursePriceID, int? CouponID,string IP)
        //{
        //    if (InitAccessToken())
        //    {
        //        var client = new RestClient();
        //        client.BaseUrl = new Uri(string.Format("{0}payments/billing-agreements/{0}/agreement-execute", Url, AgreementID));
        //        var request = new RestRequest();
        //        request.AddHeader("Authorization", "Bearer " + AccessToken);
        //        request.AddHeader("content-type", "application/json");
        //        request.Method = Method.POST;

        //        return TryToReturn<ExecuteAgreementResponse>(string.Format("Core.Modules.Payment.PayPal.PaypalService.AgreeToBillingPlan(AgreementID = {0})", AgreementID), () =>
        //        {
        //            var response = client.Execute(request);

        //            TSP_PaypalTransactions(
        //                iud: (byte)0,
        //                TransactionTypeCode: 6,
        //                RequestUrl: client.BaseUrl,
        //                ResponseString: response.Content,
        //                UserID: UserID
        //            );

        //            if (response.StatusCode != System.Net.HttpStatusCode.OK)
        //            {
        //                throw new Exception(response.StatusDescription + Environment.NewLine + response.ErrorMessage + Environment.NewLine + response.Content);
        //            }

        //            var R = JsonConvert.DeserializeObject<ExecuteAgreementResponse>(response.Content);

        //            TSP_PaypalSubscriptions(
        //                iud: (byte)0,
        //                PaypalSubscriptionID: R.ID,
        //                UserID: UserID,
        //                CoursePriceID: CoursePriceID,
        //                CouponID: CouponID,
        //                StatusCode: 1,
        //                IP: IP
        //            );
        //            if (IsError)
        //            {
        //                throw new Exception("Subscription database save error");
        //            }

        //            return R;

        //        }, () =>
        //        {
        //            IsError = true;
        //            ErrorMessage = ExceptionObject.Message;
        //            ErrorMessage.LogString();
        //            new Mail().SendErrorNotification("Paypal AgreeToBillingPlan", ErrorMessage);
        //            return null;
        //        });
        //    }

        //    return null;
        //}

        /// <summary>
        /// Cancel paypal recurring payment susbcription
        /// </summary>
        /// <param name="SubscriptionID">Paypal subscription ID</param>
        /// <param name="Note">Cancelation note</param>
        /// <param name="Url">Request URL</param>
        //public void CancelRecurringBilling(string SubscriptionID, string Note, string Url)
        //{
        //    if (InitAccessToken())
        //    {
        //        var client = new RestClient();
        //        client.BaseUrl = new Uri(string.Format("{0}payments/billing-agreements/{1}/cancel",Url, SubscriptionID));
        //        var request = new RestRequest();
        //        request.AddHeader("Authorization", "Bearer " + AccessToken);
        //        request.AddHeader("content-type", "application/json");
        //        request.Method = Method.POST;

        //        var Json = new
        //        {
        //            note = Note,
        //        };
        //        var JsonString = JsonConvert.SerializeObject(Json, Formatting.None);
        //        request.AddParameter("application/json;", JsonString, ParameterType.RequestBody);

        //        TryExecute(string.Format("Core.Modules.Payment.PayPal.PaypalService.CancelRecurringBilling(Url = {0})", client.BaseUrl), () =>
        //        {
        //            var response = client.Execute(request);

        //            if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
        //            {
        //                throw new Exception(response.StatusDescription + Environment.NewLine + response.ErrorMessage + Environment.NewLine + response.Content);
        //            }
        //        }, () =>
        //        {
        //            IsError = true;
        //            ErrorMessage = ExceptionObject.Message;
        //            ErrorMessage.LogString();
        //            new Mail().SendErrorNotification("Paypal Cancel Subscription", ErrorMessage);
        //        });
        //    }
        //}

        public PaymentAgreement GetAgreement(string ID)
        {
            if (InitAccessToken())
            {
                var client = new RestClient();
                client.BaseUrl = new Uri($"{AppSettings.PaypalApiUrl}payments/billing-agreements/{ID}");
                var request = new RestRequest();
                request.AddHeader("Authorization", "Bearer " + AccessToken);
                request.AddHeader("content-type", "application/json");
                request.Method = Method.GET;


                return TryToReturn<PaymentAgreement>(string.Format("Core.Modules.Payment.PayPal.PaypalService.GetAgreement(ID = {0})", ID), () =>
                {
                    var response = client.Execute(request);

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        throw new Exception(response.StatusDescription + Environment.NewLine + response.ErrorMessage + Environment.NewLine + response.Content);
                    }
                    var A = JsonConvert.DeserializeObject<PaymentAgreement>(response.Content);

                    return A;
                }, () =>
                {
                    IsError = true;
                    ErrorMessage = ExceptionObject.Message;
                    ErrorMessage.LogString();
                    return null;
                });
            }

            return null;
        }
        #endregion Recurring Payment
                                
        /// <summary>
        /// Initialize access token
        /// </summary>
        public bool InitAccessToken()
        {
            return TryToReturn("InitAccessToken()", () =>
            {
                if (string.IsNullOrWhiteSpace(AccessToken) || AccessTokenExpiration <= DateTime.Now)
                {
                    var client = new RestClient();
                    client.BaseUrl = new Uri($"{AppSettings.PaypalApiUrl}oauth2/token");
                    client.Authenticator = new HttpBasicAuthenticator(AppSettings.PaypalClientID, AppSettings.PaypalSecret);
                    
                    var request = new RestRequest();
                    request.AddHeader("Accept", "application/json");
                    request.AddHeader("Accept-Language", "en_US");
                    request.AddParameter("grant_type", "client_credentials");
                    request.Method = Method.POST;
                    var response = client.Execute(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var R = JsonConvert.DeserializeObject<PaypalAuthenticationResponse>(response.Content);
                        AccessToken = R.access_token;
                        AccessTokenExpiration = DateTime.Now.AddSeconds(R.expires_in);
                    }
                    else
                    {
                        throw new Exception(response.ErrorMessage);
                    }
                }

                return true;
            }, () =>
            {
                IsError = true;

                var st = new System.Diagnostics.StackTrace(ExceptionObject, true);
                var frame = st.GetFrame(0);
                ProcessException($"InitAccessToken(Url = {AppSettings.PaypalApiUrl}oauth2/token, ClientID = {AppSettings.PaypalClientID}, Secret = {AppSettings.PaypalSecret})", ExceptionObject, frame.GetFileName(), frame.GetFileLineNumber());

                new Mail().SendErrorNotification("Paypal InitAccessToken", ErrorMessage);
                return false;
            });
        }

        /// <summary>
        /// Performs CRUD action on Module_ChapterIntroductions table in database
        /// </summary>
        /// <param name="iud">Action ID</param>        
        /// <param name="ID">Database uniq ID</param>
        /// <param name="PaypalSubscriptionID">Subscription ID from paypal</param>
        /// <param name="UserID">User ID</param>
        /// <param name="CoursePriceID">Course price ID</param>
        /// <param name="CouponID">Discount coupon ID</param>
        /// <param name="StatusID">Subscription status ID</param>
        /// <param name="StatusCode">Subscription status code</param>
        /// <param name="IP">User ID address</param>
        //public void TSP_PaypalSubscriptions(byte? iud = null, int? ID = null, string PaypalSubscriptionID = null, long? UserID = null, long? CoursePriceID = null,int? CouponID = null, int? StatusID = null, int? StatusCode = null,string IP = null)
        //{
        //    TryExecute(string.Format("Core.Modules.Payment.PayPal.TSP_PaypalSubscriptions(iud = {0}, ID = {1}, PaypalSubscriptionID = {2}, UserID = {3}, CoursePriceID = {4}, CouponID = {5}, StatusID = {6}, StatusCode = {7}, IP = {8})", iud, ID, PaypalSubscriptionID, UserID, CoursePriceID,CouponID, StatusID, StatusCode, IP), () =>
        //    {
        //        using (var db = ConnectionFactory.GetDBCoreDataContext())
        //        {
        //            db.tsp_PaypalSubscriptions(iud, ref ID, PaypalSubscriptionID, UserID, CoursePriceID, CouponID, StatusID, StatusCode, IP);
        //        }
        //    });
        //}

        /// <summary>
        /// Performs CRUD action on Module_ChapterIntroductions table in database
        /// </summary>
        /// <param name="iud">Action ID</param>        
        /// <param name="ID">Database uniq ID</param>
        /// <param name="TransactionTypeID">Paypal transaction type ID</param>
        /// <param name="TransactionTypeCode">Paypal transaction type Code</param>
        /// <param name="RequestUrl">Request url</param>
        /// <param name="RequestParameters">Parameters that are sent with request</param>
        /// <param name="ResponseString">Resposne string</param>
        /// <param name="UserID">User ID</param>
        public void TSP_PaypalTransactions(byte? iud = null, int? ID = null, int? TransactionTypeID = null, int? TransactionTypeCode = null, string RequestUrl = null, string RequestParameters = null, string ResponseString = null, int? OrderID = null)
        {
            TryExecute(string.Format("TSP_PaypalTransactions(iud = {0}, ID = {1}, TransactionTypeID = {2}, TransactionTypeCode = {3}, RequestUrl = {4}, RequestParameters = {5}, ResponseString = {6}, OrderID = {7})", iud, ID, TransactionTypeID, TransactionTypeCode, RequestUrl, RequestParameters, ResponseString, OrderID), () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    db.PaypalTransactionsIUD(iud, ref ID, TransactionTypeID, TransactionTypeCode, RequestUrl, RequestParameters, ResponseString, OrderID);
                    this.TransactionID = ID;
                }
            });
        }
        #endregion Methods
    }

    #region Response Objects
    public class AgreeToBillingPlan
    {
        #region Properties
        [JsonProperty("links")]
        public Link[] Links { set; get; }
        #endregion Properties
    }

    public class Amount
    {
        #region Properties
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
        #endregion Properties
    }

    public class CreateBillingPlanResponse
    {
        #region Properties
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("name")]
        public string Name { set; get; }

        [JsonProperty("description")]
        public string Description { set; get; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("create_time")]
        public DateTime CreateTime { set; get; }

        [JsonProperty("update_time")]
        public DateTime UpdateTime { set; get; }

        [JsonProperty("payment_definitions")]
        public PaymentDefinition[] PaymentDefinitions { set; get; }

        [JsonProperty("links")]
        public Link[] Links { set; get; }
        #endregion Properties
    }

    public class CreatePaymentResponse
    {
        #region Properties
        [JsonProperty("id")]
        public string ID { set; get; }

        [JsonProperty("links")]
        public Link[] Links { set; get; }
        #endregion Properties
    }

    public class ExecuteAgreementResponse
    {
        #region Properties
        [JsonProperty("id")]
        public string ID { set; get; }

        [JsonProperty("links")]
        public Link[] Links { set; get; }
        #endregion Properties
    }

    public class ExecutePaymentResponse
    {
        #region Properties
        public int? LocalTransactionID { set; get; }
        [JsonProperty("id")]
        public string ID { set; get; }

        [JsonProperty("state")]
        public string State { set; get; }

        [JsonProperty("create_time")]
        public DateTime CreateTime { set; get; }

        [JsonProperty("update_time")]
        public DateTime UpdateTime { set; get; }

        [JsonProperty("intent")]
        public string Intent { set; get; }

        [JsonProperty("payer")]
        public Payer Payer { set; get; }

        [JsonProperty("transactions")]
        public Transaction[] Transactions { set; get; }

        [JsonProperty("links")]
        public Link[] Links { set; get; }
        #endregion Properties
    }

    public class Link
    {
        #region Properties
        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("rel")]
        public string rel { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }
        #endregion Properties
    }
    
    public class Payer
    {
        #region Properties
        [JsonProperty("payment_method")]
        public string PaymentMethod { set; get; }

        [JsonProperty("payer_info")]
        public PayerInfo PayerInfo { set; get; }
        #endregion Properties
    }

    public class PayerInfo
    {
        #region Properties
        [JsonProperty("email")]
        public string Email { set; get; }

        [JsonProperty("first_name")]
        public string FirstName { set; get; }

        [JsonProperty("last_name")]
        public string LastName { set; get; }

        [JsonProperty("payer_id")]
        public string PayerID { set; get; }

        [JsonProperty("shipping_address")]
        public ShippingAddress ShippingAddress { set; get; }
        #endregion Properties
    }
                    
    public class PaymentDefinition
    {
        #region Properties
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("name")]
        public string Name { set; get; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("frequency")]
        public string Frequency { get; set; }

        [JsonProperty("amount")]
        public Amount Amount { set; get; }

        [JsonProperty("cycles")]
        public string Cycles { set; get; }

        [JsonProperty("frequency_interval")]
        public string FrequencyInterval { set; get; }
        #endregion Properties
    }

    public class PaypalAuthenticationResponse
    {
        #region Properties
        public string scope { set; get; }
        public string access_token { set; get; }
        public string token_type { set; get; }
        public string app_id { set; get; }
        public int expires_in { set; get; }
        #endregion Properties
    }

    public class ShippingAddress
    {
        #region Properties
        [JsonProperty("line1")]
        public string Line1 { set; get; }

        [JsonProperty("line2")]
        public string Line2 { set; get; }

        [JsonProperty("city")]
        public string City { set; get; }

        [JsonProperty("state")]
        public string State { set; get; }

        [JsonProperty("postal_code")]
        public string PostalCode { set; get; }

        [JsonProperty("country_code")]
        public string CountryCode { set; get; }

        [JsonProperty("recipient_name")]
        public string RecipientName { set; get; }
        #endregion Properties
    }

    public class Transaction
    {
        [JsonProperty("amount")]
        public TransactionAmount Amount { set; get; }

        [JsonProperty("description")]
        public string Description { set; get; }

        [JsonProperty("related_resources")]
        public dynamic[] RelatedResources { set; get; }
    }
    
    public class TransactionAmount
    {
        #region Properties
        [JsonProperty("total")]
        public decimal Total { set; get; }

        [JsonProperty("currency")]
        public string Currency { set; get; }

        [JsonProperty("details")]
        public TransactionAmountDetails Details { set; get; }
        #endregion Properties
    }

    public class TransactionAmountDetails
    {
        #region Properties
        [JsonProperty("subtotal")]
        public decimal Subtotal { set; get; }
        #endregion Properties
    }

    public class TransactionSale
    {
        #region Properties
        [JsonProperty("id")]
        public string ID { set; get; }

        [JsonProperty("create_time")]
        public DateTime CreateTime { set; get; }

        [JsonProperty("update_time")]
        public DateTime UpdateTime { set; get; }

        [JsonProperty("amount")]
        public TransactionAmount Amount { set; get; }

        [JsonProperty("payment_mode")]
        public string PaymentMode { set; get; }

        [JsonProperty("state")]
        public string State { set; get; }

        [JsonProperty("reason_code")]
        public string ReasonCode { set; get; }

        [JsonProperty("protection_eligibility")]
        public string ProtectionEligibility { set; get; }

        [JsonProperty("parent_payment")]
        public string ParentPayment { set; get; }

        [JsonProperty("links")]
        public Link[] Links { set; get; }
        #endregion Properties
    }

    public class PaymentAgreement
    {
        [JsonProperty("id")]
        public string ID { set; get; }
        [JsonProperty("state")]
        public string State { set; get; }
    }
    #endregion Response Objects
}
