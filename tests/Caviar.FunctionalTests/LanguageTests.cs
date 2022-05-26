// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Caviar.SharedKernel.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var resourcesAssembly = Assembly.GetExecutingAssembly();
            var availableResources = LanguageList();
            Assert.AreEqual(availableResources.Count, 3);
            var (_, resourceName) = availableResources.FirstOrDefault(x => x.CultureName.Equals(name, StringComparison.OrdinalIgnoreCase));
            using var fileStream = resourcesAssembly.GetManifestResourceStream(resourceName);
            using var streamReader = new StreamReader(fileStream);
            return streamReader.ReadToEnd();
        }

        List<(string CultureName, string ResourceName)> LanguageList()
        {
            var resourcesAssembly = Assembly.GetExecutingAssembly();
            var availableResources = resourcesAssembly
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
            catch (Exception ex)
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
