using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Pages.User
{
    public partial class ChangePassword: ITableTemplate
    {
        Form<ChangePasswordModel> _Form;
        public ChangePasswordModel ChangePasswordModel { get; set; } = new ChangePasswordModel();

        [Parameter]
        public string SubmitUrl {get;set;}
        [Inject]
        public HttpService HttpService { get; set; }
        [Inject]
        public MessageService MessageService { get; set; }
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
            if(ChangePasswordModel.ConfirmPassword != ChangePasswordModel.NewPassword)
            {
                _ = MessageService.Error("两次密码输入不一致");
                return false;
            }
            ChangePasswordModel.NewPassword = CommonHelper.SHA256EncryptString(ChangePasswordModel.NewPassword);
            ChangePasswordModel.ConfirmPassword = CommonHelper.SHA256EncryptString(ChangePasswordModel.ConfirmPassword);
            ChangePasswordModel.OldPassword = CommonHelper.SHA256EncryptString(ChangePasswordModel.OldPassword);
            var result = await HttpService.PostJson(SubmitUrl, ChangePasswordModel);
            if(result.Status == System.Net.HttpStatusCode.OK)
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
