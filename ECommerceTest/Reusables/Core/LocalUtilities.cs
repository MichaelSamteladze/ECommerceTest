using Core.Utilities;
using System.IO;
using System.Web;
using System.Web.Mvc;
using static Core.Utilities.Constants;

namespace Reusables.Core
{
    public partial class LocalUtilities
    {
        #region Methods
        public static string RenderPartialViewToString(Controller Controller, string ViewName, object Model)
        {
            Controller.ViewData.Model = Model;

            using (var sw = new StringWriter())
            {
                // Find the partial view by its name and the current controller context.
                ViewEngineResult ViewResult = ViewEngines.Engines.FindPartialView(Controller.ControllerContext, ViewName);

                // Create a view context.
                var viewContext = new ViewContext(Controller.ControllerContext, ViewResult.View, Controller.ViewData, Controller.TempData, sw);

                // Render the view using the StringWriter object.
                ViewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public static IHtmlString GetPageTitle(HtmlHelper Html, ViewDataDictionary ViewData, bool IsForHtmlHead = false)
        {
            string PageTitle = ViewData[Constants.ViewData.Title] as string;
            if (IsForHtmlHead)
            {
                PageTitle = string.IsNullOrWhiteSpace(PageTitle) ? AppSettings.ProjectName : $"{PageTitle} | {AppSettings.ProjectName}";
            }
            return Html.Raw(PageTitle);
        }

        public static string GetWebsiteDomain(HttpRequestBase Request)
        {
            var Port = Request.Url.Port;
            var PortString = Port < 1000 ? "" : $":{Port}";

            var WebsiteDomain = $"{Request.Url.Scheme}://{Request.Url.Host}{PortString}";
            return WebsiteDomain;
        }

        public static void SetPageTitle<T>(ViewDataDictionary ViewData, string PageTitle, bool UpdateLastBreadcrumbItem = false) where T : LayoutModelBase
        {
            var Model = GetLayoutViewModel<T>(ViewData, Constants.ViewData.LayoutViewModel);
            if (Model != null)
            {
                SetPageTitle(Model, PageTitle);                
                if (UpdateLastBreadcrumbItem && Model.HasBreadcrumbs)
                {
                    Model.Breadcrumbs.Items[Model.Breadcrumbs.ItemsCount - 1].Caption = PageTitle;
                }
            }
        }

        public static void SetPageTitle<T>(T Model, string PageTitle) where T : LayoutModelBase
        {
            Model.PageTitle = PageTitle;
            Model.PageTitleHead = string.IsNullOrWhiteSpace(PageTitle) ? AppSettings.ProjectName : $"{PageTitle} | {AppSettings.ProjectName}";
        }

        public static T GetLayoutViewModel<T>(ViewDataDictionary ViewData,string Key)
        {
            return (T)ViewData[Key];
        }

        public static void SetLayoutViewModel<T>(ViewDataDictionary ViewData, T Model, string Key)
        {
            ViewData[Key] = Model;            
        }
                       
        public static void AddBreadCrumbsItem<T>(ViewDataDictionary ViewData,string Caption, string NavigateUrl = null)  where T: LayoutModelBase
        {
            var Model = GetLayoutViewModel<T>(ViewData, Constants.ViewData.LayoutViewModel);            
            if (Model != null && Model.Breadcrumbs != null)
            {
                Model.Breadcrumbs.AddItem(new Breadcrumbs.BreadCrumbItem
                {
                    Caption = Caption,
                    NavigateUrl = NavigateUrl,
                    IsLastItem = true
                });
            }
            SetLayoutViewModel(ViewData: ViewData, Model: Model, Key: Constants.ViewData.LayoutViewModel);
        }

        public static void RemoveBreadCrumbsItem<T>(ViewDataDictionary ViewData, int? Index = null) where T : LayoutModelBase
        {
            var Model = GetLayoutViewModel<T>(ViewData, Constants.ViewData.LayoutViewModel);
            
            if (Model?.Breadcrumbs?.Items?.Count > 0)
            {
                Index = Index ?? Model.Breadcrumbs.Items.Count - 1;
                Model.Breadcrumbs.DeleteItem(Index.Value);
            }
            SetLayoutViewModel(ViewData: ViewData, Model: Model, Key: Constants.ViewData.LayoutViewModel);
        }

        public static void UpdateBreadCrumbsItem<T>(ViewDataDictionary ViewData, string Caption, bool RemovePrevious = false, int? Index = null, string NavigateUrl = null) where T : LayoutModelBase
        {
            var Model = GetLayoutViewModel<T>(ViewData, Constants.ViewData.LayoutViewModel);
            if (Model?.Breadcrumbs?.Items?.Count > 0)
            {
                Index = Index?? Model.Breadcrumbs.Items.Count - 1;
                
                Model.Breadcrumbs.UpdateItem(new Breadcrumbs.BreadCrumbItem
                {
                    Caption = Caption,
                    NavigateUrl = NavigateUrl
                }, Index.Value);
                if (RemovePrevious)
                {
                    Model.Breadcrumbs.DeleteItem(Index.Value-1);
                }
            }
            SetLayoutViewModel(ViewData: ViewData, Model: Model, Key: Constants.ViewData.LayoutViewModel);
        }

        public static string FormatMoney(object Value)
        {
            return string.Format(Utility.CultureUS, $"${Formats.Decimal2FractionsEval}", Value);
        }
        #endregion
    }    
}