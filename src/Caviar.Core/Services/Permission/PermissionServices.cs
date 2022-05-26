// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.EntityFrameworkCore;

namespace Caviar.Core.Services
{
    public class PermissionServices : DbServices
    {
        private readonly ILanguageService _languageService;
        private readonly Interactor _interactor;
        public PermissionServices(IAppDbContext dbContext, ILanguageService languageService, Interactor interactor) : base(dbContext)
        {
            _languageService = languageService;
            _interactor = interactor;
        }

        /// <summary>
        /// 获取指定角色所有权限
        /// </summary>
        /// <returns></returns>
        public Task<List<SysPermission>> GetPermissionsAsync(List<int> roleIds, Expression<Func<SysPermission, bool>> whereLambda)
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
            var menuViews = menus.Select(u => new SysMenuView() { Entity = u }).ToList();
            foreach (var item in menuViews)
            {
                // 由于目录没有url，所以此处没有url的判断id
                item.IsPermission = permissionUrls.Contains(item.Entity.Url) || (permissionUrls.Contains(item.Entity.Id.ToString()) && string.IsNullOrEmpty(item.Entity.Url));
                string key = $"{CurrencyConstant.Menu}.{item.Entity.MenuName}";
                item.DisplayName = _languageService[key];//翻译显示名称
            }
            menuViews = menuViews.ListToTree();
            return menuViews;
        }

        /// <summary>
        /// 保存菜单权限
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="urls"></param>
        /// <returns></returns>
        public async Task<int> SavePermissionMenusAsync(int roleId, List<string> urls)
        {
            var permissionMenus = await GetPermissionsAsync(new List<int>() { roleId }, u => u.PermissionType == (int)PermissionType.RoleMenus);
            var menuUrls = GetPermissionsAsync(permissionMenus);
            var reomveMenus = permissionMenus.Where(u => !urls.Contains(u.Permission)).ToList();
            AppDbContext.DbContext.RemoveRange(reomveMenus);
            var addUrls = urls.Where(u => !menuUrls.Contains(u)).Select(u => u).ToHashSet();
            var addMenus = addUrls.Select(u => new SysPermission() { Permission = u, PermissionType = (int)PermissionType.RoleMenus, Entity = roleId }).ToList();
            AppDbContext.DbContext.AddRange(addMenus);
            return await AppDbContext.DbContext.SaveChangesAsync();
        }


        /// <summary>
        /// 获取当前用户所有权限或者指定权限
        /// </summary>
        /// <returns></returns>
        public Task<List<SysPermission>> GetPermissionsAsync(IList<int> roles, Expression<Func<SysPermission, bool>> whereLambda)
        {
            var user = _interactor.UserInfo;
            var permissionsSet = AppDbContext.DbContext.Set<SysPermission>();
            return permissionsSet.Where(u => roles.Contains(u.Entity)).Where(whereLambda).ToListAsync();
        }

        /// <summary>
        /// 获取当前用户所有权限或者指定权限
        /// </summary>
        /// <returns></returns>
        public Task<List<SysPermission>> GetPermissionsAsync(IList<int> roles)
        {
            var user = _interactor.UserInfo;
            var permissionsSet = AppDbContext.DbContext.Set<SysPermission>();
            return permissionsSet.Where(u => roles.Contains(u.Entity)).ToListAsync();
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

    }
}
