using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control.Permission
{
    /// <summary>
    /// 用户组权限
    /// </summary>
    public partial class PermissionController
    {
        [HttpGet]
        public async Task<IActionResult> RoleUserGroup(int PermissionId)
        {
            var Roles = await _Action.GetRoleUserGropu(PermissionId);
            ResultMsg.Data = Roles;
            return ResultOK();
        }

        [HttpPost]
        public async Task<IActionResult> RoleUserGroup(int PermissionId,int[] roleIds)
        {
            await _Action.SetRoleUserGropu(PermissionId,roleIds);
            return ResultOK();
        }
    }
}
