using Core;
using DevExpress.Web.Mvc;
using Reusables.Core;
using ECommerceTest.Areas.Admin.Models;
using System.Web.Mvc;

namespace ECommerceTest.Areas.Admin.Controllers
{
    [RoutePrefix("orders")]
    public class OrdersController : AdminController
    {
        #region Orders Grid
        [Route("", Name = ControllerActionRouteNames.Admin.Orders.Index)]
        public ActionResult Orders()
        {
            var Model = OrdersGridModel.GetOrdersPageViewModel(Url, User);
            return View(ViewNames.Admin.Orders.Page, Model);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.Orders.Grid)]
        public ActionResult OrdersGrid(string ErrorMessage = null)
        {
            var Model = OrdersGridModel.GetOrdersGridModel(Url, User, ErrorMessage);
            return PartialView(ViewNames.Admin.Shared.DevexpressGrid, Model);
        }

        [Route("grid/update", Name = ControllerActionRouteNames.Admin.Orders.GridUpdate)]
        public ActionResult OrdersGridUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] OrdersGridModel.PageViewModel.GridModel.GridItem SubmitModel)
        {
            var ErrorMessage = OrdersGridModel.OrderUpdate(SubmitModel, Url);
            return OrdersGrid(ErrorMessage);
        }

        [Route("invoice", Name = ControllerActionRouteNames.Admin.Orders.InviceReport)]
        public ActionResult ProductInvoiceExport(int? OrderID)
        {
            var Invoice = OrdersGridModel.GetInvoiceReport(OrderID);
            return File(Invoice, "application/pdf");
        }
        #endregion

        #region Order Detailss Grid
        [Route("{OrderID}/details", Name = ControllerActionRouteNames.Admin.Orders.OrderDetails.Index)]
        public ActionResult OrderDetails(int? OrderID)
        {
            var Model = OrderDetailsModel.GetOrderDetailsPageViewModel(Url, User, ViewData, OrderID);
            return View(ViewNames.Admin.Orders.Details, Model);
        }

        [Route("{OrderID}/details/grid", Name = ControllerActionRouteNames.Admin.Orders.OrderDetails.Grid)]
        public ActionResult OrderDetailsGrid(Order Order)
        {
            var Model = OrderDetailsModel.GetOrderDetailsGridModel(Url, User, Order);
            return PartialView(ViewNames.Admin.Shared.DevexpressGrid, Model);
        }
        #endregion
    }
}