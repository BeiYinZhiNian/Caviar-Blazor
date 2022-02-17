using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Services
{
    public class UserServices<T>:DbServices where T : class
    {
        Interactor _interactor;
        UserManager<T> _userManager;
        public UserServices(Interactor interactor,UserManager<T> userManager,IAppDbContext appDbContext):base(appDbContext)
        {
            _interactor = interactor;
            _userManager = userManager;
        }

        /// <summary>
        /// 获取当前用户所有角色
        /// </summary>
        /// <returns></returns>
        public async Task<IList<string>> GetRoles()
        {
            if (!_interactor.User.Identity.IsAuthenticated) return new List<string>();
            var user = await _userManager.FindByNameAsync(_interactor.User.Identity.Name);
            var roles = await _userManager.GetRolesAsync(user);
            return roles;
        }

        /// <summary>
        /// 获取指定角色所有权限
        /// </summary>
        /// <returns></returns>
        public Task<List<SysPermission>> GetPermissions(List<string> roleName,Expression<Func<SysPermission, bool>> whereLambda)
        {
            var permissionsSet = AppDbContext.DbContext.Set<SysPermission>();
            return permissionsSet.Where(u => roleName.Contains(u.Entity)).Where(whereLambda).ToListAsync();
        }

        public async Task<int> SavePermissionMenus(string roleName, List<string> urls)
        {
            var permissionMenus = await GetPermissions(new List<string>() { roleName }, u => u.PermissionType == PermissionType.RoleMenus);
            var menuUrls = GetPermissions(permissionMenus);
            var reomveMenus = permissionMenus.Where(u => !urls.Contains(u.Permission)).ToList();
            AppDbContext.DbContext.RemoveRange(reomveMenus);
            var addMenus = urls.Where(u=> !menuUrls.Contains(u)).Select(u => new SysPermission() { Permission = u, PermissionType = PermissionType.RoleMenus, Entity = roleName }).ToList();
            AppDbContext.DbContext.AddRange(addMenus);
            return await AppDbContext.DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 获取当前用户所有权限或者指定权限
        /// </summary>
        /// <returns></returns>
        public async Task<List<SysPermission>> GetPermissions(Expression<Func<SysPermission, bool>> whereLambda)
        {
            var roles = await GetRoles();
            var permissionsSet = AppDbContext.DbContext.Set<SysPermission>();
            return permissionsSet.Where(u => roles.Contains(u.Entity)).Where(whereLambda).ToList();
        }

        /// <summary>
        /// 获取当前用户所有权限或者指定权限
        /// </summary>
        /// <returns></returns>
        public async Task<List<SysPermission>> GetPermissions()
        {
            var roles = await GetRoles();
            var permissionsSet = AppDbContext.DbContext.Set<SysPermission>();
            return permissionsSet.Where(u => roles.Contains(u.Entity)).ToList();
        }
        /// <summary>
        /// 获取权限实体
        /// </summary>
        /// <param name="sysPermissions"></param>
        /// <returns></returns>
        public List<string> GetPermissions(List<SysPermission> sysPermissions)
        {
            return sysPermissions.Select(u => u.Permission).ToList();
        }
    }
}
