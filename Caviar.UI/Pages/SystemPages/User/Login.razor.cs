using AntDesign;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.UI.Pages.SystemPages.User
{
    partial class Login
    {
        public Sys_User_Login SysLoginUserData { get; set; } = new Sys_User_Login();
        [Inject] public NavigationManager NavigationManager { get; set; }

        [Inject]
        MessageService _message { get; set; }

        [Inject]
        IConfiguration Configuration { get; set; }

        [CascadingParameter]
        public EventCallback LayoutStyleCallBack { get; set; }

        public void SubmitLogin()
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

        protected override void OnInitialized()
        {
            string backgroundImage = Configuration["Background:Image"];
            var style = $"min-height:100vh;background-image: url({backgroundImage});";
            LayoutStyleCallBack.InvokeAsync(style);
            base.OnInitialized();
        }
    }
}
