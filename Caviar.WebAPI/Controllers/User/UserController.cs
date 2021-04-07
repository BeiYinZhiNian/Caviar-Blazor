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
        public IActionResult Login(string userName, string phoneNumber, string password)
        {
            var user = CreateModel<SysUserLoginAction>();
            user.UserName = userName;
            user.PhoneNumber = phoneNumber;
            user.Password = password;
            var msg = user.Login();
            LoggerMsg<UserController>(msg, IsSucc: ControllerModel.SysUserInfo.IsLogin);
            if (ControllerModel.SysUserInfo.IsLogin)
            {
                return ResultOK(msg);
            }
            return ResultError(403, msg);
        }

        [HttpPost]
        public IActionResult Register(SysUserLoginAction userLogin)
        {
            var IsRegister = userLogin.Register(out string msg);
            LoggerMsg<UserController>(msg, IsSucc: IsRegister);
            if (IsRegister)
            {
                return ResultOK(msg);
            }
            return ResultError(400, msg);
        }

    }
}
