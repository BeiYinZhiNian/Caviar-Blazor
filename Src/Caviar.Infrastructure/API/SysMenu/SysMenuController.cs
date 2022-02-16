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


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _menuServices.PermissionUrls = PermissionUrls;
            base.OnActionExecuting(context);
        }

        [HttpGet]
        public async Task<IActionResult> GetMenuBar()
        {
            var menus = await _menuServices.GetMenuBar();
            return Ok(menus);
        }

        [HttpGet]
        public override async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, bool isOrder = true, bool isNoTracking = true)
        {
            var entity = await Service.GetPageAsync(null, pageIndex, pageSize, isOrder, isNoTracking);
            entity.Rows = entity.Rows.ListToTree();
            return Ok(entity);
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
            await _menuServices.DeleteEntityAsync(vm.Entity);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetMenus(string url, string splicing)
        {
            var controllerList = splicing?.Split("|");
            var apiList = await _menuServices.GetApiList(url, controllerList);
            return Ok(apiList);
        }

    }
}
