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

        protected override async Task OnInitializedAsync()
        {
            DataSource = await GetPowerMenus();
        }

        async Task<List<ViewPowerMenu>> GetPowerMenus()
        {
            var result = await Http.GetJson<List<ViewPowerMenu>>("Menu/GetLeftSideMenus");
            var viewPowerMenus = new List<ViewPowerMenu>();
            if (result.Status != 200) return viewPowerMenus;
            result.Data.OrderBy(u => u.Id);
            foreach (var item in result.Data)
            {
                if (item.UpLayerId == 0)
                {
                    viewPowerMenus.Add(item);
                }
                else
                {
                    result.Data.SingleOrDefault(u => u.Id == item.UpLayerId)?.Children.Add(item);
                }
            }
            return viewPowerMenus;
        }

    }
}
