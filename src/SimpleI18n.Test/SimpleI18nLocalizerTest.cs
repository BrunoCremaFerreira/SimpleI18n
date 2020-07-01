using System;
using Xunit;
using SimpleI18n;
using SimpleI18n.Test.TestModel;
using System.IO;
using System.Globalization;

namespace SimpleI18n.Test
{
    public class SimpleI18nLocalizerTest
    {
        [Fact]
        public void BestCaseScenario()
        {
            //Mocking a configuration file
            var config = new ConfigurationTestModel
            (
                localeFilesPath: 
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LocaleFiles"),
                defaultCultureName: new CultureInfo("en-Us").Name
            );

            var localizer = new SimpleI18nStringLocalizer(config);
            
        }
    }
}
