using Caviar.Models.SystemData;
using Caviar.UI.Helper;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.UI.Pages.SystemPages.Menu
{
    public partial class Add
    {
        public SysPowerMenu model = new SysPowerMenu();

        [Inject]
        HttpHelper Http { get; set; }

        protected override async Task OnInitializedAsync()
        {
            SysPowerMenus = await GetPowerMenus();
        }

        private List<ViewPowerMenu> SysPowerMenus;

        async Task<List<ViewPowerMenu>> GetPowerMenus()
        {
            var result = await Http.GetJson<List<ViewPowerMenu>>("Menu/GetCatalogMenus");
            if (result.Status != 200) return new List<ViewPowerMenu>();
            return result.Data;
        }
    }
}
