using AntDesign;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caviar.SharedKernel.Entities.View;
using System.Net;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components;
using Caviar.AntDesignUI.Core;

namespace Caviar.AntDesignUI.Pages.Menu
{
    public partial class DataTemplate
    {
        [Inject]
        HttpService HttpService { get; set; }
        protected override async Task OnInitializedAsync()
        {
            ParentMenuName = LanguageService[$"{ CurrencyConstant.Page }.{ CurrencyConstant.NoUpperLevel}"];
            await base.OnInitializedAsync();
            _sysMenus = await GetMenus();
            _listMenus = TreeToList(_sysMenus);
            CheckMenuType();
        }

        private List<SysMenuView> _sysMenus = new List<SysMenuView>();

        private List<SysMenuView> _listMenus;
        

        async Task<List<SysMenuView>> GetMenus()
        {
            var result = await HttpService.GetJson<PageData<SysMenuView>>($"{UrlConfig.MenuIndex}?pageSize={Config.MaxPageSize}");
            if (result.Status != HttpStatusCode.OK) return null;
            
            return result.Data.Rows;
        }

        List<SysMenuView> TreeToList(List<SysMenuView> menus)
        {
            List<SysMenuView> listData = new List<SysMenuView>();
            menus.TreeToList(listData);
            if (DataSource.ParentId > 0)
            {
                var parent = listData.SingleOrDefault(u => u.Id == DataSource.ParentId);
                if (parent != null)
                {
                    ParentMenuName = parent.DisplayName;
                }
            }
            return listData;
        }

        string ParentMenuName { get; set; }
        void EventRecord(TreeEventArgs<SysMenuView> args)
        {
            ParentMenuName = args.Node.Title;
            DataSource.Entity.ParentId = int.Parse(args.Node.Key);
            var parent = _listMenus.SingleOrDefault(u => u.Id == DataSource.Entity.ParentId);
        }

        void RemoveRecord()
        {
            ParentMenuName = LanguageService[$"{ CurrencyConstant.Page }.{ CurrencyConstant.NoUpperLevel}"];
            DataSource.Entity.ParentId = 0;
        }
    }
}
