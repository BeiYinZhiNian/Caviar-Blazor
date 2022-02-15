using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Identity;
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
        public UserServices(Interactor interactor,UserManager<T> userManager)
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
    }
}
