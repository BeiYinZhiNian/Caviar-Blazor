using AntDesign;
using Blazored.LocalStorage;
using Caviar.AntDesignUI.Helper;
using Caviar.SharedKernel;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.User;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Web;

namespace Caviar.AntDesignUI.Pages.User
{
    public partial class Login
    {
        public UserLogin ApplicationUser { get; set; } = new UserLogin() { UserName = "admin",Password= "1031622947@qq.COM" };

        [CascadingParameter]
        public EventCallback LayoutStyleCallBack { get; set; }

        [Inject] 
        HostAuthenticationStateProvider AuthStateProvider { get; set; }

        public async void SubmitLogin()
        {
            
            Loading = true;
            var returnUrl = HttpUtility.ParseQueryString(new Uri(NavigationManager.Uri).Query)["returnUrl"];
            var result = await AuthStateProvider.Login(ApplicationUser, returnUrl);
            ApplicationUser.Password = "";
            Loading = false;
            if (result.Status == 200)
            {
                NavigationManager.NavigateTo(result.Url,forceLoad:true);
                _ = MessageService.Success(result.Title);
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
