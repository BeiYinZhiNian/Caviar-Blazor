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
        public void CheckInit(List<TreeNode<ViewRole>> nodes, List<ViewRole> menus)
        {
            foreach (var item in nodes)
            {
                var menu = menus.FirstOrDefault(u => u.Id.ToString() == item.Key);
                if (menu != null)
                {
                    if (!menu.IsDisable)
                    {
                        item.SetChecked(true);
                    }
                }
                if (item.ChildNodes != null)
                {
                    CheckInit(item.ChildNodes, menus);
                }
            }
        }
        Tree<ViewRole> Tree { get; set; }

        async Task GetSelectMenus()
        {
            var result = await Http.GetJson<List<ViewRole>>($"{Url}?PermissionId={DataSource.Id}");
            if (result.Status != 200) return;
            if (result.Data != null)
            {
                ViewRoles = result.Data.ListToTree();
                StateHasChanged();
                CheckInit(Tree.ChildNodes, result.Data);
            }
        }

        public async Task<bool> Submit()
        {
            List<ViewRole> roles = new List<ViewRole>();
            ViewRoles.TreeToList(roles);
            var ids = roles.Where(u => u.IsPermission).Select(u => u.Id);
            var result = await Http.PostJson($"{Url}?PermissionId={DataSource.Id}", ids);
            if (result.Status != 200) return false;
            MessageService.Success("操作成功");
            return true;
        }
    }
}
