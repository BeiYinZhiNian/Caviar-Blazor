using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.User;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Web;

namespace Caviar.AntDesignUI.Pages.User
{
    public partial class Login
    {
        [Inject]
        UserConfig UserConfig { get; set; }

        public UserLogin ApplicationUser { get; set; } = new UserLogin();

        [CascadingParameter]
        public EventCallback LayoutStyleCallBack { get; set; }

        [Inject] 
        HostAuthenticationStateProvider AuthStateProvider { get; set; }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        MessageService MessageService { get; set; }

        bool Loading { get; set; } = true;

        Form<UserLogin> Form;

        public async void SubmitLogin()
        {
            if (!Form.Validate()) return;
            Loading = true;
            var returnUrl = HttpUtility.ParseQueryString(new Uri(NavigationManager.Uri).Query)["returnUrl"];
            if (returnUrl == null) returnUrl = "/";
            ApplicationUser.Password = CommonHelper.SHA256EncryptString(ApplicationUser.Password);
            var result = await AuthStateProvider.Login(ApplicationUser, returnUrl);
            Loading = false;
            if (result.Status == System.Net.HttpStatusCode.OK)
            {
                _ = MessageService.Success(result.Title);
                NavigationManager.NavigateTo(JSRuntime, result.Url);
                return;
            }
            ApplicationUser.Password = "";
            StateHasChanged();
        }

        public string Style { get; set; }

        protected override void OnInitialized()
        {
            string backgroundImage = "_content/Caviar.AntDesignUI/Images/grov.jpg";
            Style = $"min-height:100vh;background-image: url({backgroundImage});";
            if (Config.IsDebug)
            {
                ApplicationUser.RememberMe = true;
                ApplicationUser.UserName = "admin";
                ApplicationUser.Password = "123456";
            }
            base.OnInitialized();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                Loading = false;
                StateHasChanged();
            }
            base.OnAfterRender(firstRender);
        }

    }
}
