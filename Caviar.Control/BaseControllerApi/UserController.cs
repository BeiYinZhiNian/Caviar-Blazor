using Caviar.Control;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Caviar.Control
{
    public class UserController : CaviarBaseController
    {
        [HttpPost]
        public IActionResult Login(SysUserLoginAction userLogin)
        {
            var loginMsg = userLogin.Login();
            if (BC.IsLogin)
            {
                ResultMsg.Data = BC.UserToken;
                return ResultOK("登录成功，欢迎回来");
            }
            return ResultErrorMsg(loginMsg);
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
            return ResultErrorMsg(msg);
        }

    }
}
