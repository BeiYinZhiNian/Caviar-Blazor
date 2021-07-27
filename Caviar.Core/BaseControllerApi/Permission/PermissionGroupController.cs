using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Permission
{
    /// <summary>
    /// 用户组权限
    /// </summary>
    public partial class PermissionController
    {
        [HttpGet]
        public async Task<IActionResult> RoleUserGroup(int PermissionId)
        {
            var result = await _Action.GetRoleUserGropu(PermissionId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> RoleUserGroup(int PermissionId,int[] roleIds)
        {
            var result = await _Action.SetRoleUserGropu(PermissionId,roleIds);
            return Ok(result);
        }
    }
}
