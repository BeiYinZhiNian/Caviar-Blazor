// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Threading.Tasks;
using Caviar.Core.Services;
using Caviar.Infrastructure.API.BaseApi;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Mvc;

namespace Caviar.Infrastructure.API
{
    public class SysLogController : BaseApiController
    {

        private readonly LogDataServices _logDataServices;
        public SysLogController(LogDataServices services)
        {
            _logDataServices = services;
        }

        /// <summary>
        /// 只能获取自身字段
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual async Task<IActionResult> GetFields()
        {
            var fields = await GetFields<SysLog>();
            return Ok(fields);
        }

        [HttpGet]
        public virtual async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, bool isOrder = true)
        {
            var pages = await _logDataServices.GetPageAsync(null, pageIndex, pageSize, isOrder);
            return Ok(pages);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Query(QueryView query)
        {
            var page = await _logDataServices.QueryAsync(query);
            return Ok(page);
        }
    }
}
