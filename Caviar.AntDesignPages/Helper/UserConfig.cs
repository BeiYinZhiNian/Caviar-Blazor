using Caviar.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Helper
{
    public class UserConfig
    {
        public string White = "#F8F8FF";
        public UserConfig(IJSRuntime jSRuntime)
        {
            JSRuntime = jSRuntime;
            Background = "background:" + White;
            ContentStyle = $"margin: 6px 16px;padding: 24px;min-height: 280px;{Background}";
            HeaderStyle = $"padding:0;{Background}";
        }
        public IJSRuntime JSRuntime { get; set; }

        public Router Router;

        public Action StateHasAction { get; set; }

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
            if (StateHasAction != null)
            {
                StateHasAction.Invoke();
            }
        }
    }
}
