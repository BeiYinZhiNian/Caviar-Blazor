using Caviar.Core.Services.SysMenuServices;
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

        public async Task<IActionResult> GetMenuBar()
        {
            var menus = await MenuServices.GetMenuBar();
            var menusVm = ToView(menus);
            return Ok(menusVm);
        }
    }
}
