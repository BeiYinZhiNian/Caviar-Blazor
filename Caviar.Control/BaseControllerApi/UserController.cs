using Caviar.Control;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Caviar.Control
{
    public partial class UserController : CaviarBaseController
    {
        [HttpPost]
        public IActionResult Login(ViewUserLogin userLogin)
        {
            var userAction = CreateModel<UserLoginAction>();
            userAction.Entity = userLogin;
            var loginMsg = userAction.Login();
            if (BC.IsLogin)
            {
                ResultMsg.Data = BC.UserToken;
                return ResultOK("登录成功，欢迎回来");
            }
            return ResultError(loginMsg);
        }

        [HttpPost]
        public IActionResult Register(ViewUserLogin userLogin)
        {
            var userAction = CreateModel<UserLoginAction>();
            userAction.Entity = userLogin;
            var IsRegister = userAction.Register(out string msg);
            LoggerMsg<UserController>(msg, IsSucc: IsRegister);
            if (IsRegister)
            {
                return ResultOK(msg);
            }
            return ResultError(msg);
        }

    }
}
