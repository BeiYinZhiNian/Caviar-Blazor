using Caviar.SharedKernel.Entities.View;
using Caviar.SharedKernel.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Caviar.SharedKernel.Entities;
using Caviar.Core.Services;

namespace Caviar.Infrastructure.API
{
    public class ServerAuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILanguageService _languageService;
        private readonly LogServices<ServerAuthService> _logServices;
        private readonly Interactor _interactor;

        public ServerAuthService(IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager,ILanguageService languageService,
            LogServices<ServerAuthService> logServices,
            Interactor interactor)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _languageService = languageService;
            _logServices = logServices;
            _interactor = interactor;
        }

        public Task<CurrentUser> CurrentUserInfo()
        {
            var user = _httpContextAccessor.HttpContext.User;
            var currentUser = new CurrentUser
            {
                IsAuthenticated = user.Identity.IsAuthenticated,
                UserName = user.Identity.Name,
                Claims = user.Claims.Select(u => new CaviarClaim(u))
            };
            return Task.FromResult(currentUser);
        }

        public async Task<ResultMsg> Login(UserLogin loginRequest, string returnUrl)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.UserName);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
                var token = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, "SignIn");

                var data = $"{user.Id}|{token}";

                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    data += $"|{returnUrl}";
                }
                _interactor.UserInfo = user;
                _interactor.UserName = loginRequest.UserName;
                _logServices.Infro($"登录成功");
                return new ResultMsg() {Title = _languageService[$"{CurrencyConstant.ResuleMsg}.Login Succeeded"], Url = $"/api/{UrlConfig.SignInActual}?t=" + Uri.EscapeDataString(data) };
            }
            else
            {
                _interactor.UserName = loginRequest.UserName;
                _logServices.Infro($"登录失败");
                return new ResultMsg() { Title = _languageService[$"{CurrencyConstant.ResuleMsg}.Username and password are invalid"], Status = System.Net.HttpStatusCode.Unauthorized };
            }
        }


        public Task<string> Logout()
        {
            _logServices.Infro($"退出登录");
            return Task.FromResult("/api/" + UrlConfig.LogoutServer);
        }
    }
}
