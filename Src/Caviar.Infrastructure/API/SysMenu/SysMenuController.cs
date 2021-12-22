using Caviar.Core.Services.SysMenuServices;
using Caviar.SharedKernel;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Infrastructure.API.SysMenuController
{
    public partial class SysMenuController
    {
        SysMenuServices MenuServices = CreateService<SysMenuServices>();

        [HttpGet]
        public async Task<IActionResult> GetMenuBar()
        {
            var menus = await MenuServices.GetMenuBar();
            var menusVm = ToView(menus).ListToTree();
            return Ok(menusVm);
        }

        [HttpGet]
        public override async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, bool isOrder = true, bool isNoTracking = true)
        {
            var entity = await Service.GetPages(null, pageIndex, pageSize, isOrder, isNoTracking);
            var entityVm = ToView(entity);
            entityVm.Rows = entityVm.Rows.ListToTree();
            return Ok(entityVm);
        }

        protected override List<SysMenuView> ToView(List<SysMenu> entity)
        {
            var vm = base.ToView(entity);
            foreach (var item in vm)
            {
                string key = $"{CurrencyConstant.MenuBar}.{item.Entity.Key}";
                item.DisplayName = LanguageService[key];//翻译显示名称
            }
            return vm;
        }

        [HttpGet]
        public async Task<IActionResult> GetApiList(string url,string splicing)
        {
            var controllerList = splicing?.Split("|");
            var apiList = await MenuServices.GetApiList(url, controllerList);
            var Vm = ToView(apiList);
            return Ok(Vm);
        }

        [HttpPost]
        public override async Task<IActionResult> DeleteEntity(SysMenuView vm)
        {
            if (vm.IsDeleteAll)
            {
                List<SysMenuView> menuViews = new List<SysMenuView>();
                vm.TreeToList(menuViews);
                var menus = ToEntity(menuViews);
                await MenuServices.DeleteEntityAll(menus);
            }
            await MenuServices.DeleteEntity(vm.Entity);
            return Ok();
        }
    }
}
