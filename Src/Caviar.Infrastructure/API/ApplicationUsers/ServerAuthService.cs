using Caviar.SharedKernel;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.User;
using Caviar.SharedKernel.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Infrastructure.API
{
    public class ServerAuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILanguageService _languageService;

        public ServerAuthService(IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager,ILanguageService languageService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _languageService = languageService;
        }

        public Task<CurrentUser> CurrentUserInfo()
        {
            var user = _httpContextAccessor.HttpContext.User;
            return Task.FromResult(new CurrentUser
            {
                IsAuthenticated = user.Identity.IsAuthenticated,
                UserName = user.Identity.Name,
                Claims = user.Claims
                .ToDictionary(c => c.Type, c => c.Value)
            });
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
                return new ResultMsg() { Url = "/api/ApplicationUser/SignInActual?t=" + Uri.EscapeDataString(data) };
            }
            else
            {
                return new ResultMsg() { Title = _languageService["ResuleMsg.Title.Username and password are invalid"], Status = System.Net.HttpStatusCode.Unauthorized };
            }
        }


        public Task<string> Logout()
        {
            return Task.FromResult("/api/ApplicationUser/LogoutServer");
        }
    }
}
