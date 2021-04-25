using AntDesign;
using Caviar.Models.SystemData;
using Caviar.UI.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.UI.Pages.SystemPages.Menu
{
    public partial class Add
    {
        private Form<SysPowerMenu> _meunForm;
        public SysPowerMenu model = new SysPowerMenu();

        [Inject]
        HttpHelper Http { get; set; }


        protected override async Task OnInitializedAsync()
        {
            SysPowerMenus = await GetPowerMenus();
        }

        private List<ViewPowerMenu> SysPowerMenus;
        string UpPowerMenuName { get; set; } = "无上层目录";

        async Task<List<ViewPowerMenu>> GetPowerMenus()
        {
            var result = await Http.GetJson<List<ViewPowerMenu>>("Menu/GetCatalogMenus");
            if (result.Status != 200) return new List<ViewPowerMenu>();
            return result.Data;
        }

        [Inject]
        MessageService _message { get; set; }


        public async Task<bool> Submit()
        {
            if(_meunForm.Validate())
            {
                return await FormSubmit();
            }
            return false;
        }

        async Task<bool> FormSubmit()
        {
            var result = await Http.PostJson<SysPowerMenu, object>("Menu/AddEntity", model);
            if (result.Status == 200)
            {
                _message.Success("创建成功");
                return true;
            }
            return false;
        }

        void EventRecord(TreeEventArgs<ViewPowerMenu> args)
        {
            UpPowerMenuName = args.Node.Title;
            model.UpLayerId = int.Parse(args.Node.Key);
        }

        void RemoveRecord()
        {
            UpPowerMenuName = "无上层目录";
            model.UpLayerId = 0;
        }
    }
}
