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
        private RoleManager<ApplicationRole> _roleManager;
        public ApplicationRoleController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpPost]
        public override async Task<IActionResult> CreateEntity(ApplicationRoleView vm)
        {
            var reuslt = await _roleManager.CreateAsync(vm.Entity);
            if (reuslt.Succeeded)
            {
                return Ok();
            }
            ResultMsg msg = new ResultMsg()
            {
                Title = "角色创建失败",
                Status = System.Net.HttpStatusCode.BadRequest,
                Detail = new Dictionary<string, string>()
            };
            foreach (var item in reuslt.Errors)
            {
                msg.Detail.Add(item.Code, item.Description);
            }
            return Ok(msg);
        }
    }
}
