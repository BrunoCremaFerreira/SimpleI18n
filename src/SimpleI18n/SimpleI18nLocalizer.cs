using System;
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

        #region :: Constructors

        public SimpleI18nStringLocalizer(IConfiguration configuration)
        {
            _configuration = configuration;
            LoadConfiguration(_configuration);
        }

        public SimpleI18nStringLocalizer(IConfiguration configuration, CultureInfo culture)
            :this(configuration)
        {
            Culture = culture;
        }

        #endregion
        
        public LocalizedString this[string name]
        {
            get { return new LocalizedString(name, string.Empty); }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get { return new LocalizedString(name, string.Empty); }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return new List<LocalizedString>();
        }

        ///<summary>
        /// Creates a new String Translation Service with a new culture
        ///</summary>
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new SimpleI18nStringLocalizer(_configuration, culture);
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
            
                var cultureName = configuration["SimpleI18n:LocaleFilesPath"] ?? "en-Us";
            }
            catch(Exception e)
            {
                throw new Exception("Error to load Localizer configuration! Take a look at InnerException for more details.", e);
            }
            
        }

        ///<summary>
        /// Reads the JSON translation file resource and loads it into memory
        ///</summary>
        private void LoadLocaleFile(string fileName)
        {
            try
            {
                var fileContent = File.ReadAllText(fileName);

                if (string.IsNullOrWhiteSpace(fileContent))
                    return;

                var jSonContent = JObject.Parse(fileContent);
                
            }
            catch(Exception e)

            {
                throw new Exception($"Error to load Localizer content from '{fileName}'.", e);
            }

        }
    }
}