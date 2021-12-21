using Blazored.LocalStorage;
using Caviar.SharedKernel;
using Caviar.SharedKernel.View;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Helper
{
    public class UserConfig
    {
        public string White = "#F8F8FF";

        protected ILocalStorageService _ILocalStorageService { get; set; }
        public UserConfig(IJSRuntime jSRuntime, ILanguageService languageService,ILocalStorageService localStorage)
        {
            JSRuntime = jSRuntime;
            Background = $"background:{White};";
            ContentStyle = $"margin: 6px 16px;padding: 24px;min-height: 280px;{Background}";
            HeaderStyle = $"padding:0;{Background}";
            _ILocalStorageService = localStorage;
            var cultureName = localStorage.GetItemAsStringAsync(CurrencyConstant.LanguageHeader);
            LanguageService = languageService;
            LanguageService.SetLanguage(cultureName);
            LanguageService.LanguageChanged += LanguageService_LanguageChanged;
            
        }

        private void LanguageService_LanguageChanged(object sender, System.Globalization.CultureInfo e)
        {
            _ILocalStorageService.SetItemAsStringAsync(CurrencyConstant.LanguageHeader, e.Name);
            RefreshMenuAction?.Invoke();
            RefreshCurrentPage?.Invoke();
        }

        public IJSRuntime JSRuntime { get; set; }

        public Router Router;
        /// <summary>
        /// 更新菜单数据
        /// </summary>
        public Action RefreshMenuAction { get; set; }

        /// <summary>
        /// 更新当前页面数据
        /// </summary>
        public Action RefreshCurrentPage { get; set; }

        IEnumerable _routes;
        public IEnumerable Routes()
        {
            if (_routes == null)
            {
                var routes = Router.GetObjValue("Routes");
                _routes = (IEnumerable)routes.GetObjValue("Routes");
            }
            return _routes;
        }
        /// <summary>
        /// 手风琴模式
        /// </summary>
        public bool Accordion { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string Theme { 
            get { return _theme; } 
            set {
                SetTheme(_theme, value);
                _theme = value; 
            } 
        }

        public ILanguageService LanguageService { get; set; }

        public string CurrentLanguage => LanguageService.CurrentCulture.Name;

        public string Background { get; set; }

        public string ContentStyle { get; set; }

        public string HeaderStyle { get; set; }

        /// <summary>
        /// 是否table页
        /// </summary>
        public bool IsTable { get; set; }
        


        private string _theme = "ant-design-blazor.css";

        public async void SetTheme(string oldThemeName,string newThemeName)
        {
            await JSRuntime.InvokeVoidAsync("loadCss", oldThemeName, newThemeName);
            switch (newThemeName)
            {
                case "ant-design-blazor.dark.css":
                    Background = "background:#2E2E2E";
                    ContentStyle = $"margin: 6px 16px;padding: 24px;min-height: 280px;{Background}";
                    HeaderStyle = $"padding:0;{Background}";
                    break;
                default:
                    Background = "background:" + White;
                    ContentStyle = $"margin: 6px 16px;padding: 24px;min-height: 280px;{Background}";
                    HeaderStyle = $"padding:0;{Background}";
                    break;
            }
        }
    }
}
