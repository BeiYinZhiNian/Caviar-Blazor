// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CacheManager.Core;
using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.User;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Caviar.Core.Services
{
    public class UserServices : DbServices
    {
        private readonly string[] _specialUsers = new string[]
        {
            CurrencyConstant.TouristUser,
        };
        private readonly Interactor _interactor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CaviarConfig _caviarConfig;
        private readonly ICacheManager<object> _cacheManager;
        public UserServices(
            Interactor interactor,
            UserManager<ApplicationUser> userManager,
            IAppDbContext appDbContext,
            CaviarConfig caviarConfig,
            ICacheManager<object> cacheManager) : base(appDbContext)
        {
            _interactor = interactor;
            _userManager = userManager;
            _caviarConfig = caviarConfig;
            _cacheManager = cacheManager;
        }

        public async Task<IdentityResult> AssignRolesAsync(string userName, IList<string> roles)
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
                throw new ResultException(new ResultMsg() { Title = "移除角色时发生错误", Status = System.Net.HttpStatusCode.BadRequest, Detail = dic });
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
            var cacheName = $"roles_{user.Id}";
            var cache = _cacheManager.Get<IList<string>>(cacheName);
            if (cache != null) return cache;
            var roles = await _userManager.GetRolesAsync(user);
            _cacheManager.Add(cacheName, roles);
            return roles;
        }

        public async Task<IList<int>> GetRoleIdsAsync(ApplicationUser user)
        {
            if (user == null) return new List<int>();
            var roleSet = AppDbContext.DbContext.Set<IdentityUserRole<int>>();
            var roleIds = await roleSet.Where(u => u.UserId == user.Id).Select(u => u.RoleId).ToListAsync();
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

        public async Task<IdentityResult> UpdateUserAsync(string operatorUp, ApplicationUserView vm)
        {
            if (_specialUsers.Contains(vm.Entity.UserName))
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
            _cacheManager.Clear();
            return result;
        }

        public Task<IdentityResult> DeleteUserAsync(ApplicationUserView vm)
        {
            if (_specialUsers.Contains(vm.Entity.UserName))
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


        public async Task<CurrentUser> GetCurrentUserInfoAsync(ClaimsPrincipal user)
        {
            List<CaviarClaim> claims = null;
            if (user.Identity.IsAuthenticated)
            {
                var applicationUser = await GetUserInfoAsync(user.Identity.Name);
                if (applicationUser == null)
                {
                    return new CurrentUser() { IsAuthenticated = false };
                }
                claims = new List<CaviarClaim>()
                {
                    new CaviarClaim(CurrencyConstant.HeadPortrait, applicationUser.HeadPortrait ?? ""),
                    new CaviarClaim(CurrencyConstant.AccountName,applicationUser.AccountName),
                };
                claims.AddRange(user.Claims.Select(u => new CaviarClaim(u)));
                var currentUser = new CurrentUser
                {
                    IsAuthenticated = user.Identity.IsAuthenticated,
                    UserName = user.Identity.Name,
                    Claims = claims
                };
                return await Task.FromResult(currentUser);
            }
            else if (_caviarConfig.TouristVisit)
            {
                var applicationUser = await GetUserInfoAsync(CurrencyConstant.TouristUser);
                if (applicationUser == null)
                {
                    return new CurrentUser() { IsAuthenticated = false };
                }
                claims = new List<CaviarClaim>()
                {
                    new CaviarClaim(CurrencyConstant.HeadPortrait, applicationUser.HeadPortrait ?? ""),
                    new CaviarClaim(CurrencyConstant.AccountName,applicationUser.AccountName),
                    new CaviarClaim(CurrencyConstant.TouristVisit,true.ToString())
                };
                claims.AddRange(user.Claims.Select(u => new CaviarClaim(u)));
                var currentUser = new CurrentUser
                {
                    IsAuthenticated = true,
                    UserName = applicationUser.AccountName,
                    Claims = claims
                };
                return await Task.FromResult(currentUser);
            }
            else
            {
                var currentUser = new CurrentUser
                {
                    IsAuthenticated = user.Identity.IsAuthenticated,
                    UserName = user.Identity.Name,
                    Claims = claims
                };
                return await Task.FromResult(currentUser);
            }

        }

        public Task<ApplicationUser> GetCurrentUserInfoAsync()
        {
            return Task.FromResult(_interactor.UserInfo);
        }

        public async Task<ApplicationUser> GetUserInfoAsync(string userName)
        {
            var cacheName = $"userInfo_{userName}";
            var cache = _cacheManager.Get<ApplicationUser>(cacheName);
            if (cache != null) return cache;
            cache = await _userManager.FindByNameAsync(userName);
            _cacheManager.Add(cacheName, cache);
            return cache;
        }

    }
}
