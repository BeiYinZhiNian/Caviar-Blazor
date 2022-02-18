using Caviar.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Infrastructure.API
{
    public partial class SysUserGroupController
    {
        private UserGroupServices _userGroupServices;
        public SysUserGroupController(UserGroupServices userGroupServices)
        {
            _userGroupServices = userGroupServices; 
        }

        [HttpGet]
        public override async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, bool isOrder = true, bool isNoTracking = true)
        {
            var pages = await _userGroupServices.GetPageAsync(null, pageIndex, pageSize, isOrder, isNoTracking);
            return Ok(pages);
        }

    }
}
