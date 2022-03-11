using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.User;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Services
{
    public class UserServices:DbServices
    {
        Interactor _interactor;
        UserManager<ApplicationUser> _userManager;
        public UserServices(Interactor interactor,UserManager<ApplicationUser> userManager,IAppDbContext appDbContext):base(appDbContext)
        {
            _interactor = interactor;
            _userManager = userManager;
        }

        public async Task<IdentityResult> AssignRoles(string userName,IList<string> roles)
        {
            var user = await GetUserInfo(userName);
            var currentRoles = await GetRoles(user);
            var addRoles = roles.Where(u => !currentRoles.Contains(u));
            var removeRoles = currentRoles.Where(u => !roles.Contains(u));
            var result = await _userManager.AddToRolesAsync(user, addRoles);
            if (!result.Succeeded)
            {
                return result;
            }
            result = await _userManager.RemoveFromRolesAsync(user, removeRoles);
            if (!result.Succeeded)
            {
                var dic = new Dictionary<string, string>();
                foreach (var item in result.Errors)
                {
                    dic.Add(item.Code, item.Description);
                }
                throw new ResultException(new ResultMsg() { Title="移除角色时发生错误",Status = System.Net.HttpStatusCode.BadRequest ,Detail = dic });
            }
            return result;
        }

        /// <summary>
        /// 获取指定用户所有角色
        /// </summary>
        /// <returns></returns>
        public async Task<IList<string>> GetRoles(ApplicationUser user)
        {
            if (user == null) return new List<string>();
            var roles = await _userManager.GetRolesAsync(user);
            return roles;
        }

        public async Task<IList<int>> GetRoleIds(ApplicationUser user)
        {
            if (user == null) return new List<int>();
            var roleSet = AppDbContext.DbContext.Set<IdentityUserRole<int>>();
            var roleIds = await roleSet.Where(u => u.UserId == user.Id).Select(u=>u.RoleId).ToListAsync();
            return roleIds;
        }

        public async Task<UserDetails> GetUserDetails(string userName)
        {
            var user = await GetUserInfo(userName);
            var useerGroup = await AppDbContext.SingleOrDefaultAsync<SysUserGroup>(u => u.Id == user.Id);
            UserDetails useerDetails = new UserDetails() 
            { 
                UserName = userName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Remark = user.Remark,
                Roles = await GetRoles(user),
                UserGroupName = useerGroup.Name,
                HeadPortrait = user.HeadPortrait,
            };
            return useerDetails;
        }
        public async Task<IdentityResult> UpdateUserDetails(string userName,UserDetails userDetails)
        {
            var user = await GetUserInfo(userName);
            user.Email = userDetails.Email;
            user.PhoneNumber = userDetails.PhoneNumber;
            user.Remark = userDetails.Remark;
            user.HeadPortrait = userDetails.HeadPortrait;
            return await _userManager.UpdateAsync(user);
        }

        /// <summary>
        /// 获取指定角色所有权限
        /// </summary>
        /// <returns></returns>
        public Task<List<SysPermission>> GetPermissions(List<int> roleIds,Expression<Func<SysPermission, bool>> whereLambda)
        {
            var permissionsSet = AppDbContext.DbContext.Set<SysPermission>();
            return permissionsSet.Where(u => roleIds.Contains(u.Entity)).Where(whereLambda).ToListAsync();
        }
        /// <summary>
        /// 保存菜单权限
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="urls"></param>
        /// <returns></returns>
        public async Task<int> SavePermissionMenus(int roleId, List<string> urls)
        {
            var permissionMenus = await GetPermissions(new List<int>() { roleId }, u => u.PermissionType == PermissionType.RoleMenus);
            var menuUrls = GetPermissions(permissionMenus);
            var reomveMenus = permissionMenus.Where(u => !urls.Contains(u.Permission)).ToList();
            AppDbContext.DbContext.RemoveRange(reomveMenus);
            var addMenus = urls.Where(u=> !menuUrls.Contains(u)).Select(u => new SysPermission() { Permission = u, PermissionType = PermissionType.RoleMenus, Entity = roleId }).ToList();
            AppDbContext.DbContext.AddRange(addMenus);
            return await AppDbContext.DbContext.SaveChangesAsync();
        }

        public async Task<CurrentUser> GetCurrentUserInfo(ClaimsPrincipal User)
        {
            List<CaviarClaim> claims = null;
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = await _userManager.FindByNameAsync(User.Identity.Name);
                claims = new List<CaviarClaim>() { new CaviarClaim(CurrencyConstant.HeadPortrait, applicationUser.HeadPortrait ?? "") };
                claims.AddRange(User.Claims.Select(u => new CaviarClaim(u)));
            }
            var currentUser = new CurrentUser
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                UserName = User.Identity.Name,
                Claims = claims
            };
            return await Task.FromResult(currentUser);
        }

        /// <summary>
        /// 获取当前用户所有权限或者指定权限
        /// </summary>
        /// <returns></returns>
        public async Task<List<SysPermission>> GetPermissions(Expression<Func<SysPermission, bool>> whereLambda)
        {
            var user = await GetCurrentUserInfo();
            var roles = await GetRoleIds(user);
            var permissionsSet = AppDbContext.DbContext.Set<SysPermission>();
            return permissionsSet.Where(u => roles.Contains(u.Entity)).Where(whereLambda).ToList();
        }

        /// <summary>
        /// 获取当前用户所有权限或者指定权限
        /// </summary>
        /// <returns></returns>
        public async Task<List<SysPermission>> GetPermissions()
        {
            var user = await GetCurrentUserInfo();
            var roles = await GetRoleIds(user);
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

        public async Task<ApplicationUser> GetCurrentUserInfo()
        {
            var user = await _userManager.GetUserAsync(_interactor.User);
            return user;
        }

        public async Task<ApplicationUser> GetUserInfo(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user;
        }

    }
}
