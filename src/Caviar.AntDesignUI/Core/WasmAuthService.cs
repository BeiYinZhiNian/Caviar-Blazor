// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Net;
using System.Threading.Tasks;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.User;

namespace Caviar.AntDesignUI.Core
{
    public class WasmAuthService : IAuthService
    {
        private readonly HttpService _httpHelper;

        public WasmAuthService(HttpService httpClient)
        {
            this._httpHelper = httpClient;
        }

        public async Task<CurrentUser> CurrentUserInfo()
        {
            var result = await _httpHelper.GetJson<CurrentUser>(UrlConfig.CurrentUserInfo);
            if (result.Status != HttpStatusCode.OK) return null;
            return result.Data;
        }

        public async Task<ResultMsg> Login(UserLogin loginRequest, string returnUrl)
        {
            var result = await _httpHelper.PostJson(UrlConfig.Login + "?returnUrl=" + returnUrl, loginRequest);
            return result;
        }

        public async Task<string> Logout()
        {
            var result = await _httpHelper.PostJson<string, string>(UrlConfig.Logout, null);
            return result.Url;
        }
    }
}
