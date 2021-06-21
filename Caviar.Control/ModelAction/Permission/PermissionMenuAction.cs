using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control.Permission
{
    /// <summary>
    /// 菜单权限操作器
    /// </summary>
    public partial class PermissionAction
    {
        /// <summary>
        /// 获取当前角色权限
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        public async Task<List<SysPermission>> GetCurrentRolePermissions(List<SysRole> roles)
        {
            List<SysPermission> permissions = new List<SysPermission>();
            foreach (var item in roles)
            {
                var permission = await BC.DC.GetEntityAsync<SysPermission>(u => u.IdentityId == item.Id && u.PermissionIdentity == PermissionIdentity.Role);
                permissions.AddRange(permission);
            }
            return permissions;
        }
        /// <summary>
        /// 设置角色菜单
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="menuIds"></param>
        /// <returns></returns>
        public async Task SetRoleMenu(int roleId, int[] menuIds)
        {
            var menus = await GetRoleMenu(roleId);
            if (menus == null) return;
            var Ids = menus.Where(u => u.IsPermission).Select(u => u.Id);
            var addIds = menuIds.Except(Ids).ToArray();
            var deleteIds = Ids.Except(menuIds).ToArray();
            List<SysPermission> addSysPermission = new List<SysPermission>();
            foreach (var item in addIds)
            {
                var permission = new SysPermission()
                {
                    PermissionId = item,
                    PermissionType = PermissionType.Menu,
                    IdentityId = roleId,
                    PermissionIdentity = PermissionIdentity.Role
                };
                addSysPermission.Add(permission);
            }
            List<SysPermission> deleteSysPermission = new List<SysPermission>();
            foreach (var item in deleteIds)
            {
                var permission = await BC.DC.GetFirstEntityAsync<SysPermission>(u => u.IdentityId == roleId && u.PermissionId == item && u.PermissionType == PermissionType.Menu && u.PermissionIdentity == PermissionIdentity.Role);
                deleteSysPermission.Add(permission);
            }
            await BC.DC.DeleteEntityAsync(deleteSysPermission, IsDelete: true);//该权限不需要保存，直接彻底删除
            await BC.DC.AddEntityAsync(addSysPermission);
        }
        /// <summary>
        /// 获取角色菜单
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<List<ViewMenu>> GetRoleMenu(int roleId)
        {
            List<ViewMenu> menus = new List<ViewMenu>();
            IEnumerable<SysPermission> permission;
            if (roleId == 0)
            {
                permission = BC.UserData.Permissions.Where(u => u.PermissionType == PermissionType.Menu);
            }
            else
            {
                permission = await BC.DC.GetEntityAsync<SysPermission>(u => u.IdentityId == roleId && u.PermissionType == PermissionType.Menu && u.PermissionIdentity == PermissionIdentity.Role);
            }
            var allMneus = BC.UserData.Menus;
            if (BC.IsAdmin)
            {
                allMneus = await BC.DC.GetAllAsync<SysMenu>();
            }
            foreach (var item in allMneus)
            {
                var menu = permission.FirstOrDefault(u => u.PermissionId == item.Id);
                item.AToB(out ViewMenu viewMenu);
                viewMenu.IsPermission = menu != null;
                menus.Add(viewMenu);
            }
            menus.OrderBy(u => u.Number);
            return menus;
        }
    }
}
