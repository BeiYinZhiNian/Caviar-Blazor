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
    public class InAssemblyLanguageService:ILanguageService
    {
        private readonly Assembly _resourcesAssembly;

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
            var availableResources = GetLanguageList();
            var (_, resourceName) = availableResources.FirstOrDefault(x => x.CultureName.Equals(name, StringComparison.OrdinalIgnoreCase));
            try
            {
                using var fileStream = _resourcesAssembly.GetManifestResourceStream(resourceName);
                using var streamReader = new StreamReader(fileStream);
                var content = streamReader.ReadToEnd();
                return JObject.Parse(content);
            }
            catch
            {
                throw new FileNotFoundException($"没有语言文件 '{name}'");
            }
        }

        public List<(string CultureName,string ResourceName)> GetLanguageList()
        {
            var availableResources = _resourcesAssembly
                .GetManifestResourceNames()
                .Select(x => Regex.Match(x, @"^.*Resources.Language\.(.+)\.json"))
                .Where(x => x.Success)
                .Select(x => (CultureName: x.Groups[1].Value, ResourceName: x.Value))
                .ToList();
            return availableResources;
        }

        public void SetLanguage(string cultureName)
        {
            if (string.IsNullOrEmpty(cultureName)) return;
            var culture = CultureInfo.GetCultureInfo(cultureName);
            SetLanguage(culture);
        }

        public async void SetLanguage(ValueTask<string> taskCultureName)
        {
            var cultureName =  await taskCultureName;
            if (string.IsNullOrEmpty(cultureName)) return;
            var culture = CultureInfo.GetCultureInfo(cultureName);
            SetLanguage(culture);
        }
    }
}
