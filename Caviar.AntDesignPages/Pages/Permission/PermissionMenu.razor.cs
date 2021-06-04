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
            await GetMenus();//获取数据源
            
        }
        public void CheckInit(List<TreeNode<ViewMenu>> nodes,List<ViewMenu> menus)
        {
            foreach (var item in nodes)
            {
                if(menus.FirstOrDefault(u=>u.Id.ToString() == item.Key)!=null)
                {
                    item.SetChecked(true);
                }
                if (item.ChildNodes != null)
                {
                    CheckInit(item.ChildNodes, menus);
                }
            }
        }
        Tree<ViewMenu> Tree { get; set; }

        async Task GetMenus()
        {
            var result = await Http.GetJson<PageData<ViewMenu>>($"Menu/Index?pageSize=100");
            if (result.Status != 200) return;
            if (result.Data != null)
            {
                ViewMenus = result.Data.Rows;
                StateHasChanged();
                await GetSelectMenus();//获取已选择数据
            }
        }

        async Task GetSelectMenus()
        {
            var result = await Http.GetJson<List<ViewMenu>>($"{Url}?roleId={DataSource.Id}");
            if (result.Status != 200) return;
            if (result.Data != null)
            {
                CheckInit(Tree.ChildNodes, result.Data);
            }
        }

        public async Task<bool> Submit()
        {
            var ids = Tree.CheckedKeys;
            var result = await Http.PostJson($"{Url}?roleId={DataSource.Id}", ids);
            if (result.Status != 200) return false;
            MessageService.Success("操作成功");
            return true;
        }
    }
}
