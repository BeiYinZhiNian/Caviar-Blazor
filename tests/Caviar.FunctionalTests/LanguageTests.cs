using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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
            var language = "zh-CN";
            var culture = CultureInfo.GetCultureInfo(language);
            InAssemblyLanguageService.UserLanguage = UserLanguage;
            InAssemblyLanguageService.GetUserLanguageList = LanguageList;
            ILanguageService languageService = new InAssemblyLanguageService(culture);
            var text = languageService.Resources["LanguageTest"].ToString();
            Assert.AreEqual("用户多语言功能测试", text);
            UserSGLanguage();
        }

        string UserLanguage(string name)
        {
            var _resourcesAssembly = Assembly.GetExecutingAssembly();
            var availableResources = LanguageList();
            Assert.AreEqual(availableResources.Count, 3);
            var (_, resourceName) = availableResources.FirstOrDefault(x => x.CultureName.Equals(name, StringComparison.OrdinalIgnoreCase));
            using var fileStream = _resourcesAssembly.GetManifestResourceStream(resourceName);
            using var streamReader = new StreamReader(fileStream);
            return streamReader.ReadToEnd();
        }

        List<(string CultureName, string ResourceName)> LanguageList()
        {
            var _resourcesAssembly = Assembly.GetExecutingAssembly();
            var availableResources = _resourcesAssembly
                    .GetManifestResourceNames()
                    .Select(x => Regex.Match(x, @"^.*Resources.Language\.(.+)\.json"))
                    .Where(x => x.Success)
                    .Select(x => (CultureName: x.Groups[1].Value, ResourceName: x.Value))
                    .ToList();
            return availableResources;
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
                Assert.AreEqual($"没有语言文件 '{name}'", ex.Message);
            }
        }

        void UserSGLanguage()
        {
            var name = "zh-SG";
            var culture = CultureInfo.GetCultureInfo(name);
            ILanguageService languageService = new InAssemblyLanguageService(culture);
            var text = languageService.Resources["LanguageTest"].ToString();
            Assert.AreEqual("用户新增多语言功能测试", text);
        }
    }
}
