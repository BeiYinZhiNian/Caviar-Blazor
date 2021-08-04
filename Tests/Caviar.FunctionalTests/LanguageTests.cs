using Caviar.SharedKernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using Xunit.Sdk;

namespace Caviar.FunctionalTests
{
    [TestClass]
    public class LanguageTests
    {
        [TestMethod]
        public void ZhLanguageTest()
        {
            var culture = CultureInfo.GetCultureInfo("zh-CN");
            ILanguageService languageService = new InAssemblyLanguageService(culture);
            var text = languageService.Resources["Language.test"].ToString();
            Assert.AreEqual("∂‡”Ô—‘π¶ƒ‹≤‚ ‘", text);
        }

        [TestMethod]
        public void EnLanguageTest()
        {
            var culture = CultureInfo.GetCultureInfo("en-US");
            ILanguageService languageService = new InAssemblyLanguageService(culture);
            var text = languageService.Resources["Language.test"].ToString();
            Assert.AreEqual("Multilingual function test", text);
        }
    }
}
