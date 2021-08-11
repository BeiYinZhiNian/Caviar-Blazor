using AntDesign;
using Caviar.SharedKernel.View;
using Caviar.AntDesignUI.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Caviar.SharedKernel;
using Caviar.AntDesignUI.Base.Entitys;

namespace Caviar.AntDesignUI.Pages.User
{
    partial class Login
    {
        bool Loading { get; set; }
        public ViewUser SysLoginUserData { get; set; } = new ViewUser() { Entity = new UserEntity { UserName = "admin" } };

        public string Password { get; set; } = "123456";
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
            SysLoginUserData.Entity.PasswordHash = CommonHelper.SHA256EncryptString(Password);
            var result = await http.PostJson<ViewUser, ViewUserToken>("User/Login",SysLoginUserData);
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
