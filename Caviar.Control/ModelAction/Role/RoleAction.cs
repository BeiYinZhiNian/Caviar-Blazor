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
                    await AddRole(roles, item.Role);
                }
            }
            //获取未登录角色
            var noRole = await BC.DC.GetEntityAsync<SysRole>(CaviarConfig.NoLoginRoleGuid);
            await AddRole(roles,noRole);
            return roles;
        }

        private async Task AddRole(List<SysRole> roles, SysRole role)
        {
            if (roles == null) return;
            roles.Add(role);
            if (role.ParentId > 0)
            {
                var userRole = await BC.DC.GetFirstEntityAsync<SysRole>(u => u.Id == role.ParentId);
                await AddRole(roles, userRole);
            }
        }

        public override List<ViewRole> ModelToViewModel(List<SysRole> model)
        {
            var outModel = CommonHelper.AToB<List<ViewRole>, List<SysRole>>(model);
            var viewMenus = new List<ViewRole>().ListAutoAssign(outModel).ToList();
            viewMenus = viewMenus.ListToTree();
            return viewMenus;
        }
    }
}
