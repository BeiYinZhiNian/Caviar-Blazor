using AntDesign;
using Caviar.Models.SystemData;
using Caviar.AntDesignPages.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Caviar.Models.SystemData;
/// <summary>
/// 生成者：未登录用户
/// 生成时间：2021/5/28 12:37:58
/// 代码由代码生成器自动生成，更改的代码可能被进行替换
/// 可在上层目录使用partial关键字进行扩展
/// </summary>
namespace Caviar.AntDesignPages.Pages.Role
{
    [DisplayName("数据模板")]
    public partial class DataTemplate: ITableTemplate
    {
        
        [Parameter]
        public ViewRole DataSource { get; set; }
        
        [Parameter]
        public string Url { get; set; }

        [Parameter]
        public string SuccMsg { get; set; } = "操作成功";

        [Inject]
        HttpHelper Http { get; set; }

        private Form<ViewRole> _meunForm;
        protected override async Task OnInitializedAsync()
        {

        }
        [Inject]
        MessageService _message { get; set; }
        [Parameter]
        public bool Visible { get; set; }
        public async Task<bool> Submit()
        {
            //数据效验
            if(_meunForm.Validate())
            {
                return await FormSubmit();
            }
            return false;
        }

        async Task<bool> FormSubmit()
        {
            var result = await Http.PostJson<ViewRole, object>(Url, DataSource);
            if (result.Status == 200)
            {
                _message.Success(SuccMsg);
                return true;
            }
            return false;
        }
    }
}
