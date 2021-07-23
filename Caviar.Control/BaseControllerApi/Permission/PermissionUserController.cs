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
        /// <summary>
        /// 获取可设置的数据权限
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetPermissionGroup()
        {
            var result = await _Action.GetPermissionGroup();
            return Ok(result);
        }

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
