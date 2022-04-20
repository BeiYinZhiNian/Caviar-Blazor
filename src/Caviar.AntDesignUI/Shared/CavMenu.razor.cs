using AntDesign;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using System.Net;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities.View;
using Caviar.SharedKernel.Entities;
using System.Web;
using Newtonsoft.Json;

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
        public string[] BreadcrumbItemArr { get; set; }
        [Parameter]
        public MenuTheme Theme { get; set; } = MenuTheme.Dark;
        [Inject]
        CavLayout Layout { get; set; }
        public Menu AntDesignMenu { get; set; }

        [Parameter]
        public EventCallback<string[]> BreadcrumbItemArrChanged { get; set; }

        public string[] OpenKeysNav { get; set; } = Array.Empty<string>();

        public string[] SelectedKeys { get; set; }

        string[] _openKeysNae;

        [Inject]
        HttpService Http { get; set; }
        [Inject]
        UserConfig UserConfig { get; set; }

        [Inject] 
        NavigationManager NavigationManager { get; set; }
        [Inject]

        ILanguageService LanguageService { get; set; }

        [Inject]
        IJSRuntime JSRuntime { get; set; }




        List<string> CreatBreadcrumbItemCav(MenuItem menuItem)
        {
            if (menuItem == null) return null;
            var breadcrumbItemArr = new List<string>();
            var parent = menuItem.ParentMenu;
            while (parent != null)
            {
                var name = LanguageService[$"{CurrencyConstant.Menu}.{parent.Key}"];
                breadcrumbItemArr.Insert(0, name);
                parent = parent.Parent;
            }
            var menuName = LanguageService[$"{CurrencyConstant.Menu}.{menuItem.Key}"];
            breadcrumbItemArr.Add(menuName);
            return breadcrumbItemArr;
        }

        public void OnMenuItemClickedNav(MenuItem menuItem)
        {
            BreadcrumbItemArr = CreatBreadcrumbItemCav(menuItem)?.ToArray();
            //在server模式下且需要自动切换
            if (Config.IsServer && UserConfig.IsAutomaticSwitchWasm)
            {
                var iframeMessage = new IframeMessage();
                iframeMessage.Pattern = Pattern.Wasm;
                iframeMessage.Url = menuItem.RouterLink;
                iframeMessage.ExchangeData = new ServerToWasmExchange() 
                { 
                    OpenKeysNav = OpenKeysNav,
                    SelectedKeys = new string[] { menuItem.Key },
                    BreadcrumbItemArr = BreadcrumbItemArr
                };
                _ = JSRuntime.InvokeVoidAsync(CurrencyConstant.JsIframeMessage, iframeMessage);
                return;
            }
            if (BreadcrumbItemArrChanged.HasDelegate)
            {
                BreadcrumbItemArrChanged.InvokeAsync(BreadcrumbItemArr);
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
        protected override void OnParametersSet()
        {
            UserConfig.RefreshMenuAction = Refresh;
            base.OnParametersSet();
            if (!Config.IsServer && !Config.IsHandleIframeMessage)
            {
                var uri = new Uri(NavigationManager.Uri);
                var query = HttpUtility.ParseQueryString(uri.Query);
                if (!string.IsNullOrEmpty(query[CurrencyConstant.JsIframeMessage]))
                {
                    Config.IsHandleIframeMessage = true;
                    var iframeMessage = JsonConvert.DeserializeObject<ServerToWasmExchange>(query[CurrencyConstant.JsIframeMessage]);
                    OpenKeysNav = iframeMessage.OpenKeysNav;//打开nav
                    SelectedKeys = iframeMessage.SelectedKeys;//选择key
                    if (BreadcrumbItemArrChanged.HasDelegate)
                    {
                        _ = BreadcrumbItemArrChanged.InvokeAsync(iframeMessage.BreadcrumbItemArr);
                    }
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            SysMenus = await GetMenus();
            UserConfig.Menus = new List<SysMenuView>();// 初始化接收菜单
            SysMenus.TreeToList(UserConfig.Menus);
        }

        protected override void OnAfterRender(bool firstRender)
        {            
            base.OnAfterRender(firstRender);
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
            var result = await Http.GetJson<List<SysMenuView>> (UrlConfig.GetMenuBar);
            if (result.Status != HttpStatusCode.OK) return new List<SysMenuView>();
            return result.Data;
        }


    }


}
