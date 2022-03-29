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
        private string[] SpecialUsers = new string[]
        {
            CurrencyConstant.TouristUser,
        };
        Interactor _interactor;
        UserManager<ApplicationUser> _userManager;
        public UserServices(Interactor interactor,UserManager<ApplicationUser> userManager,IAppDbContext appDbContext):base(appDbContext)
        {
            _interactor = interactor;
            _userManager = userManager;
        }

        public async Task<IdentityResult> AssignRolesAsync(string userName,IList<string> roles)
        {
            var user = await GetUserInfoAsync(userName);
            var currentRoles = await GetRolesAsync(user);
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
        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            if (user == null) return new List<string>();
            var roles = await _userManager.GetRolesAsync(user);
            return roles;
        }

        public async Task<IList<int>> GetRoleIdsAsync(ApplicationUser user)
        {
            if (user == null) return new List<int>();
            var roleSet = AppDbContext.DbContext.Set<IdentityUserRole<int>>();
            var roleIds = await roleSet.Where(u => u.UserId == user.Id).Select(u=>u.RoleId).ToListAsync();
            return roleIds;
        }

        public async Task<UserDetails> GetUserDetailsAsync()
        {
            var user = await GetCurrentUserInfoAsync();
            var useerGroup = await AppDbContext.SingleOrDefaultAsync<SysUserGroup>(u => u.Id == user.UserGroupId);
            UserDetails useerDetails = new UserDetails() 
            { 
                UserName = user.UserName,
                AccountName = user.AccountName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Remark = user.Remark,
                Roles = await GetRolesAsync(user),
                UserGroupName = useerGroup.Name,
                HeadPortrait = user.HeadPortrait,
            };
            return useerDetails;
        }
        public async Task<IdentityResult> UpdateUserDetailsAsync(UserDetails userDetails)
        {
            var user = await GetCurrentUserInfoAsync();
            user.AccountName = userDetails.AccountName;
            user.Email = userDetails.Email;
            user.PhoneNumber = userDetails.PhoneNumber;
            user.Remark = userDetails.Remark;
            user.HeadPortrait = userDetails.HeadPortrait;
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> UpdateUserAsync(string operatorUp,ApplicationUserView vm)
        {
            if (SpecialUsers.Contains(vm.Entity.UserName))
            {
                throw new Exception("特殊用户，禁止修改");
            }
            var user = await _userManager.FindByIdAsync(vm.Entity.Id.ToString());
            if (user == null) throw new ArgumentNullException($"{vm.Entity.UserName}不存在");
            user.UserName = vm.Entity.UserName;
            user.AccountName = vm.Entity.AccountName;
            user.PhoneNumber = vm.Entity.PhoneNumber;
            user.Email = vm.Entity.Email;
            user.UserGroupId = vm.Entity.UserGroupId;
            user.IsDisable = vm.Entity.IsDisable;
            user.Number = vm.Entity.Number;
            user.Remark = vm.Entity.Remark;
            user.UpdateTime = CommonHelper.GetSysDateTimeNow();
            user.OperatorUp = operatorUp;
            var result = await _userManager.UpdateAsync(user);
            return result;
        }

        public Task<IdentityResult> DeleteUserAsync(ApplicationUserView vm)
        {
            if (SpecialUsers.Contains(vm.Entity.UserName))
            {
                throw new Exception("特殊用户，禁止删除");
            }
            if (vm.Entity.Id == 1)
            {
                throw new Exception("非法操作，无法删除管理员账号");
            }
            var result = _userManager.DeleteAsync(vm.Entity);
            return result;
        }

        /// <summary>
        /// 获取指定角色所有权限
        /// </summary>
        /// <returns></returns>
        public Task<List<SysPermission>> GetPermissionsAsync(List<int> roleIds,Expression<Func<SysPermission, bool>> whereLambda)
        {
            var permissionsSet = AppDbContext.DbContext.Set<SysPermission>();
            return permissionsSet.Where(u => roleIds.Contains(u.Entity)).Where(whereLambda).ToListAsync();
        }

        /// <summary>
        /// 获取权限菜单
        /// 此处菜单权限取自角色
        /// </summary>
        /// <returns></returns>
        public async Task<List<SysMenuView>> GetPermissionMenus(List<string> permissionUrls)
        {
            var menus = await AppDbContext.GetAllAsync<SysMenu>().ToListAsync();
            var menuViews = menus.Select(u => new SysMenuView() { Entity = u });
            foreach (var item in menuViews)
            {
                // 由于目录没有url，所以此处没有url的判断id
                item.IsPermission = CommonHelper.IsMenuPermissions(item,permissionUrls);
            }
            return menuViews.ListToTree();
        }
        /// <summary>
        /// 保存菜单权限
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="urls"></param>
        /// <returns></returns>
        public async Task<int> SavePermissionMenusAsync(int roleId, List<string> urls)
        {
            var permissionMenus = await GetPermissionsAsync(new List<int>() { roleId }, u => u.PermissionType == PermissionType.RoleMenus);
            var menuUrls = GetPermissionsAsync(permissionMenus);
            var reomveMenus = permissionMenus.Where(u => !urls.Contains(u.Permission)).ToList();
            AppDbContext.DbContext.RemoveRange(reomveMenus);
            var addMenus = urls.Where(u=> !menuUrls.Contains(u)).Select(u => new SysPermission() { Permission = u, PermissionType = PermissionType.RoleMenus, Entity = roleId }).ToList();
            AppDbContext.DbContext.AddRange(addMenus);
            return await AppDbContext.DbContext.SaveChangesAsync();
        }

        public async Task<CurrentUser> GetCurrentUserInfoAsync(ClaimsPrincipal User,bool TouristVisit)
        {
            List<CaviarClaim> claims = null;
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = await _userManager.FindByNameAsync(User.Identity.Name);
                if(applicationUser == null)
                {
                    return new CurrentUser() { IsAuthenticated = false };
                }
                claims = new List<CaviarClaim>() 
                { 
                    new CaviarClaim(CurrencyConstant.HeadPortrait, applicationUser.HeadPortrait ?? ""),
                    new CaviarClaim(CurrencyConstant.AccountName,applicationUser.AccountName),
                };
                claims.AddRange(User.Claims.Select(u => new CaviarClaim(u)));
                var currentUser = new CurrentUser
                {
                    IsAuthenticated = User.Identity.IsAuthenticated,
                    UserName = User.Identity.Name,
                    Claims = claims
                };
                return await Task.FromResult(currentUser);
            }
            else if (TouristVisit)
            {
                var applicationUser = await _userManager.FindByNameAsync(CurrencyConstant.TouristUser);
                if (applicationUser == null)
                {
                    return new CurrentUser() { IsAuthenticated = false };
                }
                claims = new List<CaviarClaim>()
                {
                    new CaviarClaim(CurrencyConstant.HeadPortrait, applicationUser.HeadPortrait ?? ""),
                    new CaviarClaim(CurrencyConstant.AccountName,applicationUser.AccountName),
                };
                claims.AddRange(User.Claims.Select(u => new CaviarClaim(u)));
                var currentUser = new CurrentUser
                {
                    IsAuthenticated = true,
                    UserName = applicationUser.AccountName,
                    Claims = claims,
                    TouristVisit = true
                };
                return await Task.FromResult(currentUser);
            }
            else
            {
                var currentUser = new CurrentUser
                {
                    IsAuthenticated = User.Identity.IsAuthenticated,
                    UserName = User.Identity.Name,
                    Claims = claims
                };
                return await Task.FromResult(currentUser);
            }
            
        }

        /// <summary>
        /// 获取当前用户所有权限或者指定权限
        /// </summary>
        /// <returns></returns>
        public async Task<List<SysPermission>> GetPermissionsAsync(Expression<Func<SysPermission, bool>> whereLambda)
        {
            var user = await GetCurrentUserInfoAsync();
            var roles = await GetRoleIdsAsync(user);
            var permissionsSet = AppDbContext.DbContext.Set<SysPermission>();
            return permissionsSet.Where(u => roles.Contains(u.Entity)).Where(whereLambda).ToList();
        }

        /// <summary>
        /// 获取当前用户所有权限或者指定权限
        /// </summary>
        /// <returns></returns>
        public async Task<List<SysPermission>> GetPermissionsAsync()
        {
            var user = await GetCurrentUserInfoAsync();
            var roles = await GetRoleIdsAsync(user);
            var permissionsSet = AppDbContext.DbContext.Set<SysPermission>();
            return permissionsSet.Where(u => roles.Contains(u.Entity)).ToList();
        }
        /// <summary>
        /// 获取权限实体
        /// </summary>
        /// <param name="sysPermissions"></param>
        /// <returns></returns>
        public List<string> GetPermissionsAsync(List<SysPermission> sysPermissions)
        {
            return sysPermissions.Select(u => u.Permission).ToHashSet().ToList();
        }

        public Task<ApplicationUser> GetCurrentUserInfoAsync()
        {
            return Task.FromResult(_interactor.UserInfo);
        }

        public async Task<ApplicationUser> GetUserInfoAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user;
        }

    }
}
