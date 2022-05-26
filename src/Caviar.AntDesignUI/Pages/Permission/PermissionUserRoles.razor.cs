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
    public partial class PermissionUserRoles : ITableTemplate
    {
        [Parameter]
        public string CurrentUrl { get; set; }
        [Inject]
        HttpService HttpService { get; set; }
        [Inject]
        MessageService MessageService { get; set; }
        IEnumerable<ApplicationRoleView> RoleSelectedRows { get; set; }
        protected override async Task OnInitializedAsync()
        {
            IndexUrl = UrlConfig.RoleIndex;
            TableOptions.IsSelectedRows = true;
            await base.OnInitializedAsync();
            StateHasChanged();
        }



        /// <summary>
        /// 初始角色选择
        /// </summary>
        protected async Task<List<ApplicationRoleView>> RoleSelectedRowdInit(List<ApplicationRoleView> source)
        {
            var currentRoles = await GetUserRoles(DataSource.Entity.UserName);
            var rows = new List<ApplicationRoleView>();
            foreach (var item in source)
            {
                if (currentRoles.Contains(item.Entity.Name))
                {
                    rows.Add(item);
                }
            }
            return rows;
        }

        protected async Task<List<string>> GetUserRoles(string userName)
        {
            var result = await HttpService.GetJson<List<string>>($"{UrlConfig.GetUserRoles}?userName={userName}");
            if (result.Status == HttpStatusCode.OK)
            {
                return result.Data;
            }
            return null;
        }


        protected override async Task<List<ApplicationRoleView>> GetPages(int pageIndex = 1, int pageSize = 10, bool isOrder = true)
        {
            var pages = await base.GetPages(pageIndex, pageSize, isOrder);
            RoleSelectedRows = await RoleSelectedRowdInit(pages);
            return pages;
        }



        [Parameter]
        public ApplicationUserView DataSource { get; set; }

        public async Task<bool> Validate()
        {
            return await FormSubmit(DataSource.Entity.UserName);
        }

        /// <summary>
        /// 开始表单提交
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> FormSubmit(string userName)
        {
            var data = RoleSelectedRows.Select(u => u.Entity.Name);
            var result = await HttpService.PostJson($"{UrlConfig.AssignRoles}?userName={userName}", data);
            if (result.Status == HttpStatusCode.OK)
            {
                _ = MessageService.Success(result.Title);
                return true;
            }
            return false;
        }
    }
}
