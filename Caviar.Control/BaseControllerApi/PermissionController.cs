using Caviar.Control.Menu;
using Caviar.Models.SystemData;
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

        [HttpGet]
        public IActionResult RoleMenu(int roleId)
        {
            var menus = Action.GetRoleMenu(roleId);
            if (menus == null) return ResultForbidden("未查询到该角色权限，请联系管理员获取");
            ResultMsg.Data = menus;
            return ResultOK();
        }
        [HttpPost]
        public async Task<IActionResult> RoleMenu(int roleId,int[] menuIds)
        {
            var isSucc = await Action.SetRoleMenu(roleId, menuIds);
            if (isSucc) return ResultOK();
            return ResultError("操作失败");
        }

    }
}
