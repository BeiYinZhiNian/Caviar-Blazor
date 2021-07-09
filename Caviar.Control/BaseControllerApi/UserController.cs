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
            var result = await _Action.UpdateEntity(viewUser);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Login(ViewUser eneity)
        {
            var result = _Action.Login(eneity);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Register(ViewUser eneity)
        {
            var ruslut = _Action.Register(eneity);
            return Ok(ruslut);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePwd(UserPwd userPwd)
        {
            var result = await _Action.UpdatePwd(userPwd);
            return Ok(result);
        }
    }
}
