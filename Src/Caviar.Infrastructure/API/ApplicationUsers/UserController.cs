using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Infrastructure.API
{
    public partial class ApplicationUserController
    {

        [HttpPost]
        public override async Task<IActionResult> CreateEntity(ApplicationUserView vm)
        {
            if (string.IsNullOrEmpty(vm.Entity.PasswordHash))
            {
                vm.Entity.PasswordHash = CommonHelper.SHA256EncryptString(CurrencyConstant.DefaultPassword);
            }
            var result = await _userManager.CreateAsync(vm.Entity,vm.Entity.PasswordHash);
            if (result.Succeeded) return Ok("创建用户成功，初始密码：" + CurrencyConstant.DefaultPassword);
            return Error("创建用户失败", result);
        }

        [HttpPost]
        public override async Task<IActionResult> UpdateEntity(ApplicationUserView vm)
        {
            var result = await _userManager.UpdateAsync(vm.Entity);
            if (result.Succeeded) return Ok();
            return Error("修改用户失败", result);
        }

        [HttpPost]
        public override async Task<IActionResult> DeleteEntity(ApplicationUserView vm)
        {
            var result = await _userManager.DeleteAsync(vm.Entity);
            if (result.Succeeded) return Ok();
            return Error("删除用户失败", result);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoles(IList<string> roles)
        {
            var result = await UserServices.AssignRoles(roles);
            if (result.Succeeded) return Ok();
            return Error("角色分配失败", result);
        }
        [HttpGet]
        public async Task<IActionResult> GetCurrentRoles()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }

        private IActionResult Error(string title, IdentityResult result)
        {
            ResultMsg resultMsg = new ResultMsg()
            {
                Title = title,
                Status = System.Net.HttpStatusCode.BadRequest,
                Detail = new Dictionary<string, string>(),
            };
            foreach (var item in result.Errors)
            {
                resultMsg.Detail.Add(item.Code, item.Description);
            }
            return Ok(resultMsg);
        }
    }
}
