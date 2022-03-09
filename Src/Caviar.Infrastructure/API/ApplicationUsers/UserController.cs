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
            if (result.Succeeded) return Ok(title: "创建用户成功，初始密码：" + CurrencyConstant.DefaultPassword);
            return Error("创建用户失败", result);
        }

        [HttpPost]
        public override async Task<IActionResult> UpdateEntity(ApplicationUserView vm)
        {
            var user = await _userManager.FindByNameAsync(vm.Entity.UserName);
            if (user == null) throw new ArgumentNullException($"{vm.Entity.UserName}不存在");
            user.UserName = vm.Entity.UserName;
            user.PhoneNumber = vm.Entity.PhoneNumber;
            user.Email = vm.Entity.Email;
            user.UserGroupId = vm.Entity.UserGroupId;
            user.IsDisable = vm.Entity.IsDisable;
            user.Number = vm.Entity.Number;
            user.Remark = vm.Entity.Remark;
            user.UpdateTime = DateTime.Now;
            user.OperatorUp = User.Identity.Name;
            var result = await _userManager.UpdateAsync(user);
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
        public async Task<IActionResult> AssignRoles(string userName,IList<string> roles)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("请选择用户");
            }
            var result = await UserServices.AssignRoles(userName,roles);
            if (result.Succeeded) return Ok();
            return Error("角色分配失败", result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserRoles(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("请选择用户");
            }
            var user = await UserServices.GetUserInfo(userName);
            var roles = await UserServices.GetRoles(user);
            return Ok(roles);
        }
        /// <summary>
        /// 获取自己的用户详情
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> MyUserDetails()
        {
            var user = await UserServices.GetUserDetails(User.Identity.Name);
            return Ok(user);
        }
        /// <summary>
        /// 修改基本信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateDetails(UserDetails userDetails)
        {
            var result = await UserServices.UpdateUserDetails(User.Identity.Name,userDetails);
            if (result.Succeeded) return Ok();
            return Error("修改基础信息失败", result);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel changePassword)
        {
            var user = await _userManager.GetUserAsync(User);
            var result = await _userManager.ChangePasswordAsync(user, changePassword.OldPassword, changePassword.NewPassword);
            if (result.Succeeded) return Ok();
            return Error("修改密码失败", result);
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
