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
    public partial class PermissionMenus: ITableTemplate
    {


        [Parameter]
        public ApplicationRoleView DataSource { get; set; }
        List<SysMenuView> Menus { get; set; }
        protected override async Task OnInitializedAsync()
        {
            ControllerList.Add(CurrencyConstant.PermissionKey);
            await base.OnInitializedAsync();
            await GetSelectMenus();//获取已选择数据
        }

        async Task GetSelectMenus()
        {
            var result = await HttpService.GetJson<List<SysMenuView>>($"{Url[CurrencyConstant.GetMenusKey]}?roleName={DataSource.Entity.Name}");
            if (result.Status != HttpStatusCode.OK) return;
            if (result.Data != null)
            {
                Menus = result.Data;
            }
        }

        public async Task<bool> Validate()
        {
            List<SysMenuView> menus = new List<SysMenuView>();
            Menus.TreeToList(menus);
            var ids = menus.Where(u => u.IsPermission).Select(u => u.Id);
            var result = await HttpService.PostJson($"{Url}?roleName={DataSource.Entity.Name}", ids);
            if (result.Status != HttpStatusCode.OK) return false;
            _ = MessageService.Success("操作成功");
            return true;
        }
    }
}
