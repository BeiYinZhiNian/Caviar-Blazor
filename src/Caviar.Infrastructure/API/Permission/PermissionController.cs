﻿// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CacheManager.Core;
using Caviar.Core.Services;
using Caviar.Infrastructure.API.BaseApi;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Caviar.Infrastructure.API.Permission
{
    public partial class PermissionController : BaseApiController
    {
        private readonly RoleFieldServices _roleFieldServices;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly PermissionServices _permissionServices;
        private readonly ILanguageService _languageService;
        private readonly ICacheManager<object> _cacheManager;
        public PermissionController(RoleManager<ApplicationRole> roleManager,
            RoleFieldServices roleFieldServices,
            PermissionServices permissionServices,
            ILanguageService languageService,
            ICacheManager<object> cacheManager)
        {
            _roleFieldServices = roleFieldServices;
            _roleManager = roleManager;
            _permissionServices = permissionServices;
            _languageService = languageService;
            _cacheManager = cacheManager;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }

        [HttpGet]
        public IActionResult GetEntitys()
        {
            var entitys = FieldScannerServices.GetEntitys(_languageService);
            return Ok(entitys);
        }
        /// <summary>
        /// 获取类下所有字段
        /// </summary>
        /// <param name="name">类名</param>
        /// <param name="fullName">命名空间</param>
        /// <param name="roleName">角色id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetFields(string name, string fullName, string roleName)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("类名不能为空");
            if (string.IsNullOrEmpty(fullName)) throw new ArgumentNullException("命名空间不能为空");
            var fields = FieldScannerServices.GetClassFields(name, fullName, _languageService);
            var role = await _roleManager.FindByNameAsync(roleName);
            fields = await _roleFieldServices.GetRoleFields(fields, fullName, new List<int> { role.Id });
            return Ok(fields);
        }

        [HttpGet]
        public IActionResult SetCookieLanguage(string acceptLanguage, string returnUrl)
        {
            var idCookiaName = CurrencyConstant.LanguageHeader;
            var idCookieOptions = new CookieOptions
            {
                Path = "/",
                Secure = false,
                IsEssential = true,
                SameSite = SameSiteMode.Strict,
                Expires = CommonHelper.GetSysDateTimeNow().AddYears(100),
            };
            HttpContext.Response.Cookies.Append(
                key: idCookiaName,
                value: acceptLanguage,
                options: idCookieOptions);
            _cacheManager.Clear();
            return Redirect(returnUrl);
        }
        /// <summary>
        /// 保存角色字段权限
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveRoleFields(List<FieldsView> fields, string roleName)
        {
            fields = await _roleFieldServices.SavRoleFields(fields, roleName);
            return Ok(fields);
        }


        [HttpGet]
        public async Task<IActionResult> GetPermissionMenus(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            var permissions = await _permissionServices.GetPermissionsAsync(new List<int>() { role.Id }, u => u.PermissionType == (int)PermissionType.RoleMenus);
            var permissionUrls = _permissionServices.GetPermissionsAsync(permissions);
            var menus = await _permissionServices.GetPermissionMenus(permissionUrls);
            return Ok(menus);
        }

        [HttpPost]
        public async Task<IActionResult> SavePermissionMenus(string roleName, List<string> urls)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            var count = await _permissionServices.SavePermissionMenusAsync(role.Id, urls);
            return Ok(title: $"成功修改{count}条权限");
        }
    }
}
