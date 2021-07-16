using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control.Permission
{
    public partial class PermissionController
    {
        [HttpGet]
        public async Task<IActionResult> RoleUser(int PermissionId)
        {
            var result = await _Action.GetRoleUser(PermissionId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> RoleUser(int PermissionId, int[] roleIds)
        {
            var result = await _Action.SetRoleUser(PermissionId, roleIds);
            return Ok(result);
        }
    }
}
