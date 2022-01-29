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

namespace Caviar.Infrastructure.API.Permission
{
    public partial class PermissionController : BaseApiController
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        public PermissionController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
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
        /// <param name="roleId">角色id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetFields(string name,string fullName,int roleId)
        {
            var fields = FieldScannerServices.GetClassFields(name, fullName, LanguageService);
            var roleFieldServices = CreateService<RoleFieldServices>();
            fields = await roleFieldServices.GetRoleFields(fields, _roleManager, fullName, roleId);
            fields = fields.OrderBy(u => u.Entity.Number).ToList();
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
        public async Task<IActionResult> SaveRoleFields(List<ViewFields> fields, int roleId)
        {
            var roleFieldServices = CreateService<RoleFieldServices>();
            fields = await roleFieldServices.SavRoleFields(fields,_roleManager, roleId);
            return Ok(fields);
        }

    }
}
