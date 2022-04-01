using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using Xunit.Sdk;

namespace Caviar.FunctionalTests
{
    [TestClass]
    public class LanguageTests
    {

        [TestMethod]
        public void UsLanguageTest()
        {
            var culture = CultureInfo.GetCultureInfo("en-US");
            ILanguageService languageService = new InAssemblyLanguageService(culture);
            var text = languageService.Resources["LanguageTest"].ToString();
            Assert.AreEqual("Multilingual function test", text);
        }

        [TestMethod]
        public void UserMergeLanguageTest()
        {
            var culture = CultureInfo.GetCultureInfo("zh-CN");
            ILanguageService languageService = new InAssemblyLanguageService(culture);
            var text = languageService.Resources["LanguageTest"].ToString();
            Assert.AreEqual("�û������Թ��ܲ���", text);
        }

        [TestMethod]
        public void NoneLanguageTest()
        {
            var name = "zh-HK";
            try
            {
                var culture = CultureInfo.GetCultureInfo(name);
                ILanguageService languageService = new InAssemblyLanguageService(culture);
            }
            catch(Exception ex)
            {
                Assert.AreEqual($"û�������ļ� '{name}'", ex.Message);
            }
        }
    }
}
