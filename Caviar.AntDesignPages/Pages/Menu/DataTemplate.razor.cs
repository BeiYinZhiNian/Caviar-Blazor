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

namespace Caviar.AntDesignPages.Pages.Menu
{
    [DisplayName("数据模板")]
    public partial class DataTemplate: ITableTemplate
    {
        
        [Parameter]
        public SysPowerMenu DataSource { get; set; }
        
        [Parameter]
        public string Url { get; set; }

        [Parameter]
        public string SuccMsg { get; set; } = "操作成功";

        [Inject]
        HttpHelper Http { get; set; }

        private Form<SysPowerMenu> _meunForm;
        protected override async Task OnInitializedAsync()
        {
            SysPowerMenus = await GetPowerMenus();
            CheckMenuType();
        }

        private List<ViewMenu> SysPowerMenus;
        string UpPowerMenuName { get; set; } = "无上层目录";

        async Task<List<ViewMenu>> GetPowerMenus()
        {
            var result = await Http.GetJson<List<ViewMenu>>("Menu/GetLeftSideMenus");
            if (result.Status != 200) return new List<ViewMenu>();
            if (DataSource.ParentId > 0)
            {
                List<ViewMenu> listData = new List<ViewMenu>();
                result.Data.TreeToList(listData);
                var parent = listData.SingleOrDefault(u => u.Id == DataSource.ParentId);
                if (parent != null)
                {
                    UpPowerMenuName = parent.MenuName;
                }
            }
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
            var result = await Http.PostJson<SysPowerMenu, object>(Url, DataSource);
            if (result.Status == 200)
            {
                _message.Success(SuccMsg);
                return true;
            }
            return false;
        }

        void EventRecord(TreeEventArgs<ViewMenu> args)
        {
            UpPowerMenuName = args.Node.Title;
            DataSource.ParentId = int.Parse(args.Node.Key);
        }

        void RemoveRecord()
        {
            UpPowerMenuName = "无上层目录";
            DataSource.ParentId = 0;
        }
    }
}
