using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
namespace Caviar.SharedKernel
{
    public class InAssemblyLanguageService:ILanguageService
    {
        private readonly Assembly _resourcesAssembly;

        private static Dictionary<string, JObject> ResourcesCache { get; set; } = new Dictionary<string, JObject>();

        public InAssemblyLanguageService(CultureInfo culture)
        {
            _resourcesAssembly = Assembly.GetExecutingAssembly();
            SetLanguage(culture);
        }

        public InAssemblyLanguageService()
        {
            _resourcesAssembly = Assembly.GetExecutingAssembly();
            var culture = CultureInfo.GetCultureInfo("zh-CN");
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
                    if(jtoken == null)
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
            if (ResourcesCache.ContainsKey(culture.Name))
            {
                Resources = ResourcesCache[culture.Name];
                CurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                return;
            }
            if (!culture.Equals(CultureInfo.CurrentCulture))
            {
                CultureInfo.CurrentCulture = culture;
            }

            if (CurrentCulture == null || !CurrentCulture.Equals(culture))
            {
                CurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                Resources = GetKeysFromCulture(culture.Name);
                ResourcesCache.Add(culture.Name, Resources);
                LanguageChanged?.Invoke(this, culture);
            }
        }

        private JObject GetKeysFromCulture(string culture)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            string path = $"{dir}Resources/Language/{culture}.json";
            try
            {
                var content = File.ReadAllText(path);
                return JObject.Parse(content);
            }
            catch
            {
                throw new FileNotFoundException($"没有语言文件 '{culture}'");
            }
        }
    }
}
