using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caviar.Models.SystemData;

namespace Caviar.Control.Role
{
    public partial class RoleAction
    {
        public async Task<List<SysRole>> GetCurrentRoles()
        {
            List<SysRole> roles = new List<SysRole>();
            if (BC.Id > 0)
            {
                //获取当前用户角色
                var userRoles = await BC.DC.GetEntityAsync<SysRoleLogin>(u => u.UserId == BC.Id);
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
