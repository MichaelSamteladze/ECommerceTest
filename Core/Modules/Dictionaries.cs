using Core.DB;
using Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class DictionariesDataAccess : SixtyThreeBitsDataObject
    {
        #region Methods
        public static int? GetDictionaryIDByDictionaryCodeAndIntCode(int? DictionaryCode, int? intCode)
        {
            return TryToReturnStatic($"{nameof(GetDictionaryIDByDictionaryCodeAndIntCode)}({nameof(DictionaryCode)} = {DictionaryCode}, {nameof(intCode)} = {intCode})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.DictionaryGetDictionaryIDByDictionaryCodeAndIntCode(DictionaryCode, intCode);
                }
            });
        }

        public static string GetDictionarySettingsStringCodeByIntCode(int? intCode)
        {
            return TryToReturnStatic($"{nameof(GetDictionarySettingsStringCodeByIntCode)}({nameof(intCode)} = {intCode})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.DictionaryGetDictionarySettingsStrigCodeByIntCode(intCode);
                }
            });
        }

        public static List<DictionariesListResult> ListDictionaries(int? Level = null, int? DictionaryCode = null, bool? IsVisible = null)
        {
            return TryToReturnStatic($"{nameof(ListDictionaries)}({nameof(Level)} = {Level}, {nameof(DictionaryCode)} = {DictionaryCode}, {nameof(IsVisible)} = {IsVisible})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.DictionariesList(Level, DictionaryCode, IsVisible).OrderBy(Item => Item.SortIndex).ToList();
                }
            });
        }

        public static List<SimpleKeyValue<int?, string>> ListDictionaryItemsAsKeyValueByDictionaryCodename(Dictionaries.Codename DictionaryCodenameItem, ICacheAssistance Cache = null)
        {
            if(Cache == null)
            {
                Cache = new MemoryCacheAssistance();
            }

            var Items = Cache.GetDataFromCache<List<SimpleKeyValue<int?, string>>>(DictionaryCodenameItem.DictionaryName);
            if (Items == null)
            {
                Items = ListDictionaries(Level: 1, DictionaryCode: DictionaryCodenameItem.DictionaryCode).Select(Item => new SimpleKeyValue<int?, string> { Key = Item.DictionaryID, Value = Item.Caption }).ToList();
                if (Items != null)
                {
                    Cache.SetDataToCache(DictionaryCodenameItem.DictionaryName, Items, DateTime.Now.AddMinutes(AppSettings.CacheDurationInSecondsDefault));
                }
            }

            return Items.Clone();
        }

        public void DictionariesIUD(byte? DatabaseAction = null, int? DictionaryID = null, int? ParentID = null, string Caption = null, string CaptionEng = null, string CaptionRus = null, string StringCode = null, int? IntCode = null, decimal? DecimalValue = null, short? DictionaryCode = null, bool? IsDefault = null, bool? IsVisible = null, int? SortIndex = null)
        {
            TryExecute($"{nameof(DictionariesIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(DictionaryID)} = {DictionaryID}, {nameof(ParentID)} = {ParentID}, {nameof(Caption)} = {Caption}, {nameof(CaptionEng)} = {CaptionEng}, {nameof(CaptionRus)} = {CaptionRus}, {nameof(StringCode)} = {StringCode}, {nameof(IntCode)} = {IntCode}, {nameof(DecimalValue)} = {DecimalValue}, {nameof(DictionaryCode)} = {DictionaryCode}, {nameof(IsDefault)} = {IsDefault}, {nameof(IsVisible)} = {IsVisible}, {nameof(SortIndex)} = {SortIndex})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    db.DictionariesIUD(DatabaseAction, ref DictionaryID, Caption, CaptionEng, CaptionRus, ParentID, StringCode, IntCode, DecimalValue, DictionaryCode, IsDefault, IsVisible, SortIndex);
                }
            });
        }
        #endregion
    }

    public class Dictionaries
    {
        #region Sub Classes
        public class Codenames
        {
            #region Properties
            public static Codename TimeUnits = new Codename { DictionaryName = "DictionaryTimeUnits", DictionaryCode = 1 };
            public static Codename OrderStatuses = new Codename { DictionaryName = "DictionaryOrderStatuses", DictionaryCode = 2 };
            public static Codename PaymentOptions = new Codename { DictionaryName = "DictionaryPaymentMethod", DictionaryCode = 3 };
            public static Codename Countries = new Codename { DictionaryName = "DictionaryCountries", DictionaryCode = 4 };
            public static Codename UsStates = new Codename { DictionaryName = "DictionaryUsStates", DictionaryCode = 5 };
            public static Codename CanadianProvinces = new Codename { DictionaryName = "DictionaryCanadianProvinces", DictionaryCode = 6 };
            public static Codename ContactFormTopics = new Codename { DictionaryName = "DictionaryContactFormTopics", DictionaryCode = 7 };
            public static Codename PaypalTransactionTypes = new Codename { DictionaryName = "DictionatyPaypalTransactionTypes", DictionaryCode = 8 };
            public static Codename Settings = new Codename { DictionaryName = "DictionatySettings", DictionaryCode = 100 };
            #endregion
        }

        public class Codename
        {
            #region Properties
            public string DictionaryName { get; set; }
            public int? DictionaryCode { get; set; }
            #endregion
        }
        #endregion
    }

    public class IsoCodes
    {
        #region Properties
        public string CountryIsoCode { get; set; }
        public string StateIsoCode { get; set; }
        #endregion
    }
}
