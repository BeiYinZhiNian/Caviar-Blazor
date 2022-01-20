using Caviar.Core;
using Caviar.Core.Services.PermissionServices;
using Microsoft.AspNetCore.Mvc;
using Caviar.Infrastructure.API.BaseApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caviar.SharedKernel.Entities.View;
using Caviar.Core.Services.ScannerServices;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Http;

namespace Caviar.Infrastructure.API.Permission
{
    public partial class PermissionController : BaseApiController
    {

        public PermissionController(PermissionServices permissionSdk)
        {

        }

        [HttpGet]
        public IActionResult GetEntitys()
        {
            var entitys = FieldScannerServices.GetEntitys(LanguageService);
            return Ok(entitys);
        }

        [HttpGet]
        public IActionResult GetFields(string name,string fullName,int roleId)
        {
            var fields = FieldScannerServices.GetClassFields(name, fullName, LanguageService);
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



    }
}
