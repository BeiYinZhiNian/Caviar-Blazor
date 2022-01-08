using AntDesign;
using Caviar.SharedKernel;
using Caviar.SharedKernel.Entities.User;
using Caviar.SharedKernel.Interface;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Helper
{
    public class HostAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IAuthService api;
        private CurrentUser _currentUser;
        MessageService _message;
        public HostAuthenticationStateProvider(IAuthService api, MessageService messageService)
        {
            this.api = api;
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
                        .Concat(_currentUser.Claims.Select(c => new Claim(c.Key, c.Value)));

                    identity = new ClaimsIdentity(claims, nameof(HostAuthenticationStateProvider));
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Request failed:" + ex.ToString());
            }
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        private async Task<CurrentUser> GetCurrentUser()
        {
            if (_currentUser != null && _currentUser.IsAuthenticated) return _currentUser;
            _currentUser = await api.CurrentUserInfo();
            return _currentUser;
        }

        public async Task<string> Logout()
        {
            var result = await api.Logout();
            _currentUser = null;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return result;
        }

        public async Task<ResultMsg> Login(UserLogin loginParameters, string returnUrl)
        {
            var result = await api.Login(loginParameters, returnUrl);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            if(result.Status != HttpStatusCode.OK)
            {
                _ = _message.Error(result.Title);
            }
            return result;
        }
    }
}
