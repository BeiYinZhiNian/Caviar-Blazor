// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Threading.Tasks;
using Caviar.Core.Services;
using Caviar.Infrastructure.API.BaseApi;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.User;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Caviar.Infrastructure.API
{
    public partial class ApplicationUserController : EasyBaseApiController<ApplicationUserView, ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserServices _userServices;
        public ApplicationUserController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            UserServices userServices)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userServices = userServices;
        }


        [HttpPost]
        public virtual async Task<IActionResult> Login(UserLogin login, string returnUrl)
        {
            var user = await _userManager.FindByNameAsync(login.UserName);
            if (user == null) return Ok(System.Net.HttpStatusCode.Unauthorized, "Failed");
            var singInResult = await _signInManager.CheckPasswordSignInAsync(user, login.Password, true);
            if (!singInResult.Succeeded) return Ok(System.Net.HttpStatusCode.Unauthorized, "Failed");
            await _signInManager.SignInAsync(user, login.RememberMe);
            return Ok(title: "Login Succeeded", url: returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> SignInActual(string t)
        {
            var data = t;
            var parts = data.Split('|');

            var identityUser = await _userManager.FindByIdAsync(parts[0]);

            var isTokenValid = await _userManager.VerifyUserTokenAsync(identityUser, TokenOptions.DefaultProvider, "SignIn", parts[1]);

            if (isTokenValid)
            {
                await _signInManager.SignInAsync(identityUser, true);
                if (parts.Length == 3 && Url.IsLocalUrl(parts[2]))
                {
                    return Redirect(parts[2]);
                }
                return Redirect("/");
            }
            else
            {
                return Unauthorized("Validation failed");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(url: "/" + UrlConfig.Login);
        }

        [HttpGet]
        public async Task<IActionResult> LogoutServer()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/" + UrlConfig.Login);
        }

        [HttpGet]
        public async Task<IActionResult> CurrentUserInfo()
        {
            var currentUser = await _userServices.GetCurrentUserInfoAsync(User);
            return Ok(currentUser);
        }

    }
}
