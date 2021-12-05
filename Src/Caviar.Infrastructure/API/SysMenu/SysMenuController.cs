using Caviar.Core.Services.SysMenuServices;
using Caviar.SharedKernel;
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
        public override async Task<IActionResult> Index(int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true)
        {
            var entity = await Service.GetPages(null, pageIndex, pageSize, isOrder, isNoTracking);
            var entityVm = ToView(entity);
            entityVm.Rows = entityVm.Rows.ListToTree();
            return Ok(entityVm);
        }
    }
}
