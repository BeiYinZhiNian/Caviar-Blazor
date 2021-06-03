using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control.Permission
{
    public partial class PermissionAction
    {

        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<SysPermission>> GetCurrentPermissions(List<SysRole> roles,bool isAdmin = false)
        {
            List<SysPermission> permissions = new List<SysPermission>();
            if (isAdmin)
            {
                return await BC.DC.GetAllAsync<SysPermission>();
            }
            else
            {
                foreach (var item in roles)
                {
                    var rolePermission = await BC.DC.GetFirstEntityAsync<SysRolePermission>(u => u.RoleId == item.Id);
                    if (rolePermission == null) continue;
                    permissions.Add(rolePermission.Permission);
                }
            }
            return permissions;
        }
    }
}
