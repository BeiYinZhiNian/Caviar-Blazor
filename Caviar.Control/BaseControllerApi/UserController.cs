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
        [HttpGet]
        public async Task<IActionResult> MyDetails()
        {
            var result = await _Action.GetEntity(BC.UserToken.Id);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> MyDetails(ViewUser viewUser)
        {
            if (viewUser.Id != BC.UserToken.Id || viewUser.Uid != BC.UserToken.Uid)
            {
                return ResultError("正在进行非法修改");
            }
            var result = await _Action.UpdateEntity();
            return Ok(result);
        }

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

        [HttpPost]
        public async Task<IActionResult> UpdatePwd(UserPwd userPwd)
        {
            var result = await _Action.UpdatePwd(userPwd);
            return ResultOK(result);
        }
    }
}
