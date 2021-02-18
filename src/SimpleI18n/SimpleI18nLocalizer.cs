using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace SimpleI18n
{
    ///<summary>
    /// String Translator Service
    ///</summary>
    public sealed class SimpleI18nStringLocalizer : IStringLocalizer
    {
        private const string GENDER_MARK = "[f]";
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
        private CultureInfo _culture;

        ///<summary>
        /// Current Culture to which it will be translated.
        ///</summary>
        public CultureInfo Culture
            => UseCurrentThreadCulture
                ? Thread.CurrentThread.CurrentUICulture ?? Thread.CurrentThread.CurrentCulture
                : _culture;

        ///<summary>
        /// Use current Thread culture
        ///</summary>
        public bool UseCurrentThreadCulture { get; private set; } = true;

        private JObject ResourceContent;

        private string LocaleFileName 
            => Path.Combine(LocaleFilesPath, $"{Culture.Name}.json");

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
            _culture = culture;
            LoadLocaleFile();
        }

        #endregion
        
        public LocalizedString this[string name] 
            => GetTranslation(name);

        public LocalizedString this[string name, params object[] arguments] 
            => GetTranslation(name, arguments);

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
            => ResourceContent.Values().
                Select(i=> new LocalizedString(i.Type.ToString(), i.Value<string>()));
        
        ///<summary>
        /// Creates a new String Translation Service with a new culture
        ///</summary>
        public IStringLocalizer WithCulture(CultureInfo culture)
            => new SimpleI18nStringLocalizer(_configuration, culture);
        
        private LocalizedString GetTranslation(string keyName, params object[] arguments)
        {
            //Autoset gender if translation ends with "[f]"
            var hasLocalizedGenderF = arguments.Any(e=> e is LocalizedString && e.ToString().EndsWith(GENDER_MARK));
            if(hasLocalizedGenderF)
            {
                var kyHasF = keyName.EndsWith("{f}");
                var args = RemoveGenderMarkOfArguments(arguments);
                return GetTranslation(kyHasF ? keyName : $"{keyName}{{f}}", args);
            }

            var hasTranslation = ResourceContent.TryGetValue(keyName, out var jToken) 
                                    && (jToken.HasValues || jToken.Value<string>() != null);
            
            //If the translation key was not found
            if (!hasTranslation)
                return new LocalizedString(keyName, string.Empty, true, LocaleFileName);

            //Plural form translation
            if (jToken.HasValues)
            {
                var canBePluralForm = IsNumericType(arguments.FirstOrDefault());
                var pluralQuantity = canBePluralForm 
                                        ? Math.Abs(double.Parse(arguments.First().ToString())) 
                                        : 1;

                dynamic dynToken = jToken;
                string val = pluralQuantity == 0 
                                ? dynToken.Zero
                                : (pluralQuantity == 1 ? dynToken.One : dynToken.Other);
                return new LocalizedString(
                        keyName, string.Format(val, arguments), string.IsNullOrWhiteSpace(val));
            }
            
            //Single translation
            var value = jToken.Value<string>() ?? string.Empty;
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

                //If the path of the local files is not found, 
                //try to resolve it by changing the Path AltDirectorySeparatorChar
                if(!Directory.Exists(LocaleFilesPath))
                    LocaleFilesPath = LocaleFileName
                        .Replace('/', Path.AltDirectorySeparatorChar)
                        .Replace('\\', Path.AltDirectorySeparatorChar);
            
                var cultureName = configuration["SimpleI18n:DefaultCultureName"] ?? "en-US";
                _culture = new CultureInfo(cultureName);

                if(bool.TryParse(configuration["SimpleI18n:UseCurrentThreadCulture"], out var useCurrentThreadCulture))
                    UseCurrentThreadCulture = useCurrentThreadCulture;
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
            if(obj == null)
                return false;

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

        private object[] RemoveGenderMarkOfArguments(object[] attributes)
        {
            return attributes
                .Select(e=> (e is LocalizedString) 
                    ? RemoveGenderMark(e.ToString())
                    : e)
                .ToArray();

        }

        public override string ToString()
        {
            var str = base.ToString();
            return RemoveGenderMark(str);
        }

        private string RemoveGenderMark(string source)
            => source.EndsWith(GENDER_MARK)
                ? source.Replace(GENDER_MARK, string.Empty)
                : source;
    }
}