using Caviar.Infrastructure.API.BaseApi;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Infrastructure.API.WebConfig
{
    public class WebConfig: BaseApiController
    {
        /// <summary>
        /// 获取布局
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult LayoutSettings()
        {
            return Ok();
        }
        /// <summary>
        /// 保存布局
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveLayoutSettings(string name)
        {
            return Ok();
        }
    }
}
