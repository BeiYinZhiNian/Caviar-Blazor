using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control.Permission
{
    public partial class PermissionController:CaviarBaseController
    {
        [HttpPost]
        public IActionResult SetMenus(List<int> ids)
        {

            return ResultOK();
        }
    }
}
