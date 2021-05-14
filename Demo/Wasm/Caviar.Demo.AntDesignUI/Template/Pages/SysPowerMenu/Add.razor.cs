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
using Caviar.Demo.Models;
/// <summary>
/// 生成者：未登录用户
/// 生成时间：2021/5/14 15:46:09
/// 代码由代码生成器自动生成，更改的代码可能被进行替换
/// 可在上层目录使用partial关键字进行扩展
/// </summary>
namespace Caviar.Demo.AntDesignUI
{
    public partial class Add: ITableTemplate
    {
        private Form<ViewSysPowerMenu> _meunForm;
        public ViewSysPowerMenu model = new ViewSysPowerMenu() { Number = "999" };
        [Inject]
        HttpHelper Http { get; set; }
        [Inject]
        MessageService _message { get; set; }

        public async Task<bool> Submit()
        {
            if(_meunForm.Validate())
            {
                return await FormSubmit();
            }
            return false;
        }

        async Task<bool> FormSubmit()
        {
            var result = await Http.PostJson<SysPowerMenu, object>("SysPowerMenu/AddEntity", model);
            if (result.Status == 200)
            {
                _message.Success("创建成功");
                return true;
            }
            return false;
        }
    }
}
