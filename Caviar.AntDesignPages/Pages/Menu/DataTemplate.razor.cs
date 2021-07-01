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
    public partial class DataTemplate
    {
        protected override async Task OnInitializedAsync()
        {
            await GetMenus();
            CheckMenuType();
            await base.OnInitializedAsync();
        }

        private List<ViewMenu> SysMenus = new List<ViewMenu>();
        string ParentMenuName { get; set; } = "无上层目录";

        async Task GetMenus()
        {
            var result = await Http.GetJson<PageData<ViewMenu>>("Menu/Index?pageSize=100");
            if (result.Status != 200) return;
            if (DataSource.ParentId > 0)
            {
                List<ViewMenu> listData = new List<ViewMenu>();
                result.Data.Rows.TreeToList(listData);
                var parent = listData.SingleOrDefault(u => u.Id == DataSource.ParentId);
                if (parent != null)
                {
                    ParentMenuName = parent.MenuName;
                }
            }
            SysMenus = result.Data.Rows;
        }

        

        void EventRecord(TreeEventArgs<ViewMenu> args)
        {
            ParentMenuName = args.Node.Title;
            DataSource.ParentId = int.Parse(args.Node.Key);
        }

        void RemoveRecord()
        {
            ParentMenuName = "无上层目录";
            DataSource.ParentId = 0;
        }
    }
}
