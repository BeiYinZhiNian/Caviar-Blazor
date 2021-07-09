using AntDesign;
using Caviar.AntDesignPages.Helper;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Pages.Permission
{
    public partial class PermissionRoleTemple<TData> where TData:IBaseModel
    {
        [Inject]
        HttpHelper Http { get; set; }
        [Inject]
        MessageService MessageService { get; set; }
        [Parameter]
        public TData DataSource { get; set; }
        [Parameter]
        public string Url { get; set; }
        List<ViewRole> ViewRoles { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await GetSelectMenus();//获取已选择数据
        }

        async Task GetSelectMenus()
        {
            var result = await Http.GetJson<List<ViewRole>>($"{Url}?PermissionId={DataSource.Id}");
            if (result.Status != HttpState.OK) return;
            if (result.Data != null)
            {
                ViewRoles = result.Data.ListToTree();
                StateHasChanged();
            }
        }

        public async Task<bool> Submit()
        {
            List<ViewRole> roles = new List<ViewRole>();
            ViewRoles.TreeToList(roles);
            var ids = roles.Where(u => u.IsPermission).Select(u => u.Id);
            var result = await Http.PostJson($"{Url}?PermissionId={DataSource.Id}", ids);
            if (result.Status != HttpState.OK) return false;
            MessageService.Success("操作成功");
            return true;
        }
    }
}
