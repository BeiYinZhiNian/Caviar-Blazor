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
                    var rolePermission = await BC.DC.GetEntityAsync<SysRolePermission>(u => u.RoleId == item.Id);
                    foreach (var permissionItem in rolePermission)
                    {
                        permissions.Add(permissionItem.Permission);
                    }
                    
                }
            }
            return permissions;
        }

        public async Task<bool> SetRoleMenu(int roleId,int[] menuIds)
        {
            var menus = await GetRoleMenu(roleId);
            if (menus == null) return false;
            var Ids = menus.Select(u => u.Id);
            var addIds = menuIds.Except(Ids).ToArray();
            var deleteIds = Ids.Except(menuIds).ToArray();
            foreach (var item in addIds)
            {
                using (var transaction = BC.DC.BeginTransaction())
                {
                    var permission = new SysPermission()
                    {
                        PermissionId = item,
                        PermissionType = PermissionType.Menu,
                    };
                    var count = await BC.DC.AddEntityAsync(permission);
                    var rolePermission = new SysRolePermission()
                    {
                        RoleId = roleId,
                        PermissionId = permission.Id
                    };
                    await BC.DC.AddEntityAsync(rolePermission);
                    transaction.Commit();
                }
            }
            foreach (var item in deleteIds)
            {
                var rolePermission = await BC.DC.GetFirstEntityAsync<SysRolePermission>(u => u.RoleId == roleId);
                using (var transaction = BC.DC.BeginTransaction())
                {
                    var permission = await BC.DC.GetFirstEntityAsync<SysPermission>(u => u.PermissionId == item && u.PermissionType == PermissionType.Menu);
                    
                    await BC.DC.DeleteEntityAsync(permission);
                    await BC.DC.DeleteEntityAsync(rolePermission);
                    transaction.Commit();
                }
            }
            return true;
        }

        public async Task<List<SysMenu>> GetRoleMenu(int roleId)
        {
            var rolePermission = await BC.DC.GetEntityAsync<SysRolePermission>(u => u.RoleId == roleId);
            if (rolePermission == null) return null;
            List<SysMenu> menus = new List<SysMenu>();
            foreach (var item in rolePermission)
            {
                var permission = BC.Permissions.FirstOrDefault(u => u.Id == item.PermissionId && u.PermissionType == PermissionType.Menu);
                if (permission == null) return null;
                var menu = BC.Menus.FirstOrDefault(u => u.Id == permission.PermissionId);
                if (menu == null) return null;
                menus.Add(menu);
            }
            
            return menus;
        }
    }
}
