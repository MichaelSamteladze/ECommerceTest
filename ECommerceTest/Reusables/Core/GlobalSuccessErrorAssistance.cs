using Core.Properties;
using Core.Utilities;
using System;
using System.Web;
using System.Web.Mvc;

namespace Reusables.Core
{
    public class SuccessErrorPartialViewAssistance
    {
        #region Methods
        public static T SetSuccessErrorMessageInLayoutModel<T>(HttpSessionStateBase Session,T Model) where T : LayoutModelBase
        {
            if (Session[Constants.Session.SuccessErrorMessage.Error] != null)
            {
                Model.SuccessErrorModel = new SuccessErrorPartialViewModel
                {
                    ShowError = true,
                    Message = Session[Constants.Session.SuccessErrorMessage.Error].ToString()
                };

                Session.Remove(Constants.Session.SuccessErrorMessage.Error);
            }
            else if (Session[Constants.Session.SuccessErrorMessage.Success] != null)
            {
                Model.SuccessErrorModel = new SuccessErrorPartialViewModel
                {
                    ShowSuccess = true,
                    Message = Session[Constants.Session.SuccessErrorMessage.Success].ToString()
                };
                Session.Remove(Constants.Session.SuccessErrorMessage.Success);
            }

            return Model;
        }

        public static void InitErrorMessage<T>(string Message = null, ViewDataDictionary ViewData = null, HttpSessionStateBase Session = null) where T : LayoutModelBase
        {
            if (Message == null)
            {
                Message = Resources.TextError;
            }

            if (ViewData == null && Session != null)
            {
                Session[Constants.Session.SuccessErrorMessage.Error] = Message;
            }
            else if (ViewData != null)
            {
                var Model = LocalUtilities.GetLayoutViewModel<T>(ViewData:ViewData,Key: Constants.ViewData.LayoutViewModel);
                
                if (Model != null)
                {
                    Model.SuccessErrorModel = new SuccessErrorPartialViewModel
                    {
                        ShowError = true,
                        Message = Message
                    };

                    LocalUtilities.SetLayoutViewModel(ViewData: ViewData, Model: Model, Key: Constants.ViewData.LayoutViewModel);
                }
            }
        }

        public static void InitSuccessMessage(string Message = null, HttpSessionStateBase Session = null)
        {
            if (Session != null)
            {
                if (Message == null)
                {
                    Message = Resources.TextSuccess;
                }

                Session[Constants.Session.SuccessErrorMessage.Success] = Message;
            }
        }

        public static void PrepareSuccessErrorMessageForJavascript<T>(ViewDataDictionary ViewData)  where T : LayoutModelBase
        {
            if (ViewData != null)
            {
                var Model = LocalUtilities.GetLayoutViewModel<T>(ViewData: ViewData, Key: Constants.ViewData.LayoutViewModel);

                if (Model != null && Model.SuccessErrorModel == null)
                {
                    Model.SuccessErrorModel = new SuccessErrorPartialViewModel
                    {
                        IsSuccessErrorObjectGenerate = true
                    };

                    LocalUtilities.SetLayoutViewModel(ViewData:ViewData, Model: Model, Key: Constants.ViewData.LayoutViewModel);
                }
            }
        } 
        #endregion
    }

    [Serializable]
    public class SuccessErrorPartialViewModel
    {
        #region Properties
        public bool IsTop { set; get; }
        public string Message { set; get; }
        public bool ShowError { set; get; }
        public bool ShowSuccess { set; get; }
        public bool IsSuccessErrorObjectGenerate { get; set; }
        #endregion
    }
}