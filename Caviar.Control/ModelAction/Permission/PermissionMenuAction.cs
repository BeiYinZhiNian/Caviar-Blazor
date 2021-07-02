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
        /// 获取权限菜单
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public async Task<List<SysMenu>> GetPermissionMenu(List<SysPermission> permissions)
        {
            List<SysMenu> menus = new List<SysMenu>();
            permissions = permissions.Where(u => u.PermissionType == PermissionType.Menu).ToList();
            foreach (var item in permissions)
            {
                if (menus.SingleOrDefault(u => u.Id == item.PermissionId) != null) continue;
                var menu = await BC.DC.GetSingleEntityAsync<SysMenu>(u => u.Id == item.PermissionId);
                if (menu == null) continue;
                menus.Add(menu);
            }
            return menus.OrderBy(u => u.Number).ToList();
        }


        /// <summary>
        /// 获取指定角色权限
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        public async Task<List<SysPermission>> GetRolePermissions(List<SysRole> roles)
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
        /// 获取指定用户的权限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<SysPermission>> GetUserPermissions(int userId)
        {
            List<SysPermission> permissions = new List<SysPermission>();
            if (userId < 1) return permissions;
            var permission = await BC.DC.GetEntityAsync<SysPermission>(u => u.IdentityId == userId && u.PermissionIdentity == PermissionIdentity.User);
            permissions.AddRange(permission);
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
                var permission = await BC.DC.GetSingleEntityAsync<SysPermission>(u => u.IdentityId == roleId && u.PermissionId == item && u.PermissionType == PermissionType.Menu && u.PermissionIdentity == PermissionIdentity.Role);
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

        /// <summary>
        /// 为当前用户添加菜单权限
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public async Task<bool> SetMenuUser(int menuId)
        {
            if (BC.UserToken.Id < 1) return false;
            var count = await SetMenuUser(menuId, BC.UserToken.Id);
            return count > 0;
        }

        /// <summary>
        /// 为指定角色添加菜单权限
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        protected async Task<int> SetMenuUser(int menuId,int userId)
        {
            SysPermission permission = new SysPermission()
            {
                PermissionId = menuId,
                IdentityId = userId,
                PermissionIdentity = PermissionIdentity.User,
                PermissionType = PermissionType.Menu,
            };
            return await BC.DC.AddEntityAsync(permission);
        }
    }
}
