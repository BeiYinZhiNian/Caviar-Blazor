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
    public class UserController : BaseController
    {
        [HttpPost]
        public IActionResult Login(string userName,string phoneNumber,string password)
        {
            var user = CreateModel<SysUserLoginAction>();
            user.UserName = userName;
            user.PhoneNumber = phoneNumber;
            user.Password = password;
            var msg = user.Login();
            ControllerModel.GetLogger<UserController>().LogInformation($"用户：{user.UserName} 进行登录，手机号：{user.PhoneNumber}，登录消息：{msg}，登录结果：{ControllerModel.SysUserInfo.IsLogin}");
            if(ControllerModel.SysUserInfo.IsLogin)
            {
                return ResultOK(msg);
            }
            return ResultError(403,msg);
        }

    }
}
