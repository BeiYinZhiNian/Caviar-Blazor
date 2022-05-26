// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace Caviar.AntDesignUI.Pages.Setting
{
    public partial class LayoutSettings
    {
        [Inject]
        UserConfig UserConfig { get; set; }
        [Inject]
        CavLayout CavLayout { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        [Inject]
        ILanguageService LanguageService { get; set; }
        [Inject]
        MessageService MessageService { get; set; }
        private async void Preservation()
        {
            var json = JsonConvert.SerializeObject(CavLayout);
            await JSRuntime.InvokeVoidAsync(CurrencyConstant.SetCookie, CurrencyConstant.LayoutTheme, json, 999);
            _ = MessageService.Success("主题保存成功");
        }
    }
}
