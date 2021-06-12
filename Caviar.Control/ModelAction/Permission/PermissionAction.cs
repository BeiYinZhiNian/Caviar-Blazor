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
        public async Task<List<SysPermission>> GetCurrentPermissions(List<SysRole> roles,bool isAdmin = false)
        {
            if (isAdmin)
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

        public async Task SetRoleFields(string modelName,int roleId, List<SysModelFields> modelFields)
        {
            await BC.DC.UpdateEntityAsync(modelFields);
            var permission = await BC.DC.GetEntityAsync<SysPermission>(u => u.RoleId == roleId && u.PermissionType == PermissionType.Field);
            var fieldIds = modelFields.Where(u => u.IsDisable).Select(u => u.Id);
        }
    }
}
