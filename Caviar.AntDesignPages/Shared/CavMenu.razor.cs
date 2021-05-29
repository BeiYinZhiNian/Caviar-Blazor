using AntDesign;
using Caviar.Models.SystemData;
using Caviar.AntDesignPages.Helper;
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

namespace Caviar.AntDesignPages.Shared
{
    partial class CavMenu
    {
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
        public MenuItem BreadcrumbItemCav { get; set; }
        [Parameter]
        public EventCallback<MenuItem> BreadcrumbItemCavChanged { get; set; }

        public string[] OpenKeysNav { get; set; } = Array.Empty<string>();

        string[] _openKeysNae;

        [Inject]
        HttpHelper Http { get; set; }
        [Inject]
        UserConfigHelper UserConfig { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        public async void OnMenuItemClickedNav(MenuItem menuItem)
        {
            BreadcrumbItemCav = menuItem;
            if (BreadcrumbItemCavChanged.HasDelegate)
            {
                await BreadcrumbItemCavChanged.InvokeAsync(BreadcrumbItemCav);
                
            }
        }


        public void MenuItemClick(ViewMenu menu)
        {
            UserConfig.CurrentMenuId = menu.Id;
            NavigationManager.NavigateTo(menu.Url);
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

        protected override async Task OnInitializedAsync()
        {
            SysMenus = await GetMenus();
        }

        private List<ViewMenu> SysMenus;

        async Task<List<ViewMenu>> GetMenus()
        {
            var result = await Http.GetJson<List<ViewMenu>>("Menu/GetLeftSideMenus");
            if (result.Status != 200) return new List<ViewMenu>();
            return result.Data;
        }


    }


}
