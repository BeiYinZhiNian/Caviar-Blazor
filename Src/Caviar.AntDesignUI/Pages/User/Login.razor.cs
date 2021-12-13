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
        public ApplicationUser ApplicationUser { get; set; } = new ApplicationUser() { UserName = "admin",PasswordHash= "1031622947@qq.COM" };

        bool Loading = false;
        [CascadingParameter]
        public EventCallback LayoutStyleCallBack { get; set; }
        [Inject]
        ILocalStorageService localStorage { get; set; }

        public async void SubmitLogin()
        {
            
            Loading = true;
            ApplicationUserView applicationUser = new ApplicationUserView() { Entity = ApplicationUser };
            var result = await Http.PostJson<ApplicationUserView, string>(UrlList["login"], applicationUser);
            ApplicationUser.PasswordHash = "";
            Loading = false;
            if (result.Status == 200)
            {
                await localStorage.SetItemAsync(Config.TokenName, result.Title);
                NavigationManager.NavigateTo(Config.PathList.Home);
                await Message.Success("登录成功，欢迎回来");
                return;
            }
            this.StateHasChanged();
        }

        protected override void OnInitialized()
        {
            string backgroundImage = "_content/Caviar.AntDesignUI/Images/grov.jpg";
            var style = $"min-height:100vh;background-image: url({backgroundImage});";
            LayoutStyleCallBack.InvokeAsync(style);
            base.OnInitialized();
        }


    }
}
