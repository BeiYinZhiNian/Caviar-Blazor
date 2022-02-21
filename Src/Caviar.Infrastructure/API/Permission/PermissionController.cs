using Caviar.Core;
using Microsoft.AspNetCore.Mvc;
using Caviar.Infrastructure.API.BaseApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caviar.SharedKernel.Entities.View;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Http;
using Caviar.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Caviar.Infrastructure.API.Permission
{
    public partial class PermissionController : BaseApiController
    {
        private readonly RoleFieldServices _roleFieldServices;
        private readonly SysMenuServices _sysMenuServices;
        private readonly UserServices _userServices;
        public PermissionController(RoleManager<ApplicationRole> roleManager, 
            RoleFieldServices roleFieldServices,
            SysMenuServices sysMenuServices,
            UserServices userServices)
        {
            _roleFieldServices = roleFieldServices;
            _sysMenuServices = sysMenuServices;
            _userServices = userServices;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _sysMenuServices.PermissionUrls = PermissionUrls;
            base.OnActionExecuting(context);
        }

        [HttpGet]
        public IActionResult GetEntitys()
        {
            var entitys = FieldScannerServices.GetEntitys(LanguageService);
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
        public async Task<IActionResult> GetFields(string name,string fullName,string roleName)
        {
            var fields = FieldScannerServices.GetClassFields(name, fullName, LanguageService);
            fields = await _roleFieldServices.GetRoleFields(fields, fullName, new List<string> { roleName });
            return Ok(fields);
        }

        [HttpGet]
        public IActionResult SetCookieLanguage(string acceptLanguage, string returnUrl)
        {
            var idCookiaName = CurrencyConstant.LanguageHeader;
            var idCookieOptions = new CookieOptions
            {
                Path = "/",
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddYears(100),
            };
            HttpContext.Response.Cookies.Append(
                key: idCookiaName,
                value: acceptLanguage,
                options: idCookieOptions);
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
            var permissions = await _userServices.GetPermissions(new List<string>() { roleName }, u => u.PermissionType == PermissionType.RoleMenus);
            var permissionUrls = _userServices.GetPermissions(permissions);
            var menus = await _sysMenuServices.GetPermissionMenus(permissionUrls);
            return Ok(menus);
        }

        [HttpPost]
        public async Task<IActionResult> SavePermissionMenus(string roleName,List<string> urls)
        {
            var count = await _userServices.SavePermissionMenus(roleName, urls);
            return Ok(title:$"成功修改{count}条权限");
        }
    }
}
