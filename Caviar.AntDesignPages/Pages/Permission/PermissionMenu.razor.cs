using Caviar.AntDesignPages.Helper;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntDesign;
namespace Caviar.AntDesignPages.Pages.Permission
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
        List<ViewMenu> ViewMenus { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await GetSelectMenus();//获取已选择数据
        }

        async Task GetSelectMenus()
        {
            var result = await Http.GetJson<List<ViewMenu>>($"{Url}?roleId={DataSource.Id}");
            if (result.Status != 200) return;
            if (result.Data != null)
            {
                ViewMenus = result.Data.ListToTree();
                StateHasChanged();
            }
        }

        public async Task<bool> Submit()
        {
            List<ViewMenu> menus = new List<ViewMenu>();
            ViewMenus.TreeToList(menus);
            var ids = menus.Where(u => u.IsPermission).Select(u => u.Id);
            var result = await Http.PostJson($"{Url}?roleId={DataSource.Id}", ids);
            if (result.Status != 200) return false;
            MessageService.Success("操作成功");
            return true;
        }
    }
}
