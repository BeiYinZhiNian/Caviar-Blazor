using Caviar.Models.SystemData;
using Caviar.UI.Helper;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.UI.Pages.SystemPages.Menu
{
    public partial class Index
    {
        [Inject]
        HttpHelper Http { get; set; }

        public List<ViewPowerMenu> DataSource { get; set; }

        List<ViewModelHeader> ViewModelHeaders { get; set; }

        protected override async Task OnInitializedAsync()
        {
            DataSource = await GetPowerMenus();
            ViewModelHeaders = await GetViewModelNames();
        }

        async Task<List<ViewPowerMenu>> GetPowerMenus()
        {
            var result = await Http.GetJson<List<ViewPowerMenu>>("Menu/GetLeftSideMenus");
            if (result.Status != 200) return new List<ViewPowerMenu>();
            return result.Data;
        }

        async Task<List<ViewModelHeader>> GetViewModelNames()
        {
            var modelNameList = await Http.GetJson<List<ViewModelHeader>>("Base/GetModelHeader?name=SyspowerMenu");
            if (modelNameList.Status == 200)
            {
                var item = modelNameList.Data.SingleOrDefault(u => u.TypeName.ToLower() == "icon");
                if (item != null) item.ModelType = "icon";
                return modelNameList.Data;
            }
            return new List<ViewModelHeader>();
        }

    }
}
