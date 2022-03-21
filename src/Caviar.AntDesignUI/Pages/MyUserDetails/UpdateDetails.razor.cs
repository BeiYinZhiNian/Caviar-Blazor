using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Pages.MyUserDetails
{
    public partial class UpdateDetails
    {
        private FormValidationRule[] PhoneNumberRule;
        private FormValidationRule[] EmailRule;
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
            PhoneNumberRule = new FormValidationRule[]
            {
                new FormValidationRule() { Pattern = @"^1[3456789]\d{9}$", Message = UserConfig.LanguageService[$"{ CurrencyConstant.Page }.{ CurrencyConstant.PhoneNumberRuleErrorMsg}"] },
             };
            EmailRule = new FormValidationRule[]
            {
                new FormValidationRule() { Type = FormFieldType.Email, Required = true, Message = UserConfig.LanguageService[$"{ CurrencyConstant.Page }.{ CurrencyConstant.EmailRuleErrorMsg}"] },
             };
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
                if(result.Status == System.Net.HttpStatusCode.OK)
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
            if(result.Status == System.Net.HttpStatusCode.OK)
            {
                _ = _message.Success(result.Title);
                NavigationManager.NavigateTo(JSRuntime, UrlConfig.Home,true);
            }
        }
    }
}
