using Caviar.Core;
using Caviar.Core.Services.PermissionServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Infrastructure.API.Permission
{
    public partial class PermissionController : BaseApiController
    {

        public PermissionController(PermissionServices permissionSdk)
        {

        }

        [HttpGet]
        public IActionResult GetEntitysasync()
        {
            var entitys = FieldScannerServices.GetClasss();
            return Ok(entitys);
        }
    }
}
