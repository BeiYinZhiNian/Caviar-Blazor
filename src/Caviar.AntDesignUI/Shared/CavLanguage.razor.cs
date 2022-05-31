// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Collections.Generic;
using System.Threading.Tasks;
using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace Caviar.AntDesignUI.Shared
{
    public partial class CavLanguage
    {
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        [Inject]
        ILanguageService LanguageService { get; set; }
        NavLinkMatch RouterMatch { get; set; } = NavLinkMatch.Prefix;
        List<(string CultureName, string ResourceName)> LanguageList { get; set; }
        protected override Task OnInitializedAsync()
        {
            LanguageList = LanguageService.GetLanguageList();
            return base.OnInitializedAsync();
        }

        public void SelectLanguage(MenuItem menuItem)
        {
            NavigationManager.NavigateTo(JSRuntime, $"{CurrencyConstant.Api}{UrlConfig.SetCookieLanguage}?acceptLanguage={menuItem.Key}&returnUrl=/", true);
        }
    }
}
