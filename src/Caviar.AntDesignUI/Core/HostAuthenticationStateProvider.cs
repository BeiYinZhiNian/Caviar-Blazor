// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AntDesign;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.User;
using Microsoft.AspNetCore.Components.Authorization;

namespace Caviar.AntDesignUI.Core
{
    public class HostAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IAuthService _api;
        private CurrentUser _currentUser;
        MessageService _message;
        public HostAuthenticationStateProvider(IAuthService api, MessageService messageService)
        {
            _api = api;
            _message = messageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();
            try
            {
                var userInfo = await GetCurrentUser();
                if (userInfo.IsAuthenticated)
                {
                    var claims = new[] { new Claim(ClaimTypes.Name, _currentUser.UserName) }
                        .Concat(_currentUser.Claims.Select(u => new Claim(u.Type, u.Value)));

                    identity = new ClaimsIdentity(claims, nameof(HostAuthenticationStateProvider));
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Request failed:" + ex.ToString());
            }
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public async Task<CurrentUser> GetCurrentUser()
        {
            if (_currentUser != null && _currentUser.IsAuthenticated) return _currentUser;
            _currentUser = await _api.CurrentUserInfo();
            return _currentUser;
        }

        public async Task<string> Logout()
        {
            var result = await _api.Logout();
            _currentUser = null;
            if (!Config.IsServer)
            {
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            }
            return result;
        }

        public async Task<ResultMsg> Login(UserLogin loginParameters, string returnUrl)
        {
            var result = await _api.Login(loginParameters, returnUrl);
            if (Config.IsServer && result.Status != HttpStatusCode.OK)
            {
                _ = _message.Error(result.Title);
            }
            if (!Config.IsServer && result.Status == HttpStatusCode.OK)
            {
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            }
            return result;
        }
    }
}
