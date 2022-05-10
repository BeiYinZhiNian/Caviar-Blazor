using Caviar.Core.Services;
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
    public partial class ApplicationRoleController
    {
        private readonly RoleServices _roleServices;
        public ApplicationRoleController(RoleManager<ApplicationRole> roleManager, RoleServices roleServices)
        {
            _roleServices = roleServices;
        }

        [HttpGet]
        public async Task<IActionResult> RoleFindById(int id)
        {
            var result = await _roleServices.RoleFindById(id);
            return Ok(result);
        }

        [HttpPost]
        public override async Task<IActionResult> CreateEntity(ApplicationRoleView vm)
        {
            var result = await _roleServices.CreateUserAsync(vm);
            if (result.Succeeded) return Ok();
            return Error("创建角色失败", result);
        }

        [HttpPost]
        public override async Task<IActionResult> UpdateEntity(ApplicationRoleView vm)
        {
            vm.Entity.OperatorUp = User.Identity.Name;
            var result = await _roleServices.UpdateUserAsync(vm);
            if (result.Succeeded) return Ok();
            return Error("修改角色失败", result);
        }

        [HttpPost]
        public override async Task<IActionResult> DeleteEntity(ApplicationRoleView vm)
        {
            var result = await _roleServices.DeleteUserAsync(vm);
            if (result.Succeeded) return Ok();
            return Error("删除角色失败", result);
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
