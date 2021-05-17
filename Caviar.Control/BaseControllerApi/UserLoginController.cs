using Caviar.Control;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Caviar.Control
{
    public partial class UserLoginController : CaviarBaseController
    {
        [HttpPost]
        public IActionResult Login(UserLoginAction userLogin)
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
        public IActionResult Register(UserLoginAction userLogin)
        {
            var IsRegister = userLogin.Register(out string msg);
            LoggerMsg<UserLoginController>(msg, IsSucc: IsRegister);
            if (IsRegister)
            {
                return ResultOK(msg);
            }
            return ResultErrorMsg(msg);
        }

    }
}
