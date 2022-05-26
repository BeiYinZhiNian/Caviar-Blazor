// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Threading.Tasks;
using Caviar.SharedKernel.Entities.User;

namespace Caviar.SharedKernel.Entities
{
    public interface IAuthService
    {
        Task<ResultMsg> Login(UserLogin userLogin, string returnUrl);
        Task<string> Logout();
        Task<CurrentUser> CurrentUserInfo();
    }
}
