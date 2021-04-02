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
            var user = CreatModel<SysUserLogin>();
            var str = Model.DataContext.GetAllAsync<SysUserLogin>();
            Model.Logger.LogDebug(Model.SysUserInfo.SysUserLogin.UserName + "测试11");
            Model.Logger.LogInformation(Model.SysUserInfo.SysUserLogin.UserName + "测试2");
            Model.Logger.LogWarning(Model.SysUserInfo.SysUserLogin.UserName + "测试3");
            Model.Logger.LogTrace(Model.SysUserInfo.SysUserLogin.UserName + "测试4");
            Model.Logger.LogCritical(Model.SysUserInfo.SysUserLogin.UserName + "测试5");
            Model.Logger.LogError(Model.SysUserInfo.SysUserLogin.UserName + "测试");
            return ResultOK(Model.SysUserInfo.SysUserLogin.UserName + "测试错误");
        }


    }
}
