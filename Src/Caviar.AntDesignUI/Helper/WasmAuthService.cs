using Caviar.SharedKernel;
using Caviar.SharedKernel.Entities.User;
using Caviar.SharedKernel.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Helper
{
    public class WasmAuthService : IAuthService
    {
        private readonly HttpHelper httpHelper;

        public WasmAuthService(HttpHelper httpClient)
        {
            this.httpHelper = httpClient;
        }

        public async Task<CurrentUser> CurrentUserInfo()
        {
            var result = await httpHelper.GetJson<CurrentUser>(Config.PathList.CurrentUserInfo);
            if (result.Status != StatusCodes.Status200OK) return null;
            return result.Data;
        }

        public async Task<ResultMsg> Login(UserLogin loginRequest, string returnUrl)
        {
            var result = await httpHelper.PostJson(Config.PathList.Login + "?returnUrl=" + returnUrl, loginRequest);
            return result;
        }

        public async Task<string> Logout()
        {
            var result = await httpHelper.PostJson<string,string>(Config.PathList.Logout, null);
            return result.Title;
        }
    }
}
