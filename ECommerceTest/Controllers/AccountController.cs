using ECommerceTest.Filters;
using ECommerceTest.Models;
using Reusables.Core;
using System.Web.Mvc;

namespace ECommerceTest.Controllers
{
    [RoutePrefix("account")]
    [UserAuthorization(Order = 1)]
    [InitAccountTabs(Order = 2)]
    public class AccountController : WebsiteController
    {
        #region Profile
        [Route("profile", Name = ControllerActionRouteNames.Website.Account.Profile)]
        public new ActionResult Profile()
        {
            var Model = AccountModel.Profile.GetPageViewModel(Session);
            LocalUtilities.SetPageTitle<WebsiteLayoutModel>(ViewData, Model.PageTitle);
            return View(ViewNames.Website.Account.Profile, Model);
        }

        [HttpPost]
        [Route("profile")]
        public new ActionResult Profile(AccountModel.Profile.PageViewModel Model)
        {
            var Result = default(ActionResult);
            AccountModel.Profile.GetPageViewModel(Session, Model);
            LocalUtilities.SetPageTitle<WebsiteLayoutModel>(ViewData, Model.PageTitle);

            AccountModel.Profile.ValidateAccountForm(Model, Session);
            if (Model.Form.HasErrors)
            {
                Result = View(ViewNames.Website.Account.Profile, Model);
            }
            else
            {
                AccountModel.Profile.Update(Model, Session);
                if (Model.Form.IsSaved)
                {
                    SuccessErrorPartialViewAssistance.InitSuccessMessage(Session: Session);
                    Result = Redirect(Request.RawUrl);
                }
                else
                {
                    SuccessErrorPartialViewAssistance.InitErrorMessage<WebsiteLayoutModel>(ViewData: ViewData);
                    Result = View(ViewNames.Website.Account.Profile, Model);
                }
            }

            return Result;
        }
        #endregion

        #region Orders
        [Route("orders", Name = ControllerActionRouteNames.Website.Account.OrderHistory)]
        public ActionResult OrderHistory()
        {
            var Model = AccountModel.OrderHistoryModel.GetPageViewModel(Url, this);
            LocalUtilities.SetPageTitle<WebsiteLayoutModel>(ViewData, Model.PageTitle);
            return View(ViewNames.Website.Account.Orders, Model);
        }

        [Route("orders/invoice", Name = ControllerActionRouteNames.Website.Account.InvoiceExport)]
        public ActionResult ProductInvoiceExport(int? OrderID)
        {
            var Result = default(ActionResult);

            var IsUsersOrder = AccountModel.OrderHistoryModel.ValidateOrderIDByUserID(OrderID, User);
            if (IsUsersOrder)
            {
                var Invoice = AccountModel.OrderHistoryModel.GetInvoiceReport(OrderID);
                Result = File(Invoice, "application/pdf");
            }
            else
            {
                Result = NotFound();
            }
            return Result;
        }
        #endregion

        #region Change Password
        [Route("change-password", Name = ControllerActionRouteNames.Website.Account.ChangePassword)]
        public ActionResult ProfileChangePassword()
        {
            var Model = AccountModel.PasswordChangeModel.GetPageViewModel();
            LocalUtilities.SetPageTitle<WebsiteLayoutModel>(ViewData, Model.PageTitle);
            return View(ViewNames.Website.Account.ChangePassword, Model);
        }

        [HttpPost]
        [Route("change-password")]
        public ActionResult ProfileChangePassword(AccountModel.PasswordChangeModel.PageViewModel SubmitModel)
        {
            var Result = default(ActionResult);

            var Model = AccountModel.PasswordChangeModel.GetPageViewModel(SubmitModel);
            LocalUtilities.SetPageTitle<WebsiteLayoutModel>(ViewData, Model.PageTitle);

            AccountModel.PasswordChangeModel.ValidateUpdatePasswordForm(SubmitModel, User.Password);
            if (Model.Form.HasErrors)
            {
                Result = View(ViewNames.Website.Account.ChangePassword, Model);
            }
            else
            {
                Model = AccountModel.PasswordChangeModel.UpdatePassword(Model, Session);
                if (Model.Form.IsSaved)
                {
                    SuccessErrorPartialViewAssistance.InitSuccessMessage(Session: Session);
                    Result = Redirect(Request.RawUrl);
                }
                else
                {
                    SuccessErrorPartialViewAssistance.InitErrorMessage<WebsiteLayoutModel>(ViewData: ViewData);
                    Result = View(ViewNames.Website.Account.ChangePassword, Model);
                }
            }
                
            return Result;
        }     
        #endregion
    }
}