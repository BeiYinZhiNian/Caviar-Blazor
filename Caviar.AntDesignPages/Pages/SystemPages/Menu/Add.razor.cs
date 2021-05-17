using AntDesign;
using Caviar.Models.SystemData;
using Caviar.AntDesignPages.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Pages.SystemPages.Menu
{
    [DisplayName("添加菜单")]
    public partial class Add: ITableTemplate
    {
        private Form<SysPowerMenu> _meunForm;
        public SysPowerMenu model = new SysPowerMenu() { Number = "999" };

        [Inject]
        HttpHelper Http { get; set; }


        protected override async Task OnInitializedAsync()
        {
            SysPowerMenus = await GetPowerMenus();
        }

        private List<ViewMenu> SysPowerMenus;
        string UpPowerMenuName { get; set; } = "无上层目录";

        async Task<List<ViewMenu>> GetPowerMenus()
        {
            var result = await Http.GetJson<List<ViewMenu>>("Menu/GetLeftSideMenus");
            if (result.Status != 200) return new List<ViewMenu>();
            return result.Data;
        }

        [Inject]
        MessageService _message { get; set; }
        [Parameter]
        public bool Visible { get; set; }

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
            var result = await Http.PostJson<SysPowerMenu, object>("Menu/Add", model);
            if (result.Status == 200)
            {
                _message.Success("创建成功");
                return true;
            }
            return false;
        }

        void EventRecord(TreeEventArgs<ViewMenu> args)
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
