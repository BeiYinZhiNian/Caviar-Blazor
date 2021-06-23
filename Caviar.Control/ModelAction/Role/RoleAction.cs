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
                    if (roles.SingleOrDefault(u=>u.Id == item.RoleId) != null) continue;
                    var role = await BC.DC.GetSingleEntityAsync<SysRole>(u => u.Id == item.RoleId);
                    await AddRole(roles, role);
                }
                if (BC.UserData.UserGroup != null)
                {
                    var userGroupRoles = await BC.DC.GetEntityAsync<SysRoleUserGroup>(u => u.UserGroupId == BC.UserData.UserGroup.Id);
                    foreach (var item in userGroupRoles)
                    {
                        if (roles.SingleOrDefault(u => u.Id == item.RoleId) != null) continue;
                        var role = await BC.DC.GetSingleEntityAsync<SysRole>(u => u.Id == item.RoleId);
                        await AddRole(roles, role);
                    }
                }
            }
            if(roles.SingleOrDefault(u => u.Uid == CaviarConfig.NoLoginRoleGuid) == null)
            {
                //获取未登录角色
                var noRole = await BC.DC.GetEntityAsync<SysRole>(CaviarConfig.NoLoginRoleGuid);
                await AddRole(roles, noRole);
            }
            return roles.ToList();
        }
        /// <summary>
        /// 加入当前角色和所有父角色
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        private async Task AddRole(List<SysRole> roles, SysRole role)
        {
            if (roles == null) return;
            roles.Add(role);
            if (role.ParentId > 0)
            {
                var userRole = await BC.DC.GetSingleEntityAsync<SysRole>(u => u.Id == role.ParentId);
                if (roles.SingleOrDefault(u => u.Id == userRole.Id) != null) return;
                await AddRole(roles, userRole);
            }
        }

        public override List<ViewRole> ModelToViewModel(List<SysRole> model)
        {
            model.AToB(out List<ViewRole> outModel);
            outModel = outModel.ListToTree();
            return outModel;
        }
    }
}
