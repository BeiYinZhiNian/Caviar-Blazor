using Caviar.Control.Menu;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control.Permission
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
            var menus = await _Action.GetRoleMenu(roleId);
            if (menus == null) return ResultForbidden("未查询到该角色权限，请联系管理员获取");
            menus.Data = menus.Data.ListToTree();
            return Ok(menus);
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
