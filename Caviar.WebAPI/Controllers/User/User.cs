using Caviar.Control;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.WebAPI.Controllers
{
    public class User : BaseController
    {
        [HttpGet]
        public IActionResult Login()
        {
            var str = IDataContext.GetAllAsync<SysUserLogin>();
            Base_Logger.LogDebug("测试11");
            Base_Logger.LogInformation("测试2");
            Base_Logger.LogWarning("测试3");
            Base_Logger.LogTrace("测试4");
            Base_Logger.LogCritical("测试5");
            Base_Logger.LogError("测试");
            return ResultOK("测试错误");
        }


    }
}
