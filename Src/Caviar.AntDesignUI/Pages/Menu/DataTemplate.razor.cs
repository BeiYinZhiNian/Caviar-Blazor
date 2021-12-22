using AntDesign;
using Caviar.SharedKernel.View;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Caviar.SharedKernel;
using Caviar.SharedKernel.Entities.View;

namespace Caviar.AntDesignUI.Pages.Menu
{
    public partial class DataTemplate
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await GetMenus();
            CheckMenuType();
        }

        private List<SysMenuView> SysMenus = new List<SysMenuView>();
        string ParentMenuName { get; set; } = "无上层目录";

        async Task GetMenus()
        {
            
            var result = await HttpService.GetJson<PageData<SysMenuView>>($"{Url["SysMenu"]}?pageSize=100");
            if (result.Status != StatusCodes.Status200OK) return;
            if (DataSource.ParentId > 0)
            {
                List<SysMenuView> listData = new List<SysMenuView>();
                result.Data.Rows.TreeToList(listData);
                var parent = listData.SingleOrDefault(u => u.Id == DataSource.ParentId);
                if (parent != null)
                {
                    ParentMenuName = parent.Entity.Key;
                }
            }
            SysMenus = result.Data.Rows;
        }

        

        void EventRecord(TreeEventArgs<SysMenuView> args)
        {
            ParentMenuName = args.Node.Title;
            DataSource.Entity.ParentId = int.Parse(args.Node.Key);
            DataSource.Entity.ControllerName = args.Node.Key;
        }

        void RemoveRecord()
        {
            ParentMenuName = "无上层目录";
            DataSource.Entity.ParentId = 0;
        }
    }
}
