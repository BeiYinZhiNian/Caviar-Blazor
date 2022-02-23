using Caviar.AntDesignUI.Shared;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Pages.Permission
{
    public partial class PermissionUserRoles : ITableTemplate
    {
        IEnumerable<ApplicationRoleView> RoleSelectedRows {get;set;}
        protected override async Task OnInitializedAsync()
        {
            ControllerName = CurrencyConstant.ApplicationRoleKey;
            ControllerList.Add(CurrencyConstant.ApplicationUserKey);
            await base.OnInitializedAsync();
            StateHasChanged();
        }

        

        /// <summary>
        /// 初始角色选择
        /// </summary>
        protected async Task<List<ApplicationRoleView>> RoleSelectedRowdInit(List<ApplicationRoleView> source)
        {
            var currentRoles = await GetCurrentRoles();
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

        protected async Task<List<string>> GetCurrentRoles()
        {
            var result = await HttpService.GetJson<List<string>>(Url[CurrencyConstant.GetCurrentRolesKey]);
            if(result.Status == HttpStatusCode.OK)
            {
                return result.Data;
            }
            return null;
        }


        protected override async Task<List<ApplicationRoleView>> GetPages(int pageIndex = 1, int pageSize = 10, bool isOrder = true)
        {
            SubmitUrl = UrlConfig.RoleIndex;
            var pages = await base.GetPages(pageIndex, pageSize, isOrder);
            RoleSelectedRows = await RoleSelectedRowdInit(pages);
            return pages;
        }



        [Parameter]
        public ApplicationUserView DataSource { get; set; }

        public async Task<bool> Validate()
        {
            return await FormSubmit();
        }

        /// <summary>
        /// 开始表单提交
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> FormSubmit()
        {
            var data = RoleSelectedRows.Select(u => u.Entity.Name);
            var result = await HttpService.PostJson(Url[CurrencyConstant.AssignRolesKey], data);
            if (result.Status == HttpStatusCode.OK)
            {
                _ = MessageService.Success(result.Title);
                return true;
            }
            return false;
        }
    }
}
