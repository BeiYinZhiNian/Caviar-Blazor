// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Threading.Tasks;
using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Caviar.AntDesignUI.Pages.Enclosure
{
    public partial class SysEnclosureIndex
    {
        [Inject]
        HttpService HttpService { get; set; }
        [Inject]
        MessageService MessageService { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        protected override Task OnInitializedAsync()
        {
            TableOptions.CreateButtons = CreateButtons;
            return base.OnInitializedAsync();
        }
        protected override async Task RowCallback(RowCallbackData<SysEnclosureView> row)
        {
            switch (row.Menu.Entity.MenuName)
            {
                //case "Menu Key"
                case CurrencyConstant.DownloadKey:
                    var result = await HttpService.PostJson<SysEnclosureView, string>(UrlConfig.Download, row.Data);
                    if (result.Status == System.Net.HttpStatusCode.OK)
                    {
                        _ = JSRuntime.InvokeVoidAsync("open_blank", result.Data);
                    }
                    break;
            }
            await base.RowCallback(row);
        }

        void OnSingleCompleted(UploadInfo fileinfo)
        {
            if (fileinfo.File.State == UploadState.Success)
            {
                var result = fileinfo.File.GetResponse<ResultMsg>();
                if (result.Status == System.Net.HttpStatusCode.OK)
                {
                    MessageService.Success(result.Title);
                    Refresh();
                }
                else
                {
                    MessageService.Error(result.Title);
                }
            }
        }

    }
}
