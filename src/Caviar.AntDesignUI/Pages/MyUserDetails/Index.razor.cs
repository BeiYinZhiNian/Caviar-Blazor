// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Threading.Tasks;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components;

namespace Caviar.AntDesignUI.Pages.MyUserDetails
{
    public partial class Index
    {
        [Inject]
        HttpService HttpService { get; set; }
        [Inject]
        ILanguageService LanguageService { get; set; }

        string Roles { get; set; }

        UserDetails _user;
        protected override async Task OnInitializedAsync()
        {
            var result = await HttpService.GetJson<UserDetails>(UrlConfig.MyDetails);
            if (result.Status == System.Net.HttpStatusCode.OK)
            {
                _user = result.Data;
                foreach (var item in _user.Roles)
                {
                    Roles += item + ";";
                }
            }
            await base.OnInitializedAsync();
        }
    }
}
