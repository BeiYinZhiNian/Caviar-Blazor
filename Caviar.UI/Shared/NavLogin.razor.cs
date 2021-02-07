using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntDesign;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Components;

namespace Caviar.UI.Shared
{
    partial class NavLogin
    {
        public Sys_LoginUserData SysLoginUserData { get; set; } = new Sys_LoginUserData();
        [Inject] public NavigationManager NavigationManager { get; set; }

        [Inject]
        MessageService _message { get; set; }

        public void HandleSubmit()
        {
            if (SysLoginUserData.UserName == "admin" && SysLoginUserData.Password == "123456")
            {
                NavigationManager.NavigateTo("/");
                return;
            }
            else
            {
                _message.Error("登录失败，请检查用户名或密码");
            }
            
        }
    }
}
