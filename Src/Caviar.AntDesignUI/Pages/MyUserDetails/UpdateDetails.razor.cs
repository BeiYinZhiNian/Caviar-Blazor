using AntDesign;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Pages.MyUserDetails
{
    public partial class UpdateDetails
    {

        bool loading = false;
        [Parameter]
        public UserDetails UserDetails { get; set; }
        [Inject]
        MessageService _message { get; set; }
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
    }
}
