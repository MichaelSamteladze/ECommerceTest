using Core;
using Core.Properties;
using Core.Utilities;
using DevExpress.Data;
using DevExpress.Data.Linq;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using static Core.Utilities.Constants;

namespace Reusables.Core
{
    #region Controller Base Classes
    public class ProjectController : Controller
    {
        #region Properties
        public bool IsUserLoggedIn => User != null;
        public new User User { get; set; }
        #endregion Properties        
    }
    #endregion

    #region Devexpress
    public abstract class DevexpressGridViewModelBase : DevexpressTypesBase
    {
        #region Methods
        public virtual LinqServerModeDataSourceSelectEventArgs GetLinqServerModeDataSourceSelectEventArgs(LinqServerModeDataSourceSelectEventArgs EventArgs)
        {
            return EventArgs;
        }

        public ServerModeExceptionThrownEventArgs GetLinqServerModeDataSourceSelectEventArgs(ServerModeExceptionThrownEventArgs EventArgs)
        {
            EventArgs.Exception.Message.LogString();
            return EventArgs;
        }

        public abstract void InitGridSettings(GridViewSettings Settings, HtmlHelper Html, UrlHelper Url);

        public void InitGridViewDefaultSettings(GridViewSettings Settings)
        {
            Settings.Width = Unit.Percentage(100);
            Settings.Settings.ShowFilterRow = true;
            Settings.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            Settings.Settings.ShowFilterRowMenu = true;
            Settings.SettingsBehavior.AllowFocusedRow = true;
            Settings.SettingsBehavior.ConfirmDelete = true;
            Settings.SettingsEditing.Mode = GridViewEditingMode.Inline;
            Settings.CommandColumn.Width = Unit.Pixel(90);
            Settings.SettingsCommandButton.RenderMode = GridCommandButtonRenderMode.Link;
            Settings.SettingsCommandButton.NewButton.Text = "<i class=\"fas fa-plus\"></i> ";
            Settings.SettingsCommandButton.DeleteButton.Text = "<i class=\"fas fa-trash-alt\"></i> ";
            Settings.SettingsCommandButton.EditButton.Text = "<i class=\"fas fa-pencil-alt\"></i> ";
            Settings.SettingsCommandButton.UpdateButton.Text = "<i class=\"fas fa-check\"></i> ";
            Settings.SettingsCommandButton.CancelButton.Text = "<i class=\"fas fa-minus-circle\"></i> ";
            Settings.Settings.ShowFooter = true;
            Settings.SettingsPager.Visible = true;
            Settings.SettingsPager.AlwaysShowPager = true;
            Settings.SettingsPager.PageSize = 30;
            Settings.SettingsPager.PageSizeItemSettings.Visible = true;
            Settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "30", "50", "100" };

            Settings.SettingsExport.EnableClientSideExportAPI = true;
            Settings.Settings.GridLines = GridLines.Both;

            Settings.CallbackRouteValues = UrlList;
            Settings.SettingsEditing.AddNewRowRouteValues = UrlAddNew;
            Settings.SettingsEditing.UpdateRowRouteValues = UrlUpdate;
            Settings.SettingsEditing.DeleteRowRouteValues = UrlDelete;
            Settings.CustomActionRouteValues = UrlCustomAction;

            Settings.SettingsText.ConfirmDelete = Resources.TextConfirmDelete;
            //Settings.SettingsPager.Summary.Text = Resources.TextGridPager;
            //Settings.SettingsPager.Summary.EmptyText = Resources.TextNoData;
            //Settings.SettingsText.EmptyDataRow = Resources.TextNoData;
            //Settings.SettingsPager.PageSizeItemSettings.Caption = Resources.TextPageSize;

            Settings.CommandColumn.ShowEditButton = ShowUpdateButton;
            Settings.CommandColumn.ShowDeleteButton = ShowDeleteButton;
            Settings.CommandColumn.Visible = ShowUpdateButton || ShowDeleteButton;


            Settings.Styles.InlineEditCell.Paddings.PaddingLeft = Settings.Styles.InlineEditCell.Paddings.PaddingRight = Unit.Pixel(7);
            Settings.Styles.Footer.Font.Bold = true;

            if (IsError)
            {
                Settings.CustomJSProperties = (sender, e) =>
                {
                    e.Properties["cpErrorMessage"] = ErrorMessage;
                };
            }
            Settings.ClientSideEvents.EndCallback = "function(s,e){ Globals.Devexpress.OnGridEndCallback(s,e); }";
        }
        #endregion
    }

    public class DevexpressTypesBase
    {
        #region Properties        
        public bool ShowAddNewButton { get; set; }
        public bool ShowUpdateButton { get; set; }
        public bool ShowDeleteButton { get; set; }

        public string UrlAddNew { get; set; }
        public string UrlUpdate { get; set; }
        public string UrlNodeDragDrop { get; set; }
        public string UrlDelete { get; set; }
        public string UrlList { get; set; }
        public string UrlCustomAction { get; set; }

        public bool IsError => !string.IsNullOrWhiteSpace(ErrorMessage);
        public string ErrorMessage { get; set; }
        #endregion

        #region Methods
        public void InitCheckBoxProperties(CheckBoxProperties Properties, string GridName = null, string TextChecked = null, string TextUnChecked = null)
        {
            if (!string.IsNullOrWhiteSpace(GridName))
            {
                Properties.ClientSideEvents.Init = $"function(s,e){{ Globals.Devexpress.OnGridCheckBoxColumnEditorInit({GridName},s,e); }}";
            }
            if (string.IsNullOrWhiteSpace(TextChecked) || string.IsNullOrWhiteSpace(TextUnChecked))
            {
                TextChecked = Resources.TextYes;
                TextUnChecked = Resources.TextNo;
            }
            Properties.DisplayTextChecked = TextChecked;
            Properties.DisplayTextUnchecked = TextUnChecked;
        }

        public void InitComboBoxProperties<T1, T2>(ComboBoxProperties Properties, IEnumerable<SimpleKeyValue<T1, T2>> DataSource, bool MakeRequired = false)
        {
            Properties.DataSource = DataSource;
            Properties.TextField = "Value";
            Properties.ValueField = "Key";
            Properties.ValueType = typeof(int);
            Properties.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            if (MakeRequired)
            {
                Properties.ValidationSettings.RequiredField.IsRequired = true;
                Properties.ValidationSettings.RequiredField.ErrorText = Resources.ValidationRequiredField;
            }
        }

        public void InitDateEditProperties(DateEditProperties Properties, bool UseDateTimeFormat = false, bool MakeRequired = false)
        {
            Properties.DisplayFormatString =
            Properties.EditFormatString = UseDateTimeFormat ? Formats.DateTime : Formats.Date;
            Properties.TimeSectionProperties.Visible = UseDateTimeFormat;

            if (MakeRequired)
            {
                Properties.ValidationSettings.RequiredField.IsRequired = true;
                Properties.ValidationSettings.RequiredField.ErrorText = Resources.ValidationRequiredField;
            }
        }

        public void InitSpinEditProperties(SpinEditProperties Properties, int DecimalPlaces = 0, bool IsPercent = false, bool MakeRequired = false, decimal? MinValue = null, decimal? MaxValue = null)
        {
            Properties.AllowMouseWheel = false;
            Properties.SpinButtons.ShowIncrementButtons = false;
            Properties.DisplayFormatInEditMode = true;
            if (DecimalPlaces == DecimalPlace.NO_TRAILING_ZEROES)
            {
                Properties.DisplayFormatString = Formats.DecimalNoTrailingZerosEval;
            }
            else
            {
                Properties.DisplayFormatString = $"n{DecimalPlaces}";
                Properties.DecimalPlaces = DecimalPlaces;
            }

            if (IsPercent)
            {
                Properties.DisplayFormatString = $"{Formats.DecimalNoTrailingZerosEval}%";
                Properties.MinValue = 0;
                Properties.MaxValue = 100;
            }

            if (MakeRequired)
            {
                Properties.ValidationSettings.RequiredField.IsRequired = true;
                Properties.ValidationSettings.RequiredField.ErrorText = Resources.ValidationRequiredField;
            }

            if (MinValue.HasValue)
            {
                Properties.MinValue = MinValue.Value;
            }
            if (MaxValue.HasValue)
            {
                Properties.MaxValue = MaxValue.Value;
            }
        }

        public void InitTextEditProperties(TextBoxProperties Properties, int MaxLength = 0, bool MakeRequired = false)
        {
            if (MaxLength > 0)
            {
                Properties.MaxLength = MaxLength;
            }

            if (MakeRequired)
            {
                Properties.ValidationSettings.RequiredField.IsRequired = true;
                Properties.ValidationSettings.RequiredField.ErrorText = Resources.ValidationRequiredField;
            }
        }
        #endregion

        #region Constants
        public struct DecimalPlace
        {
            public const int NO_TRAILING_ZEROES = -1;
        }
        #endregion
    }

    public abstract class DevexpressTreeListVeiwModelBase : DevexpressTypesBase
    {
        #region Methods        
        public abstract void InitTreeSettings(TreeListSettings Settings, HtmlHelper Html, UrlHelper Url);

        public void InitTreeListDefaultSettings(TreeListSettings Settings)
        {
            Settings.SettingsPager.Visible = true;
            Settings.CommandColumn.NewButton.Text = "<i class=\"fas fa-plus\"></i> ";
            Settings.CommandColumn.DeleteButton.Text = "<i class=\"fas fa-trash-alt\"></i> ";
            Settings.CommandColumn.EditButton.Text = "<i class=\"fas fa-pencil-alt\"></i> ";
            Settings.CommandColumn.UpdateButton.Text = "<i class=\"fas fa-check\"></i> ";
            Settings.CommandColumn.CancelButton.Text = "<i class=\"fas fa-minus-circle\"></i> ";
            Settings.SettingsBehavior.AutoExpandAllNodes = true;
            Settings.SettingsBehavior.AllowFocusedNode = true;

            Settings.Settings.ShowFilterRow = true;
            Settings.Settings.ShowFilterRowMenu = true;
            Settings.CommandColumn.Width = Unit.Pixel(100);
            Settings.Width = Unit.Percentage(100);
            Settings.SettingsText.ConfirmDelete = Resources.TextConfirmDelete;
            Settings.Settings.GridLines = GridLines.Both;

            Settings.CallbackRouteValues = UrlList;
            Settings.SettingsEditing.AddNewNodeRouteValues = UrlAddNew;
            Settings.SettingsEditing.UpdateNodeRouteValues = UrlUpdate;
            Settings.SettingsEditing.DeleteNodeRouteValues = UrlDelete;
            Settings.SettingsEditing.NodeDragDropRouteValues = UrlNodeDragDrop;
            Settings.CustomActionRouteValues = UrlCustomAction;

            if (IsError)
            {
                Settings.CustomJSProperties = (sender, e) =>
                {
                    e.Properties["cpErrorMessage"] = ErrorMessage;
                };
            }
            Settings.ClientSideEvents.EndCallback = "function(s,e){ Globals.Devexpress.OnTreeEndCallback(s,e); }";
        }
        #endregion
    }
    #endregion

    #region Layout
    public class LayoutModelBase
    {
        #region Properties
        public User UserItem { get; set; }
        public bool HasUserItem => UserItem != null;

        public string PageTitle { get; set; }
        public string PageTitleHead { get; set; }

        public bool HasMenu => Menu?.Count > 0;
        public List<ProjectMenuItem> Menu { get; set; }

        public bool HasBreadcrumbs => Breadcrumbs != null && Breadcrumbs.Items?.Count > 0;
        public Breadcrumbs Breadcrumbs { get; set; }

        public SuccessErrorPartialViewModel SuccessErrorModel { get; set; }
        public bool IsSuccessErrorPartialInitiated => SuccessErrorModel != null;

        public string TextError { get; set; } = Resources.TextError;
        public string TextSuccess { get; set; } = Resources.TextSuccess;
        #endregion
    }
    #endregion

    public class FormViewModelBase
    {
        #region Properties
        public List<SimpleKeyValue<string, string>> Errors { get; set; }
        public bool HasErrors => Errors?.Count > 0;
        public string ErrorsJson => Errors.ToJSON();
        public bool IsSaved { get; set; }
        public bool IsError { get; set; }

        public string TextConfirmDeleteAttachment { get; set; } = Resources.TextConfirmDeleteAttachment;
        #endregion

        #region Methods

        public void InitComboBoxSettings(ComboBoxSettings Settings)
        {
            Settings.Width = Unit.Percentage(100);
            Settings.Properties.TextField = "Value";
            Settings.Properties.ValueField = "Key";
            Settings.Properties.ValueType = typeof(int);
            Settings.Properties.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
            Settings.ControlStyle.CssClass = "form-control";
        }

        public void InitDateEditSettings(DateEditSettings Settings, bool UseDateTimeFormat = false)
        {
            Settings.Width = Unit.Percentage(100);
            Settings.Properties.DisplayFormatString =
            Settings.Properties.EditFormatString = UseDateTimeFormat ? Formats.DateTime : Formats.Date;
            Settings.Properties.TimeSectionProperties.Visible = UseDateTimeFormat;
            Settings.Properties.ShowOutOfRangeWarning = false;
            Settings.ControlStyle.CssClass = "form-control";
            Settings.Properties.ClientSideEvents.GotFocus = "function(s, e) { s.ShowDropDown(); }";
        }

        public void InitSpinEditSettings(SpinEditSettings Settings, int DecimalPlaces = 0, bool IsPercent = false)
        {
            Settings.Width = Unit.Percentage(100);

            Settings.Properties.AllowMouseWheel = false;
            Settings.Properties.SpinButtons.ShowIncrementButtons = false;
            Settings.ControlStyle.CssClass = "form-control";

            if (DecimalPlaces == Constants.Decimal.NoTrailingZeros)
            {
                Settings.Properties.DisplayFormatString = Formats.DecimalNoTrailingZerosEval;
            }
            else
            {
                Settings.Properties.DisplayFormatString = $"n{DecimalPlaces}";
                Settings.Properties.DecimalPlaces = DecimalPlaces;
            }

            if (IsPercent)
            {
                Settings.Properties.DisplayFormatString = $"{Formats.DecimalNoTrailingZerosEval}%";
                Settings.Properties.MinValue = 0;
                Settings.Properties.MaxValue = 100;
            }
            Settings.Properties.DisplayFormatInEditMode = true;
            Settings.Properties.ShowOutOfRangeWarning = false;
        }
        #endregion
    }
}