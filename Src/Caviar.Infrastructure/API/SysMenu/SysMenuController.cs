using Caviar.Core.Services;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Caviar.Infrastructure.API.SysMenuController
{
    public partial class SysMenuController
    {
        SysMenuServices _menuServices;

        public SysMenuController(SysMenuServices sysMenuServices)
        {
            _menuServices = sysMenuServices;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await base.OnActionExecutionAsync(context, next);
            var menuPermission = await UserServices.GetPermissions(u => u.PermissionType == PermissionType.RoleMenus);
            _menuServices.MenuPermissions = menuPermission;
        }

        [HttpGet]
        public async Task<IActionResult> GetMenuBar()
        {
            var menus = await _menuServices.GetMenuBar();
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
                string key = $"{CurrencyConstant.Menu}.{item.Entity.Key}";
                item.DisplayName = LanguageService[key];//翻译显示名称
            }
            return vm;
        }


        [HttpPost]
        public override async Task<IActionResult> DeleteEntity(SysMenuView vm)
        {
            if (vm.IsDeleteAll)
            {
                List<SysMenuView> menuViews = new List<SysMenuView>();
                vm.TreeToList(menuViews);
                var menus = ToEntity(menuViews);
                await _menuServices.DeleteEntityAll(menus);
            }
            await _menuServices.DeleteEntity(vm.Entity);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetMenus(string url, string splicing)
        {
            var controllerList = splicing?.Split("|");
            var apiList = await _menuServices.GetApiList(url, controllerList);
            var Vm = ToView(apiList);
            return Ok(Vm);
        }

    }
}
