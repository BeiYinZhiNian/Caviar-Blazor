// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Threading.Tasks;
using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Caviar.AntDesignUI.Pages.MyUserDetails
{
    public partial class UpdateDetails
    {
        bool loading = false;
        [Parameter]
        public UserDetails UserDetails { get; set; }
        [Inject]
        MessageService _message { get; set; }
        [Inject]
        HttpService HttpService { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        [Inject]
        UserConfig UserConfig { get; set; }

        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }
        bool BeforeUpload(UploadFileItem file)
        {
            var isJpgOrPng = file.Type == "image/jpeg" || file.Type == "image/png";
            if (!isJpgOrPng)
            {
                _message.Error("You can only upload JPG/PNG file!");
            }
            var isLt2M = file.Size / 1024 / 1024 < 3;
            if (!isLt2M)
            {
                _message.Error("Image must smaller than 3MB!");
            }
            return isJpgOrPng && isLt2M;
        }

        void OnSingleCompleted(UploadInfo fileinfo)
        {
            if (fileinfo.File.State == UploadState.Success)
            {
                var result = fileinfo.File.GetResponse<ResultMsg<SysEnclosure>>();
                if (result.Status == System.Net.HttpStatusCode.OK)
                {
                    UserDetails.HeadPortrait = result.Data.FilePath;
                }
                else
                {
                    _message.Error(result.Title);
                }
            }
        }

        async Task FormSubmit()
        {
            var result = await HttpService.PostJson(UrlConfig.UpdateDetails, UserDetails);
            if (result.Status == System.Net.HttpStatusCode.OK)
            {
                _ = _message.Success(result.Title);
                NavigationManager.NavigateTo(JSRuntime, UrlConfig.Home, true);
            }
        }
    }
}
