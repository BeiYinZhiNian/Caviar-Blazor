// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Threading.Tasks;
using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components;

namespace Caviar.AntDesignUI.Pages.User
{
    public partial class ChangePassword : ITableTemplate
    {
        [Parameter]
        public string CurrentUrl { get; set; }
        Form<ChangePasswordModel> _Form;
        ChangePasswordModel ChangePasswordModel { get; set; } = new ChangePasswordModel();
        [Inject]
        HttpService HttpService { get; set; }
        [Inject]
        MessageService MessageService { get; set; }
        [Inject]
        UserConfig UserConfig { get; set; }
        [Inject]
        ILanguageService LanguageService { get; set; }
        public Task<bool> Validate()
        {
            if (_Form.Validate())
            {
                return FormSubmit();
            }
            return Task.FromResult(false);
        }

        FormValidateStatus ValidateStatus { get; set; }

        private async Task<bool> FormSubmit()
        {
            if (ChangePasswordModel.ConfirmPassword != ChangePasswordModel.NewPassword)
            {
                _ = MessageService.Error(LanguageService[$"{CurrencyConstant.ResuleMsg}.{CurrencyConstant.InconsistentPasswords}"]);
                return false;
            }
            ChangePasswordModel.NewPassword = CommonHelper.SHA256EncryptString(ChangePasswordModel.NewPassword);
            ChangePasswordModel.ConfirmPassword = CommonHelper.SHA256EncryptString(ChangePasswordModel.ConfirmPassword);
            ChangePasswordModel.OldPassword = CommonHelper.SHA256EncryptString(ChangePasswordModel.OldPassword);
            var result = await HttpService.PostJson(UrlConfig.ChangePassword, ChangePasswordModel);
            if (result.Status == System.Net.HttpStatusCode.OK)
            {
                _ = MessageService.Success(result.Title);
                return true;
            }
            ChangePasswordModel.NewPassword = null;
            ChangePasswordModel.ConfirmPassword = null;
            ChangePasswordModel.OldPassword = null;
            StateHasChanged();
            return false;
        }
    }
}
