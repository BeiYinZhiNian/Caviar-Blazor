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
    public partial class PermissionMenu
    {
        [Inject]
        HttpHelper Http { get; set; }
        List<ViewMenu> DataSource { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await GetMenus();//获取数据源
        }

        async Task GetMenus()
        {
            var result = await Http.GetJson<PageData<ViewMenu>>("Menu/GetPages?pageSize=100");
            if (result.Status != 200) return;
            if (result.Data != null)
            {
                DataSource = result.Data.Rows;
            }
        }
    }
}
