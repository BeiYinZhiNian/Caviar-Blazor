﻿// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Threading.Tasks;
using Caviar.Core.Services;
using Caviar.SharedKernel.Common;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Caviar.Infrastructure.API
{
    public class ServerAuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILanguageService _languageService;
        private readonly LogServices<ServerAuthService> _logServices;
        private readonly IInteractor _interactor;
        private readonly UserServices _userServices;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ServerAuthService(IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager, ILanguageService languageService,
            LogServices<ServerAuthService> logServices,
            IInteractor interactor, UserServices userServices, SignInManager<ApplicationUser> signInManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _languageService = languageService;
            _logServices = logServices;
            _interactor = interactor;
            _userServices = userServices;
            _signInManager = signInManager;
        }

        public async Task<CurrentUser> CurrentUserInfo()
        {
            var currentUser = await _userServices.GetCurrentUserInfoAsync(_httpContextAccessor.HttpContext.User);
            return await Task.FromResult(currentUser);
        }

        public async Task<ResultMsg> Login(UserLogin loginRequest, string returnUrl)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.UserName);
            if (user == null) return new ResultMsg() { Title = _languageService[$"{CurrencyConstant.ResuleMsg}.Failed"], Status = System.Net.HttpStatusCode.Unauthorized };
            var singInResult = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, true);
            if (singInResult.Succeeded)
            {
                var token = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, "SignIn");

                var data = $"{user.Id}|{token}";

                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    data += $"|{returnUrl}";
                }
                _interactor.UserInfo = user;
                _logServices.Infro($"登录成功：{singInResult}");
                return new ResultMsg() { Title = _languageService[$"{CurrencyConstant.ResuleMsg}.Login Succeeded"], Url = $"/{CurrencyConstant.Api}{UrlConfig.SignInActual}?t=" + Uri.EscapeDataString(data) };
            }
            else
            {
                _logServices.Infro($"登录失败:{singInResult}");
                return new ResultMsg() { Title = _languageService[$"{CurrencyConstant.ResuleMsg}.{singInResult}"], Status = System.Net.HttpStatusCode.Unauthorized };
            }
        }


        public Task<string> Logout()
        {
            _logServices.Infro($"退出登录");
            return Task.FromResult("/" + CurrencyConstant.Api + UrlConfig.LogoutServer);
        }
    }
}
