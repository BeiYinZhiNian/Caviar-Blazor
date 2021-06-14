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
        public async Task<List<SysPermission>> GetCurrentPermissions(List<SysRole> roles)
        {
            if (BC.IsAdmin)
            {
                return await BC.DC.GetAllAsync<SysPermission>();
            }
            List<SysPermission> permissions = new List<SysPermission>();
            foreach (var item in roles)
            {
                var permission = await BC.DC.GetEntityAsync<SysPermission>(u => u.RoleId == item.Id);
                permissions.AddRange(permission);
            }
            return permissions;
        }

        public async Task SetRoleMenu(int roleId,int[] menuIds)
        {
            var menus = GetRoleMenu(roleId);
            if (menus == null) return;
            var Ids = menus.Select(u => u.Id);
            var addIds = menuIds.Except(Ids).ToArray();
            var deleteIds = Ids.Except(menuIds).ToArray();
            List<SysPermission> addSysPermission = new List<SysPermission>();
            foreach (var item in addIds)
            {
                var permission = new SysPermission()
                {
                    PermissionId = item,
                    PermissionType = PermissionType.Menu,
                    RoleId = roleId
                };
                addSysPermission.Add(permission);
            }
            List<SysPermission> deleteSysPermission = new List<SysPermission>();
            foreach (var item in deleteIds)
            {
                var permission = await BC.DC.GetFirstEntityAsync<SysPermission>(u => u.RoleId == roleId && u.PermissionId == item && u.PermissionType == PermissionType.Menu);
                deleteSysPermission.Add(permission);
            }
            await BC.DC.DeleteEntityAsync(deleteSysPermission, IsDelete: true);//该权限不需要保存，直接彻底删除
            await BC.DC.AddEntityAsync(addSysPermission);
        }

        public List<SysMenu> GetRoleMenu(int roleId)
        {
            List<SysMenu> menus = new List<SysMenu>();
            var permission = BC.Permissions.Where(u => u.RoleId == roleId && u.PermissionType == PermissionType.Menu);
            foreach (var item in permission)
            {
                var menu = BC.Menus.FirstOrDefault(u => u.Id == item.PermissionId);
                if (menu == null) return null;
                menus.Add(menu);
            }
            return menus;
        }

        public async Task<List<SysModelFields>> GetRoleFields(string fullName,int roleId = 0)
        {
            if (string.IsNullOrEmpty(fullName)) return null;
            IEnumerable<SysPermission> permission;
            if (roleId == 0)
            {
                permission = BC.Permissions.Where(u => u.PermissionType == PermissionType.Field);
            }
            else{
                permission = BC.Permissions.Where(u => u.RoleId == roleId && u.PermissionType == PermissionType.Field);
            }
            var sysModelFields = new List<SysModelFields>();
            var fields = await BC.DC.GetEntityAsync<SysModelFields>(u => u.FullName == fullName);
            foreach (var item in fields)
            {
                if (permission.FirstOrDefault(u => u.Id == item.Id)!=null)
                {
                    item.IsDisable = false;
                    sysModelFields.Add(item);
                }
            }
            return fields;
        }

        public async Task SetRoleFields(string fullName, int roleId, List<SysModelFields> modelFields)
        {
            if (string.IsNullOrEmpty(fullName) || roleId == 0) return;
            var permission = await BC.DC.GetEntityAsync<SysPermission>(u => u.RoleId == roleId && u.PermissionType == PermissionType.Field);
            var fields = await BC.DC.GetEntityAsync<SysModelFields>(u => u.FullName == fullName);
            foreach (var item in modelFields)
            {
                var field = fields.FirstOrDefault(u => u.TypeName == item.TypeName);
                if (field == null) continue;
                var perm = permission.FirstOrDefault(u => u.PermissionType == PermissionType.Field && u.PermissionId == field.Id);
                if (item.IsDisable)
                {
                    if (perm != null)
                    {
                        await BC.DC.DeleteEntityAsync(perm);
                    }

                }
                else
                {
                    if (perm == null)
                    {
                        perm = new SysPermission()
                        {
                            PermissionType = PermissionType.Field,
                            PermissionId = field.Id,
                            RoleId = roleId,
                        };
                        await BC.DC.AddEntityAsync(perm);
                    }
                }
            }
        }
    }
}
