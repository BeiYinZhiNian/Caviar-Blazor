using AntDesign;
using Caviar.SharedKernel.View;
using Caviar.AntDesignUI.Helper;
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
using Microsoft.AspNetCore.Http;
using Caviar.SharedKernel.Entities.View;
using Microsoft.JSInterop;

namespace Caviar.AntDesignUI.Shared
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
        public MenuTheme Theme { get; set; } = MenuTheme.Dark;

        public Menu AntDesignMenu { get; set; }
        [Parameter]
        public EventCallback<MenuItem> BreadcrumbItemCavChanged { get; set; }

        public string[] SelectedKeys { get; set; }

        public string[] OpenKeysNav { get; set; } = Array.Empty<string>();

        string[] _openKeysNae;

        [Inject]
        HttpHelper Http { get; set; }
        [Inject]
        UserConfig UserConfig { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }

        [Inject]
        IJSRuntime jSRuntime { get; set; }
        public async void OnMenuItemClickedNav(MenuItem menuItem)
        {
            BreadcrumbItemCav = menuItem;
            IframeMessage iframeMessage = new IframeMessage()
            {
                Action = "switch_wasm",
                Data = menuItem.RouterLink
            };
            _ = jSRuntime.InvokeVoidAsync("iframeMessage", iframeMessage);
            if (BreadcrumbItemCavChanged.HasDelegate)
            {
                await BreadcrumbItemCavChanged.InvokeAsync(BreadcrumbItemCav);
                
            }
        }


        public void MenuItemClick(SysMenuView menu)
        {
            NavigationManager.NavigateTo(menu.Entity.Url);
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
        protected override void OnParametersSet()
        {
            UserConfig.RefreshMenuAction = Refresh;
            base.OnParametersSet();
        }

        protected override async Task OnInitializedAsync()
        {
            SysMenus = await GetMenus();
        }

        private List<SysMenuView> SysMenus;

        public async void Refresh()
        {
            await OnInitializedAsync();
            StateHasChanged();
            foreach (var item in AntDesignMenu.MenuItems)
            {
                if(item.Key == AntDesignMenu.SelectedKeys.FirstOrDefault())
                {
                    OnMenuItemClickedNav(item);
                }
            }
            
        }

        async Task<List<SysMenuView>> GetMenus()
        {
            var result = await Http.GetJson<List<SysMenuView>> ("SysMenu/GetMenuBar");
            if (result.Status != StatusCodes.Status200OK) return new List<SysMenuView>();
            return result.Data;
        }


    }


}
