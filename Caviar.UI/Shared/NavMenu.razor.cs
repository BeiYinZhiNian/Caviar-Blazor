using AntDesign;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Caviar.UI.Shared
{
    partial class NavMenu
    {
        [Inject]
        HttpClient Http { get; set; }
        [Inject]
        IConfiguration Configuration { get; set; }
        bool _inlineCollapsed;
        [Parameter]
        public bool InlineCollapsed
        {
            get { return _inlineCollapsed; }
            set
            {
                OnCollapsed(value);
                _inlineCollapsed = value;
            }
        }

        [Parameter]
        public MenuItem BreadcrumbItemNav { get; set; }
        [Parameter]
        public EventCallback<MenuItem> BreadcrumbItemNavChanged { get; set; }

        public string[] OpenKeysNav { get; set; } = Array.Empty<string>();

        string[] _openKeysNae;


        public async void OnMenuItemClickedNav(MenuItem menuItem)
        {
            BreadcrumbItemNav = menuItem;
            if (BreadcrumbItemNavChanged.HasDelegate)
            {
                await BreadcrumbItemNavChanged.InvokeAsync(BreadcrumbItemNav);
            }
        }

        /// <summary>
        /// 当收缩时候将打开的菜单关闭，防止出现第二菜单。
        /// </summary>
        /// <param name="collapsed"></param>
        public void OnCollapsed(bool collapsed)
        {
            if (collapsed == _inlineCollapsed) return;
            if (collapsed)
            {
                _openKeysNae = OpenKeysNav;
                OpenKeysNav = Array.Empty<string>();
            }
            else
            {
                OpenKeysNav = _openKeysNae;
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            

        }
    }


}
