using Caviar.Core;
using Caviar.Core.UserGroup;
using Caviar.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Caviar.Core.User
{
    public partial class UserController
    {
        [HttpGet]
        public async Task<IActionResult> MyDetails()
        {
            var result = await _Action.GetEntity(Interactor.UserToken.Id);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> MyDetails(ViewUser viewUser)
        {
            var result = await _Action.UpateMyData(viewUser);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Login(ViewUser eneity)
        {
            var result = await _Action.Login(eneity);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePwd(UserPwd userPwd)
        {
            var result = await _Action.UpdatePwd(userPwd);
            return Ok(result);
        }
    }
}
