using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caviar.Models;

namespace Caviar.Core.Role
{
    public partial class RoleAction
    {
        /// <summary>
        /// 获取指定用户的所有角色
        /// </summary>
        /// <returns></returns>
        public async Task<ResultMsg<List<SysRole>>> GetUserRoles(int userId,int? userGroupId = null)
        {
            List<SysRole> roles = new List<SysRole>();
            if (userId > 0)
            {
                //获取当前用户角色
                var userRoles = await BC.DbContext.GetEntityAsync<SysRoleUser>(u => u.UserId == userId);
                foreach (var item in userRoles)
                {
                    if (roles.SingleOrDefault(u=>u.Id == item.RoleId) != null) continue;
                    var role = await BC.DbContext.GetSingleEntityAsync<SysRole>(u => u.Id == item.RoleId);
                    await AddRole(roles, role);
                }
                if (userGroupId != null && userGroupId>0)
                {
                    var userGroupRoles = await BC.DbContext.GetEntityAsync<SysRoleUserGroup>(u => u.UserGroupId == userGroupId);
                    foreach (var item in userGroupRoles)
                    {
                        if (roles.SingleOrDefault(u => u.Id == item.RoleId) != null) continue;
                        var role = await BC.DbContext.GetSingleEntityAsync<SysRole>(u => u.Id == item.RoleId);
                        await AddRole(roles, role);
                    }
                }
            }
            if(roles.SingleOrDefault(u => u.Uid == CaviarConfig.NoLoginRoleGuid) == null)
            {
                //获取未登录角色
                var noRole = await BC.DbContext.GetEntityAsync<SysRole>(CaviarConfig.NoLoginRoleGuid);
                await AddRole(roles, noRole);
            }
            return Ok(roles.ToList());
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
                var userRole = await BC.DbContext.GetSingleEntityAsync<SysRole>(u => u.Id == role.ParentId);
                if (roles.SingleOrDefault(u => u.Id == userRole.Id) != null) return;
                await AddRole(roles, userRole);
            }
        }

        public override List<ViewRole> ToViewModel(List<SysRole> model)
        {
            model.AToB(out List<ViewRole> outModel);
            outModel = outModel.ListToTree();
            return outModel;
        }
    }
}
