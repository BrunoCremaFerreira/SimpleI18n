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
        [Theory]
        [InlineData("en-US", "Fork", "Fork")]
        [InlineData("pt-BR", "Fork", "Garfo")]
        [InlineData("fr-FR", "Fork", "Fourchette")]
        public void TranslationTermWithoutParams(string cultureName, string key, string expectedTranslation)
        {
            //Mocking a configuration file
            var config = new ConfigurationTestModel
            (
                localeFilesPath: 
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LocaleFiles"),
                defaultCultureName: cultureName
            );

            //Localizer service
            var localizer = new SimpleI18nStringLocalizer(config);

            var translationTerm = localizer[key].Value;
            Assert.True(string.Equals(expectedTranslation, translationTerm));
        }
    }
}
