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
        private readonly SysMenuServices _sysMenuServices;

        public SysMenuController(SysMenuServices sysMenuServices)
        {
            _sysMenuServices = sysMenuServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetMenuBar()
        {
            var menus = await _sysMenuServices.GetMenuBar();
            return Ok(menus);
        }

        [HttpGet]
        public override async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, bool isOrder = true)
        {
            var pages = await _sysMenuServices.GetPageAsync(null, pageIndex, pageSize, isOrder);
            return Ok(pages);
        }

        [HttpPost]
        public override async Task<IActionResult> DeleteEntity(SysMenuView vm)
        {
            if (vm.IsDeleteAll)
            {
                await _sysMenuServices.DeleteEntityAll(vm);
            }
            else
            {
                await _sysMenuServices.DeleteEntityAsync(vm.Entity);
            }
            return Ok();
        }

        /// <summary>
        /// 获取指定页面的其他
        /// </summary>
        /// <param name="IndexUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetApis(string indexUrl)
        {
            var apiList = await _sysMenuServices.GetApis(indexUrl);
            return Ok(apiList);
        }

        [HttpPost]
        public override async Task<IActionResult> CreateEntity(SysMenuView vm)
        {
            await _sysMenuServices.AddEntityAsync(vm.Entity);
            return Ok();
        }


    }
}
