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

        [Theory]
        [InlineData("pt-BR", "Animal", "Animal", 0)]
        [InlineData("pt-BR", "Animal", "Animal", 1)]
        [InlineData("pt-BR", "Animal", "Animais", 2)]
        [InlineData("pt-BR", "{0} example", "nenhum exemplo", 0)]
        [InlineData("pt-BR", "{0} example", "1 exemplo", 1)]
        [InlineData("pt-BR", "{0} example", "2 exemplos", 2)]
        [InlineData("pt-BR", "{0} example", "230 exemplos", 230)]
        public void TranslationTermPluralform(string cultureName, string key, string expectedTranslation, int quantityParam)
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

            var translationTerm = localizer[key, quantityParam].Value;
            Assert.True(string.Equals(expectedTranslation, translationTerm));
        }
    }
}
