using Caviar.AntDesignUI.Helper;
using Caviar.SharedKernel.View;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntDesign;
using Microsoft.AspNetCore.Http;
using Caviar.SharedKernel;
using Caviar.SharedKernel.Entities.View;

namespace Caviar.AntDesignUI.Pages.Permission
{
    public partial class PermissionMenu: ITableTemplate
    {
        [Inject]
        HttpHelper Http { get; set; }
        [Inject]
        MessageService MessageService { get; set; }
        [Parameter]
        public ViewRole DataSource { get; set; }
        [Parameter]
        public string Url { get; set; }
        List<SysMenuView> ViewMenus { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await GetSelectMenus();//获取已选择数据
        }

        async Task GetSelectMenus()
        {
            var result = await Http.GetJson<List<SysMenuView>>($"{Url}?roleId={DataSource.Id}");
            if (result.Status != StatusCodes.Status200OK) return;
            if (result.Data != null)
            {
                ViewMenus = result.Data;
                StateHasChanged();
            }
        }

        public async Task<bool> Validate()
        {
            List<SysMenuView> menus = new List<SysMenuView>();
            ViewMenus.TreeToList(menus);
            var ids = menus.Where(u => u.IsPermission).Select(u => u.Id);
            var result = await Http.PostJson($"{Url}?roleId={DataSource.Id}", ids);
            if (result.Status != StatusCodes.Status200OK) return false;
            MessageService.Success("操作成功");
            return true;
        }
    }
}
