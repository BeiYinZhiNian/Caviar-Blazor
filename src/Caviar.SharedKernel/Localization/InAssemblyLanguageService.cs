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
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace Caviar.SharedKernel.Entities
{
    public class InAssemblyLanguageService : ILanguageService
    {
        private readonly Assembly _resourcesAssembly;
        public static Func<string, string> UserLanguage { get; set; }
        public static Func<List<(string CultureName, string ResourceName)>> GetUserLanguageList { get; set; }
        private static Dictionary<string, JObject> _resourcesCache { get; set; } = new Dictionary<string, JObject>();

        private static object _resourceLock = new object();

        public InAssemblyLanguageService(CultureInfo culture)
        {
            _resourcesAssembly = Assembly.GetExecutingAssembly();
            SetLanguage(culture);
        }

        public InAssemblyLanguageService()
        {
            _resourcesAssembly = Assembly.GetExecutingAssembly();
            var culture = CultureInfo.GetCultureInfo(CurrencyConstant.DefaultLanguage);
            SetLanguage(culture);
        }

        public CultureInfo CurrentCulture { get; private set; }

        public JObject Resources { get; private set; }


        public string this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name)) return "";
                var arr = name.Split(".");
                JToken jtoken = null;
                foreach (var item in arr)
                {
                    if (jtoken == null)
                    {
                        jtoken = Resources[item];
                    }
                    else
                    {
                        jtoken = jtoken[item];
                    }

                }
                if (jtoken == null) return arr[arr.Length - 1];
                return jtoken.ToString();
            }
        }

        public event EventHandler<CultureInfo> LanguageChanged;


        public void SetLanguage(CultureInfo culture)
        {
            if (QueryCache(culture)) return;
            //防止_resourcesCache多次添加对象
            lock (_resourceLock)
            {
                if (QueryCache(culture)) return;
                //设置当请语言
                CurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                Resources = GetKeysFromCulture(culture.Name);
                _resourcesCache.Add(culture.Name, Resources);
                LanguageChanged?.Invoke(this, culture);
            }
        }

        private bool QueryCache(CultureInfo culture)
        {
            if (_resourcesCache.ContainsKey(culture.Name))
            {
                Resources = _resourcesCache[culture.Name];
                if (CurrentCulture == null || !CurrentCulture.Equals(culture))
                {
                    CurrentCulture = culture;
                    CultureInfo.DefaultThreadCurrentCulture = culture;
                    CultureInfo.DefaultThreadCurrentUICulture = culture;
                }
                LanguageChanged?.Invoke(this, culture);
                return true;
            }
            return false;
        }

        private JObject GetKeysFromCulture(string name)
        {

            try
            {
                var content = "";
                JObject jobject = new JObject();
                var availableResources = GetAssemblyLanguageList();
                var (_, resourceName) = availableResources.FirstOrDefault(x => x.CultureName.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(resourceName))
                {
                    using var fileStream = _resourcesAssembly.GetManifestResourceStream(resourceName);
                    using var streamReader = new StreamReader(fileStream);
                    content = streamReader.ReadToEnd();
                    jobject.Merge(JObject.Parse(content));
                }
                content = UserLanguage?.Invoke(name);
                if (!string.IsNullOrEmpty(content))
                {
                    jobject.Merge(JObject.Parse(content));
                }
                if (jobject.Count == 0)
                {
                    throw new FileNotFoundException($"没有语言文件 '{name}'");
                }
                return jobject;
            }
            catch
            {
                throw new FileNotFoundException($"没有语言文件 '{name}'");
            }
        }

        private List<(string CultureName, string ResourceName)> GetAssemblyLanguageList()
        {
            var availableResources = _resourcesAssembly
                .GetManifestResourceNames()
                .Select(x => Regex.Match(x, @"^.*Resources.Language\.(.+)\.json"))
                .Where(x => x.Success)
                .Select(x => (CultureName: x.Groups[1].Value, ResourceName: x.Value))
                .ToList();
            return availableResources;
        }

        public List<(string CultureName, string ResourceName)> GetLanguageList()
        {
            var list = GetUserLanguageList?.Invoke();
            if (list != null) return list;
            return GetAssemblyLanguageList();
        }

        public void SetLanguage(string cultureName)
        {
            if (string.IsNullOrEmpty(cultureName)) return;
            var culture = CultureInfo.GetCultureInfo(cultureName);
            SetLanguage(culture);
        }

        public async void SetLanguage(ValueTask<string> taskCultureName)
        {
            var cultureName = await taskCultureName;
            if (string.IsNullOrEmpty(cultureName)) return;
            var culture = CultureInfo.GetCultureInfo(cultureName);
            SetLanguage(culture);
        }
    }
}
