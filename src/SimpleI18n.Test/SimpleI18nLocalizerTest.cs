using System;
using Xunit;
using SimpleI18n;
using SimpleI18n.Test.TestModel;
using System.IO;
using System.Linq;
using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace SimpleI18n.Test
{
    public class SimpleI18nLocalizerTest
    {
        //Use Command "export VSTEST_HOST_DEBUG=1" to debug this test on VsCode
        [Theory]
        [InlineData("en-US", "Fork", "Fork")]
        [InlineData("pt-BR", "Fork", "Garfo")]
        [InlineData("fr-FR", "Fork", "Fourchette")]
        public void TranslationTermWithoutParams(string cultureName, string key, string expectedTranslation)
        {
            //Mocking a configuration file
            var config = GetMockConfiguration(cultureName, false);

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
        public void TranslationTermPluralForm(string cultureName, string key, string expectedTranslation, int quantityParam)
        {
            //Mocking a configuration file
            var config = GetMockConfiguration(cultureName, false);

            //Localizer service
            var localizer = new SimpleI18nStringLocalizer(config);

            var translationTerm = localizer[key, quantityParam].Value;
            Assert.True(string.Equals(expectedTranslation, translationTerm));
        }

        [Theory]
        [InlineData(
            "pt-BR", 
            "My Name is {0} and i live in {1}", 
            "Meu nome é Bruno e moro em Ghent", 
            "Bruno", "Ghent"
        )]
        [InlineData(
            "pt-BR", 
            "My Name is {0} and i live in {1}", 
            "Meu nome é Bruno e moro na Bélgica",
            "Bruno", "/=Belgium"
        )]
        public void TranslationTermWithParams(string cultureName, string key, string expectedTranslation, params object[] translationParams)
        {
            //Mocking a configuration file
            var config = GetMockConfiguration(cultureName, false);

            //Localizer service
            var localizer = new SimpleI18nStringLocalizer(config);

            translationParams = translationParams.
                Select(e => e.ToString().StartsWith("/=") 
                    ? localizer[e.ToString().Replace("/=", "")] : e.ToString()).ToArray();
            
            var translationTerm = localizer[key, translationParams].Value;
            Assert.True(string.Equals(expectedTranslation, translationTerm));
        }

        [Fact]
        public void TranslationTermNotFound()
        {
            var config = GetMockConfiguration("fr-FR", false);
            var localizer = new SimpleI18nStringLocalizer(config);
            var localizedString = localizer["Hat"];
            Assert.True(localizedString.ResourceNotFound);
            Assert.True(localizedString.Value == string.Empty);
        }

        [Fact]
        public void ForceInvalidCulture()
        {
            var pass = false;
            var config = GetMockConfiguration("bxx-XX", false);
            try
            {
                new SimpleI18nStringLocalizer(config);
            }
            catch(Exception e)
            {
                pass = e.Message.StartsWith("Error to load Localizer content");
            }
            Assert.True(pass);
        }

        ///<summary>
        /// Mock a configuration file
        ///</summary>
        private IConfiguration GetMockConfiguration(string cultureName, bool useCurrentThreadCulture)
        {
            return new ConfigurationTestModel
            (
                localeFilesPath: 
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LocaleFiles"),
                defaultCultureName: cultureName,
                useCurrentThreadCulture
            );
        }
    }
}
