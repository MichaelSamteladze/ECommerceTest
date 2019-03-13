using Core;
using Core.Services.Payments;
using ECommerceTest.Filters;
using ECommerceTest.Models;
using Reusables.Core;
using SixtyThreeBits.Libraries;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ECommerceTest.Controllers
{
    [RoutePrefix("checkout")]
    [UserAuthorization]
    public class CheckoutController : WebsiteController
    {
        [Route("", Name = ControllerActionRouteNames.Website.Checkout.Page)]
        public ActionResult Checkout()
        {
            var Model = CheckoutModel.GetPageViewModel(this);
            LocalUtilities.SetPageTitle<WebsiteLayoutModel>(ViewData, Model.PageTitle);
            return View(ViewNames.Website.Checkout.Page, Model);
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult> Checkout(CheckoutModel.PageViewModel SubmitModel)
        {
            var AR = new AjaxResponse();
            CheckoutModel.ValidatePageViewModel(SubmitModel);
            if (SubmitModel.Form.HasErrors)
            {
                AR.Data = SubmitModel.Form.ErrorsJson;
            }
            else
            {
                var Order = CheckoutModel.UpdateOrder(SubmitModel, User);
                if (Order != null && Order.OrderTotalPrice > 0)
                {
                    if (SubmitModel.IsPaymentByCard)
                    {
                        SubmitModel.Form.Errors = await CheckoutModel.ProcessCreditCard(SubmitModel, Order, User);
                        if (SubmitModel.Form.HasErrors)
                        {
                            AR.Data = SubmitModel.Form.ErrorsJson;
                        }
                        else
                        {                            
                            CheckoutModel.CompletePayment(Order.OrderID, this);
                            AR.IsSuccess = true;
                            AR.Data = Url.RouteUrl(ControllerActionRouteNames.Website.Checkout.Success);
                        }
                    }
                    else
                    {
                        var RedirectUrl = await CheckoutModel.ProcessPaypalPayment(Order, Url);
                        if (!string.IsNullOrWhiteSpace(RedirectUrl))
                        {
                            AR.IsSuccess = true;
                            AR.Data = RedirectUrl;
                        }
                    }
                }
            }
            
            return Json(AR);
        }
        
        [Route("fail", Name = ControllerActionRouteNames.Website.Checkout.Fail)]
        public ActionResult CheckoutFail()
        {
            return View(ViewNames.Website.Checkout.ChechoutFail);
        }

        [Route("success", Name = ControllerActionRouteNames.Website.Checkout.Success)]
        public ActionResult CheckoutSuccess()
        {            
            return View(ViewNames.Website.Checkout.CheckoutSuccess);
        }

        [Route("paypal-response/", Name = ControllerActionRouteNames.Website.Checkout.PaypalResponse)]
        public async Task<ActionResult> CheckoutPaypalResponse()
        {

            var Model = CheckoutModel.GetPageViewModel(this);
            var Token = Request.QueryString["token"];
            var PayerID = Request.QueryString["PayerID"];
            Model.TransactionID = Request.QueryString["paymentId"];
            //Model.Basket = BasketRepository.GetBasketFromSession();

            if (!string.IsNullOrWhiteSpace(Token) && !string.IsNullOrWhiteSpace(PayerID) && !string.IsNullOrWhiteSpace(Model.TransactionID) && Model.OrderID > 0)
            {
                var P = new PayPalService();
                var Result = await P.ExecutePayment(Model.TransactionID, PayerID, Model.OrderID);
                if (Result.State == "approved")
                {
                    CheckoutModel.CompletePayment(Model.OrderID, this, Model.TransactionID);
                    return Redirect(Url.RouteUrl(ControllerActionRouteNames.Website.Checkout.Success));
                }
                else
                {
                    Model.IsPaymentFailed = true;
                }
            }
            
            return RedirectToAction(Url.RouteUrl(ControllerActionRouteNames.Website.Checkout.Page), Model);
        }
    }
}