using Caviar.Control;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Caviar.WebAPI.Controllers
{
    public class UserController : BaseController
    {
        [HttpPost]
        public IActionResult Login(SysUserLoginAction userLogin)
        {
            var msg = userLogin.Login();
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
