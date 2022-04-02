using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.User;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Web;

namespace Caviar.Demo.Wasm.Pages
{
    public partial class Login
    {
        public UserLogin ApplicationUser { get; set; } = new UserLogin()
        {
            UserName = "admin",
            Password = "123456",
            RememberMe = true,
        };

        string? style;

        protected override void OnInitialized()
        {
            string backgroundImage = "_content/Caviar.AntDesignUI/images/grov.jpg";
            style = $"min-height:100vh;background-image: url({backgroundImage});";
            base.OnInitialized();
        }
    }
}
