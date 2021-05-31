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

        async Task GetMenus()
        {
            var result = await Http.GetJson<PageData<ViewMenu>>($"Menu/Index?pageSize=100");
            if (result.Status != 200) return;
            if (result.Data != null)
            {
                ViewMenus = result.Data.Rows;
            }
        }

        public Task<bool> Submit()
        {
            throw new NotImplementedException();
        }
    }
}
