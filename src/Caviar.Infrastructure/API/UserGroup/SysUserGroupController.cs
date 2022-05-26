﻿// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Threading.Tasks;
using Caviar.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Caviar.Infrastructure.API
{
    public partial class SysUserGroupController
    {
        private readonly UserGroupServices _userGroupServices;
        public SysUserGroupController(UserGroupServices userGroupServices)
        {
            _userGroupServices = userGroupServices;
        }

        [HttpGet]
        public override async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, bool isOrder = true)
        {
            var pages = await _userGroupServices.GetPageAsync(null, pageIndex, pageSize, isOrder);
            return Ok(pages);
        }

    }
}
