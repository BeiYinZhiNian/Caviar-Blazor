using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control.Permission
{
    /// <summary>
    /// 用户组权限操作器
    /// </summary>
    public partial class PermissionAction
    {
        /// <summary>
        /// 获取用户组的角色
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        public async Task<List<ViewRole>> GetRoleUserGropu(int userGroupId)
        {
            List<SysRole> SelectUrg = new List<SysRole>();
            List<SysRole> haveSysRoles = new List<SysRole>();
            var roleUserGroups = await BC.DC.GetEntityAsync<SysRoleUserGroup>(u => u.UserGroupId == userGroupId);
            foreach (var item in roleUserGroups)
            {
                SelectUrg.Add(item.Role);
            }
            if (BC.IsAdmin)
            {
                haveSysRoles = await BC.DC.GetAllAsync<SysRole>();
            }
            else
            {
                haveSysRoles = BC.UserData.Roles;
            }
            var viewUrg = new List<ViewRole>();
            foreach (var item in haveSysRoles)
            {
                var role = SelectUrg.FirstOrDefault(u => u.Id == item.Id);
                item.AToB(out ViewRole userGroup);
                if (role == null)
                {
                    //用IsPermission来标识是否有该权限，
                    //true表示有，false表示没有
                    userGroup.IsPermission = false;
                }
                else
                {
                    userGroup.IsPermission = true;
                }
                viewUrg.Add(userGroup);
            }
            viewUrg.OrderBy(u => u.Number);
            return viewUrg;
        }
        /// <summary>
        /// 设置用户组角色
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <param name="viewRoles"></param>
        /// <returns></returns>
        public async Task SetRoleUserGropu(int userGroupId,int[] roleIds)
        {
            var urgs = await BC.DC.GetEntityAsync<SysRoleUserGroup>(u => u.UserGroupId == userGroupId);
            var urgIds = urgs.Select(u => u.RoleId);
            var addIds = roleIds.Except(urgIds);
            var deleteIds = urgIds.Except(roleIds);
            var addUrgList = new List<SysRoleUserGroup>();
            foreach (var item in addIds)
            {
                addUrgList.Add(new SysRoleUserGroup() 
                {
                    RoleId = item,
                    UserGroupId = userGroupId,
                });
            }
            var deleteUrgList = new List<SysRoleUserGroup>();
            foreach (var item in deleteIds)
            {
                var roleUserGroup = await BC.DC.GetFirstEntityAsync<SysRoleUserGroup>(u => u.RoleId == item && u.UserGroupId == userGroupId);
                deleteUrgList.Add(roleUserGroup);
            }
            await BC.DC.DeleteEntityAsync(deleteUrgList,IsDelete:true);
            await BC.DC.AddEntityAsync(addUrgList);
        }
    }
}
