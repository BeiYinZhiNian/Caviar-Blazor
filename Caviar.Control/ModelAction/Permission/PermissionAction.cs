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
        public List<SysPermission> GetCurrentPermissions(List<SysRole> roles)
        {
            List<SysPermission> permissions = new List<SysPermission>();
            foreach (var item in roles)
            {
                var rolePermission = BC.DC.GetEntityAsync<SysRolePermission>(u => u.RoleId == item.Id).FirstOrDefault();
                if (rolePermission == null) continue;
                permissions.Add(rolePermission.Permission);
            }
            return permissions;
        }
    }
}
