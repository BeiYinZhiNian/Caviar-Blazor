using Caviar.Core.Menu;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Permission
{
    /// <summary>
    /// 菜单权限
    /// </summary>
    public partial class PermissionController
    {
        
        /// <summary>
        /// 获取角色菜单
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> RoleMenu(int roleId)
        {
            var result = await _Action.GetRoleMenu(roleId);
            return Ok(result);
        }
        /// <summary>
        /// 设置角色菜单
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="menuIds"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RoleMenu(int roleId,int[] menuIds)
        {
            var result = await _Action.SetRoleMenu(roleId, menuIds);
            return Ok(result);
        }

    }
}
