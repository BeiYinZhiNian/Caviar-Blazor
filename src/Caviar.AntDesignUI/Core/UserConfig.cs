// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Collections;
using System.Collections.Generic;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace Caviar.AntDesignUI.Core
{
    public class UserConfig
    {
        public CavLayout Layout { get; set; }

        public UserConfig(IJSRuntime jSRuntime, CavLayout layoutEntity)
        {
            JSRuntime = jSRuntime;
            Layout = layoutEntity;
            Layout.ThemeChanged += SetTheme;
        }
        /// <summary>
        /// 是否自动切换为wasm模式
        /// </summary>
        public bool IsAutomaticSwitchWasm { get; set; } = true;
        public IJSRuntime JSRuntime { get; set; }
        /// <summary>
        /// 路由
        /// </summary>
        public static Router Router;
        /// <summary>
        /// 更新菜单数据
        /// </summary>
        public Action RefreshMenuAction { get; set; }

        /// <summary>
        /// 更新当前页面数据
        /// </summary>
        public Action RefreshCurrentPage { get; set; }

        static IEnumerable _routes;
        public static IEnumerable Routes()
        {
            if (_routes == null)
            {
                var routes = Router.GetObjValue("Routes");
                _routes = (IEnumerable)routes.GetObjValue("Routes");
            }
            return _routes;
        }
        /// <summary>
        /// 是否为游客
        /// </summary>
        public bool IsTourist { get; set; }
        /// <summary>
        /// 更新布局
        /// </summary>
        public Action LayoutPage { get; set; }

        public List<SysMenuView> Menus { get; set; } = new List<SysMenuView>();

        public async void SetTheme(string oldThemeName, string newThemeName)
        {
            await JSRuntime.InvokeVoidAsync("loadCss", oldThemeName, newThemeName);
            switch (newThemeName)
            {
                case "ant-design-blazor.dark.css":
                    Layout.White = "#2E2E2E";
                    break;
                default:
                    Layout.White = "#F8F8FF";
                    break;
            }
            Layout.Background = $"background:{Layout.White};";
            Layout.ContentStyle = $"margin: 6px 16px;padding: 24px;min-height: 280px;{Layout.Background}";
            Layout.HeaderStyle = $"padding:0;{Layout.Background}";
            LayoutPage?.Invoke();
        }
    }
}
