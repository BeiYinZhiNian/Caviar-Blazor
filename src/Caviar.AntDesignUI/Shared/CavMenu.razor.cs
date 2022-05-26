// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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



        /// <summary>
        /// 创建面包屑信息
        /// </summary>
        /// <param name="menuItem"></param>
        /// <returns></returns>
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
                    BreadcrumbItemArr = BreadcrumbItemArr,
                    Layout = JsonConvert.SerializeObject(Layout)
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
            if (!Config.IsServer && firstRender)
            {
                //在wasm中注册事件
                //wasm模式初始化完成，接收事件
                IframeMessage.SwitchWasm += SwitchWasm_RefChanged;
            }
        }
        /// <summary>
        /// 切换wasm模式，并保持相同状态
        /// </summary>
        /// <param name="message"></param>
        private void SwitchWasm_RefChanged(IframeMessage message)
        {
            OpenKeysNav = message.ExchangeData.OpenKeysNav ?? Array.Empty<string>();//打开nav
            SelectedKeys = message.ExchangeData.SelectedKeys;//选择key
            if (BreadcrumbItemArrChanged.HasDelegate)
            {
                //面包屑更新
                _ = BreadcrumbItemArrChanged.InvokeAsync(message.ExchangeData.BreadcrumbItemArr);
            }
            try
            {
                JsonConvert.PopulateObject(message.ExchangeData.Layout, Layout);
            }
            catch
            {
                //防止错误数据
            }
            NavigationManager.NavigateTo(message.Url);
            JSRuntime.InvokeVoidAsync(CurrencyConstant.SwitchWasm);
        }

        private List<SysMenuView> SysMenus;

        public async void Refresh()
        {
            await OnInitializedAsync();
            StateHasChanged();
            foreach (var item in AntDesignMenu.MenuItems)
            {
                if (item.Key == AntDesignMenu.SelectedKeys.FirstOrDefault())
                {
                    OnMenuItemClickedNav(item);
                }
            }

        }

        async Task<List<SysMenuView>> GetMenus()
        {
            var result = await Http.GetJson<List<SysMenuView>>(UrlConfig.GetMenuBar);
            if (result.Status != HttpStatusCode.OK) return new List<SysMenuView>();
            return result.Data;
        }


    }


}
