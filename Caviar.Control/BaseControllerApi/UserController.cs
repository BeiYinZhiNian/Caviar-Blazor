using Caviar.Control;
using Caviar.Control.UserGroup;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Caviar.Control.User
{
    public partial class UserController
    {

        [HttpPost]
        public IActionResult Login(ViewUser userLogin)
        {
            var userAction = CreateModel<UserAction>();
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
        public IActionResult Register(ViewUser userLogin)
        {
            var userAction = CreateModel<UserAction>();
            userAction.Entity = userLogin;
            var IsRegister = userAction.Register(out string msg);
            if (IsRegister)
            {
                return ResultOK(msg);
            }
            return ResultError(msg);
        }
    }
}
