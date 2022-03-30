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
        public string Style { get; set; }

        protected override void OnInitialized()
        {
            string backgroundImage = "_content/Caviar.AntDesignUI/Images/grov.jpg";
            Style = $"min-height:100vh;background-image: url({backgroundImage});";
            base.OnInitialized();
        }
    }
}
