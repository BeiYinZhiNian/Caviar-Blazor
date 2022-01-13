using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.View;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using Xunit.Sdk;

namespace Caviar.FunctionalTests
{
    [TestClass]
    public class LanguageTests
    {
        [TestMethod]
        public void CnLanguageTest()
        {
            var culture = CultureInfo.GetCultureInfo("zh-CN");
            ILanguageService languageService = new InAssemblyLanguageService(culture);
            var text = languageService["LanguageTest"];
            Assert.AreEqual("∂‡”Ô—‘π¶ƒ‹≤‚ ‘", text);
        }

        [TestMethod]
        public void UsLanguageTest()
        {
            var culture = CultureInfo.GetCultureInfo("en-US");
            ILanguageService languageService = new InAssemblyLanguageService(culture);
            var text = languageService.Resources["LanguageTest"].ToString();
            Assert.AreEqual("Multilingual function test", text);
        }
    }
}
