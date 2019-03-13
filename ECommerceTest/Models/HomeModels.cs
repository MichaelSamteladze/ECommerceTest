using Core;
using Core.Enums;
using Core.Properties;
using Core.Services;
using Core.Utilities;
using ECommerceTest.Controllers;
using Reusables.Core;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ECommerceTest.Models
{
    public class AboutModel
    {
        #region Methods
        public static PageViewModel GetPageViewModel()
        {
            return new PageViewModel();
        }
        #endregion

        #region Sub Classes
        public class PageViewModel
        {
            #region Properties
            public string PageTitle { get; set; } = Resources.TextAboutUs;
            #endregion
        } 
        #endregion
    }

    public class ContactModel
    {
        #region Methods
        public static PageViewModel GetPageViewModel(ViewDataDictionary ViewData, PageViewModel Model = null)
        {
            if (Model == null)
            {
                Model = new PageViewModel();
            }

            Model.PageTitle = Resources.TextContact;

            var DictionariesList = DictionariesDataAccess.ListDictionaries(Level: 1);

            Model.Countries = DictionariesList?.Where(D => D.DictionaryCode == Dictionaries.Codenames.Countries.DictionaryCode)?.OrderByDescending(I => I.IsDefault)?.ThenBy(C => C.Caption)?.Select(Item => new SimpleKeyValue<int?, string>
            {
                Key = Item.DictionaryID,
                Value = Item.Caption
            }).ToList();

            Model.UsStates = DictionariesList?.Where(D => D.DictionaryCode == Dictionaries.Codenames.UsStates.DictionaryCode)?.Select(Item => new SimpleKeyValue<int?, string>
            {
                Key = Item.DictionaryID,
                Value = Item.Caption
            }).ToList();

            Model.CanadianProvinces = DictionariesList?.Where(D => D.DictionaryCode == Dictionaries.Codenames.CanadianProvinces.DictionaryCode)?.Select(Item => new SimpleKeyValue<int?, string>
            {
                Key = Item.DictionaryID,
                Value = Item.Caption
            }).ToList();

            Model.Topics = DictionariesList?.Where(D => D.DictionaryCode == Dictionaries.Codenames.ContactFormTopics.DictionaryCode)?.Select(Item => new SimpleKeyValue<int?, string>
            {
                Key = Item.DictionaryID,
                Value = Item.Caption
            }).ToList();

            Model.Countries?.ForEach(Item =>
            {
                Item.IsSelected = Item.Key == Model.SelectedCountryID;
            });

            Model.UsStates?.ForEach(Item =>
            {
                Item.IsSelected = Item.Key == Model.SelectedStateID;
            });

            Model.CanadianProvinces?.ForEach(Item =>
            {
                Item.IsSelected = Item.Key == Model.SelectedStateID;
            });

            Model.Topics?.ForEach(Item =>
            {
                Item.IsSelected = Item.Key == Model.SelectedTopicID;
            });

            Model.USCountryID = DictionariesList?.Where(Item => Item.IntCode == CountryCodes.US)?.FirstOrDefault()?.DictionaryID;
            Model.CanadaCountryID = DictionariesList?.Where(Item => Item.IntCode == CountryCodes.CANADA)?.FirstOrDefault()?.DictionaryID;
            Model.CompanyEmail = DictionariesList?.Where(Item => Item.DictionaryCode == Dictionaries.Codenames.Settings.DictionaryCode && Item.IntCode == Settings.CONTACT_EMAIL)?.FirstOrDefault()?.StringCode;
            Model.CompanyPhoneNumber = DictionariesList?.Where(Item => Item.DictionaryCode == Dictionaries.Codenames.Settings.DictionaryCode && Item.IntCode == Settings.CONTACT_PHONE)?.FirstOrDefault()?.StringCode;
            Model.CompanyName = DictionariesList?.Where(Item => Item.DictionaryCode == Dictionaries.Codenames.Settings.DictionaryCode && Item.IntCode == Settings.COMPANY_NAME)?.FirstOrDefault()?.StringCode;

            var LayoutModel = LocalUtilities.GetLayoutViewModel<WebsiteLayoutModel>(ViewData, Constants.ViewData.LayoutViewModel);
            LocalUtilities.SetLayoutViewModel(ViewData: ViewData, Model: LayoutModel, Key: Constants.ViewData.LayoutViewModel);

            return Model;
        }

        public static void SendContactEmail(PageViewModel Model)
        {
            Model.IsMailSent = NotificationManager.SendContactFormEmail(
                FirstName: Model.FirstName,
                LastName: Model.LastName,
                Email: Model.Email,
                Phone: Model.Phone,
                CountryID: Model.SelectedCountryID,
                StateID: Model.SelectedStateID,
                City: Model.City,
                IsProfessional: Model.IsLicensedProfessional,
                TopicID: Model.SelectedTopicID,
                Message: Model.Body
            );
        }

        public static void ValidateContactForm(PageViewModel Model)
        {
            Model.Form.Errors = new List<SimpleKeyValue<string, string>>();

            Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.FirstName)}]", ValueToValidate: Model.FirstName));
            Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.LastName)}]", ValueToValidate: Model.LastName));
            Model.Form.Errors.Add(Validation.ValidateEmail(ErrorKey: $"[name={nameof(Model.Email)}]", Email: Model.Email, ValidateRequired: true, ValidateUnique: false, UserID: null));
            Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.Phone)}]", ValueToValidate: Model.Phone));
            Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.SelectedCountryID)}]", ValueToValidate: Model.SelectedCountryID));
            Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.City)}]", ValueToValidate: Model.City));
            Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.IsLicensedProfessional)}]", ValueToValidate: Model.IsLicensedProfessional));
            Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.SelectedTopicID)}]", ValueToValidate: Model.SelectedTopicID));
            Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.Body)}]", ValueToValidate: Model.Body));

            var DictionariesList = DictionariesDataAccess.ListDictionaries(1, Dictionaries.Codenames.Countries.DictionaryCode).Where(Item => Item.IsDefault).ToList();

            var IsStateRequired = DictionariesList.Any(Item => Item.DictionaryID == Model.SelectedCountryID);

            if (IsStateRequired)
            {
                Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.SelectedStateID)}]", ValueToValidate: Model.SelectedStateID));
            }

            Model.Form.Errors.RemoveAll(Item => Item == null);
        }
        #endregion

        #region Sub Classes
        public class PageViewModel
        {
            #region Properties
            public string PageTitle { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public List<SimpleKeyValue<int?, string>> Countries { get; set; }
            public int? SelectedCountryID { get; set; }
            public List<SimpleKeyValue<int?, string>> UsStates { get; set; }
            public List<SimpleKeyValue<int?, string>> CanadianProvinces { get; set; }
            public int? SelectedStateID { get; set; }
            public string City { get; set; }
            public bool? IsLicensedProfessional { get; set; } = null;
            public List<SimpleKeyValue<int?, string>> Topics { get; set; }
            public int? SelectedTopicID { get; set; }
            public string Body { get; set; }
            public FormViewModelBase Form { get; set; } = new FormViewModelBase();
            public bool IsMailSent { get; set; }
            public string CompanyEmail { get; set; }
            public string CompanyPhoneNumber { get; set; }
            public string CompanyName { get; set; }
            public int? USCountryID { get; set; }
            public int? CanadaCountryID { get; set; }
            #endregion
        }
        #endregion
    }    

    public class FaqModel
    {
        #region Methods
        public static PageViewModel GetPageViewModel(ViewDataDictionary ViewData)
        {
            var Model = new PageViewModel
            {
                PageTitle = Resources.TextFAQ,
                Faq = FaqDataAccess.ListFaq(IsPublished: true).Select(Item => new PageViewModel.QA
                {
                    Question = Item.Question,
                    Answer = Item.Answer
                }).ToList()
            };
            return Model;
        }
        #endregion

        #region Sub Classes
        public class PageViewModel
        {
            #region Properties
            public string PageTitle { get; set; }
            public List<QA> Faq { get; set; }
            public bool HasFaq => Faq?.Count > 0;
            #endregion

            #region Sub Classes
            public class QA
            {
                #region Properties
                public string Question { get; set; }
                public string Answer { get; set; }
                #endregion
            }
            #endregion
        }
        #endregion
    }

    public class HomeModel
    {
        #region Methods
        public static PageViewModel GetPageViewModel(UrlHelper Url, ViewDataDictionary ViewData)
        {
            var Model = new PageViewModel
            {
                Carousel = CarouselDataAccess.ListCarouselItems(IsPublished: true)?.Select(Item => new PageViewModel.CarouselSingleItem
                {
                    CarouselCaption = Item.CarouselCaption,
                    CarouselText = Item.CarouselText,
                    CarouselUrl = Item.CarouselUrl,
                    CarouselImageHttpPath = string.IsNullOrWhiteSpace(Item.CarouselImageFilename) ? AppSettings.NoImagePath : Utility.GetUploadedFileHttpPath(Item.CarouselImageFilename)
                }).ToList(),
                Categories = CategoriesDataAccess.ListCategories(IsPublished: true)?.Select(Item => new PageViewModel.CategoryItem
                {
                    CategoryCaption = Item.CategoryCaption,
                    CategoryDescription = Item.CategoryDescription,
                    CategoryNavigateUrl = Url.RouteUrl(ControllerActionRouteNames.Website.Products.ProductsByCategory, new { Item.CategorySlug }),
                    CategoryImageFilename = string.IsNullOrWhiteSpace(Item.CategoryImageFilename) ? AppSettings.NoImagePath : Utility.GetUploadedFileHttpPath(Item.CategoryImageFilename)
                }).ToList()
            };

            return Model;
        }
        #endregion

        #region Sub Clases
        public class PageViewModel
        {
            #region Properties            
            public List<CarouselSingleItem> Carousel { get; set; }
            public bool HasCarousel => Carousel?.Count > 0;
            public List<CategoryItem> Categories { get; set; }
            public bool HasCategories => Categories?.Count > 0;
            #endregion

            #region Sub Classes
            public class CarouselSingleItem : CarouselItem
            {
                #region Properties
                public string CarouselImageHttpPath { get; set; }
                public bool HasUrl => CarouselUrl?.Length > 0;
                #endregion
            }

            public class CategoryItem : Category
            {
                #region Properties
                public string CategoryImageHttpPath { get; set; }
                public string CategoryNavigateUrl { get; set; }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}