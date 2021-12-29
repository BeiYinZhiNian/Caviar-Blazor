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
        public UserLogin ApplicationUser { get; set; } = new UserLogin() { UserName = "admin",Password= "1031622947@qq.COM",RememberMe=true };

        [CascadingParameter]
        public EventCallback LayoutStyleCallBack { get; set; }

        [Inject] 
        HostAuthenticationStateProvider AuthStateProvider { get; set; }

        public async void SubmitLogin()
        {
            
            Loading = true;
            var returnUrl = HttpUtility.ParseQueryString(new Uri(NavigationManager.Uri).Query)["returnUrl"];
            if (returnUrl == null) returnUrl = "/";
            var result = await AuthStateProvider.Login(ApplicationUser, returnUrl);
            ApplicationUser.Password = "";
            Loading = false;
            if (result.Status == 200)
            {
                NavigationManager.NavigateTo(result.Url,true);
                _ = MessageService.Success(result.Title);
                return;
            }
            this.StateHasChanged();
        }

        public string Style { get; set; }

        protected override void OnInitialized()
        {
            CurrentUrl = Config.PathList.Login;
            string backgroundImage = "_content/Caviar.AntDesignUI/Images/grov.jpg";
            Style = $"min-height:100vh;background-image: url({backgroundImage});";
            
            base.OnInitialized();
        }


    }
}
