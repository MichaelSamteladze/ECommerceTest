using Core;
using Core.Properties;
using Reusables.Core;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ECommerceTest.Areas.Admin.Models
{
    public class FaqModel
    {
        #region Methods
        public static PageViewModel GetFaqPageViewModel(UrlHelper Url)
        {
            return new PageViewModel
            {
                UrlAdd = Url.RouteUrl(ControllerActionRouteNames.Admin.Faq.FaqCreate),
                UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Faq.FaqUpdate),
                UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Faq.FaqDelete),
                UrlSync = Url.RouteUrl(ControllerActionRouteNames.Admin.Faq.FaqSyncSortIndexes),

                FaqViewItems = FaqDataAccess.ListFaq()?.Select(Item => new FaqItem
                {
                    FaqID = Item.FaqID,
                    Question = Item.Question,
                    Answer = Item.Answer,
                    IsPublished = Item.IsPublished
                }).ToList()
            };
        }

        public static AjaxResponse FaqIUD(byte? DatabaseAction, FaqItem Model)
        {
            var DAL = new FaqDataAccess();
            DAL.FaqIUD
                (
                    DatabaseAction: DatabaseAction,
                    FaqID: Model.FaqID,
                    Question: Model.Question,
                    Answer: Model.Answer,
                    IsPublished: Model.IsPublished
                );
            return new AjaxResponse
            {
                IsSuccess = !DAL.IsError
            };
        }

        public static AjaxResponse FaqSyncSortIndexes(SyncSortIndexesModel SubmitModel)
        {
            var DAL = new FaqDataAccess();            
            DAL.SyncSortIndexes(SubmitModel.SortIndexes);
            return new AjaxResponse
            {
                IsSuccess = !DAL.IsError
            };
        }
        #endregion

        #region Sub Classes
        public class PageViewModel : FormViewModelBase
        {
            #region Properties
            public string UrlAdd { get; set; }
            public string UrlUpdate { get; set; }
            public string UrlDelete { get; set; }
            public string UrlSync { get; set; }            
            public bool HasFaqViewItems => FaqViewItems?.Count > 0;
            public List<FaqItem> FaqViewItems { get; set; }

            public string TextConfirmDelete { get; set; } = Resources.TextConfirmDelete;
            #endregion
        }
        #endregion
    }
}