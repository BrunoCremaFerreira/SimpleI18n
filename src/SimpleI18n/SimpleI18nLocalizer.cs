using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;

namespace SimpleI18n
{
    ///<summary>
    /// String Translator Service
    ///</summary>
    public sealed class SimpleI18nStringLocalizer : IStringLocalizer
    {
        ///<summary>
        /// System configuration
        ///</summary>
        private readonly IConfiguration _configuration;

        ///<summary>
        /// Translation JSON Files
        ///</summary>
        private string LocaleFilesPath;

        ///<summary>
        /// Culture to which it will be translated.
        ///</summary>
        private CultureInfo Culture;

        private JObject ResourceContent;

        private string LocaleFileName => Path.Combine(LocaleFilesPath, $"{Culture.Name}.json");

        #region :: Constructors

        public SimpleI18nStringLocalizer(IConfiguration configuration)
        {
            _configuration = configuration;
            LoadConfiguration(_configuration);
            LoadLocaleFile();
        }

        public SimpleI18nStringLocalizer(IConfiguration configuration, CultureInfo culture)
            :this(configuration)
        {
            Culture = culture;
            LoadLocaleFile();
        }

        #endregion
        
        public LocalizedString this[string name] => GetTranslation(name);

        public LocalizedString this[string name, params object[] arguments] => GetTranslation(name, arguments);

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return ResourceContent.Values().
                Select(i=> new LocalizedString(i.Type.ToString(), i.Value<string>()));
        }

        ///<summary>
        /// Creates a new String Translation Service with a new culture
        ///</summary>
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new SimpleI18nStringLocalizer(_configuration, culture);
        }

        private LocalizedString GetTranslation(string keyName, params object[] arguments)
        {
            var hasTranslation = ResourceContent.TryGetValue(keyName, out var jToken) 
                                    && jToken.HasValues;
            
            //If the translation key was not found
            if (!hasTranslation)
                return new LocalizedString(keyName, string.Empty, true, LocaleFileName);

            var canBePluralForm = IsNumericType(arguments.FirstOrDefault());
            var translationsForKey = jToken.Values();

            //Plural form translation
            if (translationsForKey.Count() > 1 && canBePluralForm)
            {
                var pluralQuantity = Math.Abs((double)arguments.First());
                dynamic dynToken = jToken;
                string val = pluralQuantity == 0 
                                ? dynToken.Zero
                                : (pluralQuantity == 1 ? dynToken.One : dynToken.Other);
                return new LocalizedString(
                        keyName, string.Format(val, arguments), string.IsNullOrWhiteSpace(val));
            }
            
            //Single translation
            var value = translationsForKey.FirstOrDefault()?.Value<string>() ?? string.Empty;
            return new LocalizedString(
                        keyName, string.Format(value, arguments), string.IsNullOrWhiteSpace(value));
        }

        ///<summary>
        /// Load and prepare Localizer configuration
        ///</summary>
        private void LoadConfiguration(IConfiguration configuration)
        {
            try
            {
                LocaleFilesPath = configuration["SimpleI18n:LocaleFilesPath"] ?? 
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LocaleFiles");
            
                var cultureName = configuration["SimpleI18n:DefaultCultureName"] ?? "en-Us";
            }
            catch(Exception e)
            {
                throw new Exception("Error to load Localizer configuration! Take a look at InnerException for more details.", e);
            }
            
        }

        ///<summary>
        /// Reads the JSON translation file resource and loads it into memory
        ///</summary>
        private void LoadLocaleFile()
        {
            try
            {
                var fileContent = File.ReadAllText(LocaleFileName);

                if (string.IsNullOrWhiteSpace(fileContent))
                    return;

                ResourceContent = JObject.Parse(fileContent);
                
            }
            catch(Exception e)

            {
                throw new Exception($"Error to load Localizer content from '{LocaleFileName}'.", e);
            }

        }

        private bool IsNumericType(object obj)
        {   
            switch (Type.GetTypeCode(obj.GetType()))
            {
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}