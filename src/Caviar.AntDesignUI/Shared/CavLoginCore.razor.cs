// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Web;
using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.User;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Caviar.AntDesignUI.Shared
{
    public partial class CavLoginCore
    {
        [Inject]
        UserConfig UserConfig { get; set; }

        [Parameter]
        public UserLogin ApplicationUser { get; set; } = new UserLogin();

        [Inject]
        HostAuthenticationStateProvider AuthStateProvider { get; set; }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        MessageService MessageService { get; set; }
        [Inject]
        ILanguageService LanguageService { get; set; }

        bool Loading { get; set; } = true;

        Form<UserLogin> Form;

        public async void SubmitLogin()
        {
            if (!Form.Validate()) return;
            Loading = true;
            var returnUrl = HttpUtility.ParseQueryString(new Uri(NavigationManager.Uri).Query)["returnUrl"];
            if (returnUrl == null) returnUrl = "/";
            ApplicationUser.Password = CommonHelper.SHA256EncryptString(ApplicationUser.Password);
            var result = await AuthStateProvider.Login(ApplicationUser, returnUrl);
            Loading = false;
            if (result.Status == System.Net.HttpStatusCode.OK)
            {
                _ = MessageService.Success(result.Title);
                NavigationManager.NavigateTo(JSRuntime, result.Url);
                return;
            }
            ApplicationUser.Password = "";
            StateHasChanged();
        }
        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                Loading = false;
                StateHasChanged();
            }
            base.OnAfterRender(firstRender);
        }
    }
}
