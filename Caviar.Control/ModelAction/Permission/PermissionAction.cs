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
        public async Task<List<SysRole>> GetCurrentPermissions()
        {
            List<SysRole> roles = new List<SysRole>();
            if (BC.Id > 0)
            {
                //获取当前用户角色
                var userRoles = BC.DC.GetEntityAsync<SysRoleLogin>(u => u.UserId == BC.Id);
                foreach (var item in userRoles)
                {
                    roles.Add(item.Role);
                }
            }
            //获取未登录角色
            var noRole = await BC.DC.GetEntityAsync<SysRole>(CaviarConfig.NoLoginRoleGuid);
            roles.Add(noRole);
            return roles;
        }
    }
}
