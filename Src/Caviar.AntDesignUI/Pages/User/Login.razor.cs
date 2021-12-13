using AntDesign;
using Blazored.LocalStorage;
using Caviar.AntDesignUI.Helper;
using Caviar.SharedKernel;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
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
    public partial class Login
    {
        bool Loading { get; set; }
        public ApplicationUser ApplicationUser { get; set; } = new ApplicationUser() { UserName = "admin",PasswordHash= "1031622947@qq.COM" };
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
        ILocalStorageService localStorage { get; set; }

        [Inject]
        IJSRuntime JsRuntime { get; set; }
        public async void SubmitLogin()
        {
            Loading = true;
            ApplicationUserView applicationUser = new ApplicationUserView() { Entity = ApplicationUser };
            var result = await http.PostJson<ApplicationUserView, string>("ApplicationUser/Login", applicationUser);
            ApplicationUser.PasswordHash = "";
            Loading = false;
            if (result.Status == 200)
            {
                await localStorage.SetItemAsync("authToken", result.Title);
                NavigationManager.NavigateTo("/");
                await _message.Success("登录成功，欢迎回来");
                return;
            }
            this.StateHasChanged();
        }

        protected override void OnInitialized()
        {
            string backgroundImage = "_content/Caviar.AntDesignPages/Images/e613f3b11ffd2a7c9db467cd25a694c8.jpeg";
            var style = $"min-height:100vh;background-image: url({backgroundImage});";
            LayoutStyleCallBack.InvokeAsync(style);
            base.OnInitialized();
        }


    }
}
