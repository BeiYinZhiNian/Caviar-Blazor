using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Permission
{
    public partial class PermissionAction
    {
        /// <summary>
        /// 获取用户角色
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        public async Task<ResultMsg<List<ViewRole>>> GetRoleUser(int userId)
        {
            List<SysRole> SelectUrg = new List<SysRole>();
            List<SysRole> haveSysRoles = new List<SysRole>();
            var roleUserGroups = await BC.DbContext.GetEntityAsync<SysRoleUser>(u => u.UserId == userId);
            foreach (var item in roleUserGroups)
            {
                var role = await BC.DbContext.GetSingleEntityAsync<SysRole>(u => u.Id == item.RoleId);
                SelectUrg.Add(role);
            }
            if (BC.IsAdmin)
            {
                haveSysRoles = await BC.DbContext.GetAllAsync<SysRole>();
            }
            else
            {
                haveSysRoles = BC.UserData.Roles;
            }
            var viewUrg = new List<ViewRole>();
            foreach (var item in haveSysRoles)
            {
                var role = SelectUrg.FirstOrDefault(u => u.Id == item.Id);
                item.AToB(out ViewRole user);
                if (role == null)
                {
                    //用IsPermission来标识是否有该权限，
                    //true表示有，false表示没有
                    user.IsPermission = false;
                }
                else
                {
                    user.IsPermission = true;
                }
                viewUrg.Add(user);
            }
            viewUrg.OrderBy(u => u.Number);
            return Ok(viewUrg);
        }


        /// <summary>
        /// 设置用户角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="viewRoles"></param>
        /// <returns></returns>
        public async Task<ResultMsg> SetRoleUser(int userId, int[] roleIds)
        {
            if (userId == 0 || roleIds == null) return Error("设置用户角色错误,请检查用户或角色");
            var urgs = await BC.DbContext.GetEntityAsync<SysRoleUser>(u => u.UserId == userId);
            var urgIds = urgs.Select(u => u.RoleId);
            var addIds = roleIds.Except(urgIds);
            var deleteIds = urgIds.Except(roleIds);
            var addUrgList = new List<SysRoleUser>();
            foreach (var item in addIds)
            {
                addUrgList.Add(new SysRoleUser()
                {
                    RoleId = item,
                    UserId = userId,
                });
            }
            var deleteUrgList = new List<SysRoleUser>();
            foreach (var item in deleteIds)
            {
                var roleUserGroup = await BC.DbContext.GetSingleEntityAsync<SysRoleUser>(u => u.RoleId == item && u.UserId == userId);
                deleteUrgList.Add(roleUserGroup);
            }
            await BC.DbContext.DeleteEntityAsync(deleteUrgList, IsDelete: true);
            await BC.DbContext.AddEntityAsync(addUrgList);
            return Ok();
        }

        public async Task<ResultMsg<List<ViewUserGroup>>> GetPermissionGroup()
        {
            var userGroup = await BC.DbContext.GetAllAsync<SysUserGroup>();
            CommonlyHelper.AToB(userGroup, out List<ViewUserGroup> viewUserGroup);
            var tree = viewUserGroup.ListToTree();
            return Ok(tree);
        }
    }
}
