using AntDesign;
using Caviar.SharedKernel;
using Caviar.AntDesignUI.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace Caviar.AntDesignUI.Pages.User
{
    partial class Login
    {
        bool Loading { get; set; }
        public SysUser SysLoginUserData { get; set; } = new SysUser(){ UserName = "admin" , Password = "123456"};
        [Inject] public NavigationManager NavigationManager { get; set; }

        [Inject]
        MessageService _message { get; set; }

        [Inject]
        IConfiguration Configuration { get; set; }

        [CascadingParameter]
        public EventCallback LayoutStyleCallBack { get; set; }
        [Inject]
        HttpHelper http { get; set; }
        [Inject]
        ViewUserToken UserToken { get; set; }
        [Inject]
        IJSRuntime JsRuntime { get; set; }
        public async void SubmitLogin()
        {
            Loading = true;
            SysLoginUserData.Password = CommonHelper.SHA256EncryptString(SysLoginUserData.Password);
            var result = await http.PostJson<SysUser, ViewUserToken>("User/Login",SysLoginUserData);
            SysLoginUserData.Password = "";
            Loading = false;
            if (result.Status==200)
            {
                UserToken.AutoAssign(result.Data);
                await JsRuntime.InvokeVoidAsync("setCookie", Config.CookieName, UserToken.Token, UserToken.Duration);
                http.IsSetCookie = false;//更新cookies
                NavigationManager.NavigateTo("/");
                _message.Success(result.Title);
                return;
            }
            this.StateHasChanged();
        }

        protected override void OnInitialized()
        {
            string backgroundImage = "_content/Caviar.AntDesignUI/Images/e613f3b11ffd2a7c9db467cd25a694c8.jpeg";
            var style = $"min-height:100vh;background-image: url({backgroundImage});";
            LayoutStyleCallBack.InvokeAsync(style);
            base.OnInitialized();
        }

        
    }
}
