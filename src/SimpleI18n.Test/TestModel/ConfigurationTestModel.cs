using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace SimpleI18n.Test.TestModel
{
    public class ConfigurationTestModel : IConfiguration
    {
        private IDictionary<string, string> InternalConfig;

        public ConfigurationTestModel(string localeFilesPath, string defaultCultureName, bool useCurrentThreadCulture)
        {
            InternalConfig = new Dictionary<string, string>();
            InternalConfig.Add("SimpleI18n:LocaleFilesPath", localeFilesPath);
            InternalConfig.Add("SimpleI18n:DefaultCultureName", defaultCultureName);
            InternalConfig.Add("SimpleI18n:UseCurrentThreadCulture", useCurrentThreadCulture.ToString());
        }

        public string this[string key] 
        { 
            get => InternalConfig[key]; 
            set => InternalConfig[key] = value; 
        }

        #region :: Not Used

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            throw new NotImplementedException();
        }

        public IChangeToken GetReloadToken()
        {
            throw new NotImplementedException();
        }

        public IConfigurationSection GetSection(string key)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
