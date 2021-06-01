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
    public partial class DataTemplate: ITableTemplate
    {
        partial void PratialOnInitializedAsync(ref bool IsContinue)
        {
            GetMenus();
            CheckMenuType();
        }

        private List<ViewMenu> SysMenus = new List<ViewMenu>();
        string ParentMenuName { get; set; } = "无上层目录";

        async void GetMenus()
        {
            var result = await Http.GetJson<List<ViewMenu>>("Menu/GetLeftSideMenus");
            if (result.Status != 200) return;
            if (DataSource.ParentId > 0)
            {
                List<ViewMenu> listData = new List<ViewMenu>();
                result.Data.TreeToList(listData);
                var parent = listData.SingleOrDefault(u => u.Id == DataSource.ParentId);
                if (parent != null)
                {
                    ParentMenuName = parent.MenuName;
                }
            }
            SysMenus = result.Data;
            StateHasChanged();
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
