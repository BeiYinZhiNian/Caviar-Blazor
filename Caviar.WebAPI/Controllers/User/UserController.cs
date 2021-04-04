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
        public IActionResult Login(string nameOrPhone,string psw)
        {
            var user = CreatEntity<SysUserLoginAction>();
            user.UserName = nameOrPhone;
            user.Password = psw;
            var userInfo = user.Login();
            if(userInfo==null)
            {
                return ResultError(403,"用户名或密码错误");
            }
            return ResultOK("登录成功，欢迎回来");
        }

    }
}
