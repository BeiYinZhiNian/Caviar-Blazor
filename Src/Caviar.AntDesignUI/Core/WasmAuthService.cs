using Caviar.SharedKernel.Entities.View;
using Caviar.SharedKernel.Entities.User;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using Caviar.SharedKernel.Entities;

namespace Caviar.AntDesignUI.Core
{
    public class WasmAuthService : IAuthService
    {
        private readonly HttpService httpHelper;

        public WasmAuthService(HttpService httpClient)
        {
            this.httpHelper = httpClient;
        }

        public async Task<CurrentUser> CurrentUserInfo()
        {
            var result = await httpHelper.GetJson<CurrentUser>(UrlConfig.CurrentUserInfo);
            if (result.Status != HttpStatusCode.OK) return null;
            return result.Data;
        }

        public async Task<ResultMsg> Login(UserLogin loginRequest, string returnUrl)
        {
            var result = await httpHelper.PostJson(UrlConfig.Login + "?returnUrl=" + returnUrl, loginRequest);
            return result;
        }

        public async Task<string> Logout()
        {
            var result = await httpHelper.PostJson<string,string>(UrlConfig.Logout, null);
            return result.Url;
        }
    }
}
