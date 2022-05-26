// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;

namespace Caviar.AntDesignUI.Pages.Permission
{
    public partial class PermissionMenus : ITableTemplate
    {
        [Parameter]
        public string CurrentUrl { get; set; }

        [Parameter]
        public ApplicationRoleView DataSource { get; set; }
        [Inject]
        UserConfig UserConfig { get; set; }
        [Inject]
        ILanguageService LanguageService { get; set; }
        [Inject]
        HttpService HttpService { get; set; }
        [Inject]
        MessageService MessageService { get; set; }
        List<SysMenuView> Menus { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await GetSelectMenus();//获取已选择数据
        }

        async Task GetSelectMenus()
        {
            var result = await HttpService.GetJson<List<SysMenuView>>($"{UrlConfig.GetPermissionMenus}?roleName={DataSource.Entity.Name}");
            if (result.Status != HttpStatusCode.OK) return;
            if (result.Data != null)
            {
                Menus = result.Data;
            }
        }

        private void CheckedChanged(bool check, SysMenuView menu)
        {
            List<SysMenuView> menus = new List<SysMenuView>();
            Menus.TreeToList(menus);
            if (string.IsNullOrEmpty(menu.Entity.Url))
            {
                menus = menus.Where(u => u.Entity.Id == menu.Entity.Id).ToList();
            }
            else
            {
                menus = menus.Where(u => u.Entity.Url == menu.Entity.Url).ToList();
            }
            foreach (var item in menus)
            {
                item.IsPermission = check;
            }
        }

        public async Task<bool> Validate()
        {
            List<SysMenuView> menus = new List<SysMenuView>();
            Menus.TreeToList(menus);
            var urls = menus.Where(u => u.IsPermission && !string.IsNullOrEmpty(u.Entity.Url)).Select(u => u.Entity.Url).ToList();
            // 当权限菜单没有url时，以id为权限
            urls.AddRange(menus.Where(u => u.IsPermission && string.IsNullOrEmpty(u.Entity.Url)).Select(u => u.Entity.Id.ToString()));
            var result = await HttpService.PostJson($"{UrlConfig.SavePermissionMenus}?roleName={DataSource.Entity.Name}", urls);
            if (result.Status != HttpStatusCode.OK) return false;
            _ = MessageService.Success(result.Title);
            UserConfig.RefreshMenuAction.Invoke();//更新菜单
            return true;
        }
    }
}
