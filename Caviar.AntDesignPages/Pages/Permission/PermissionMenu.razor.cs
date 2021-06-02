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
        NavigationManager NavigationManager { get; set; }
        [Parameter]
        public ViewRole DataSource { get; set; }
        [Parameter]
        public string Url { get; set; }
        List<ViewMenu> ViewMenus { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await GetMenus();//获取数据源
        }

        Tree<ViewMenu> Tree { get; set; }
        

        async Task GetMenus()
        {
            var result = await Http.GetJson<PageData<ViewMenu>>($"Menu/Index?pageSize=100");
            if (result.Status != 200) return;
            if (result.Data != null)
            {
                ViewMenus = result.Data.Rows;
            }
        }

        public async Task<bool> Submit()
        {
            var ids = Tree.CheckedKeys;
            var result = await Http.PostJson(Url, ids);
            throw new NotImplementedException();
        }
    }
}
