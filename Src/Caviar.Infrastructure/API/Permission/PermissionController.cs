using Caviar.Core;
using Caviar.Core.Services.PermissionServices;
using Microsoft.AspNetCore.Mvc;
using Caviar.Infrastructure.API.BaseApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caviar.SharedKernel;

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
            var entitys = FieldScannerServices.GetEntitys();
            foreach (var item in entitys)
            {
                item.Entity.DisplayName = LanguageService[$"{CurrencyConstant.MenuBar}.{item.Entity.FieldName}"];
            }
            return Ok(entitys);
        }
    }
}
