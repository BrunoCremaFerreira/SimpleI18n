using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Configuration;

namespace SimpleI18n
{
    //UNDONE: Under Construction!
    public sealed class SimpleI18nStringLocalizer : IStringLocalizer
    {
        private readonly IConfiguration _configuration;

        private string LocaleFilesPath;

        private CultureInfo Culture;

        public SimpleI18nStringLocalizer(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
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

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}