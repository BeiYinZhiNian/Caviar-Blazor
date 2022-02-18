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
        SysMenuServices _sysMenuServices;

        public SysMenuController(SysMenuServices sysMenuServices)
        {
            _sysMenuServices = sysMenuServices;
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _sysMenuServices.PermissionUrls = PermissionUrls;
            base.OnActionExecuting(context);
        }

        [HttpGet]
        public async Task<IActionResult> GetMenuBar()
        {
            var menus = await _sysMenuServices.GetMenuBar(PermissionUrls);
            return Ok(menus);
        }

        [HttpGet]
        public override async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, bool isOrder = true, bool isNoTracking = true)
        {
            var pages = await _sysMenuServices.GetPageAsync(null, pageIndex, pageSize, isOrder, isNoTracking);
            return Ok(pages);
        }




        [HttpPost]
        public override async Task<IActionResult> DeleteEntity(SysMenuView vm)
        {
            if (vm.IsDeleteAll)
            {
                await _sysMenuServices.DeleteEntityAll(vm);
            }
            await _sysMenuServices.DeleteEntityAsync(vm.Entity);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetMenus(string controllerName, string splicing)
        {
            var controllerList = splicing?.Split("|");
            var apiList = await _sysMenuServices.GetMenus(controllerName, controllerList);
            return Ok(apiList);
        }

        

    }
}
