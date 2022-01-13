using AntDesign;
using Caviar.AntDesignUI.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Shared
{
    public partial class CavLanguage
    {
        [Inject]
        UserConfig UserConfig { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        NavLinkMatch RouterMatch { get; set; } = NavLinkMatch.Prefix;
        List<(string CultureName, string ResourceName)> LanguageList { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        protected override Task OnInitializedAsync()
        {
            LanguageList = UserConfig.LanguageService.GetLanguageList();
            return base.OnInitializedAsync();
        }

        public void SelectLanguage(MenuItem menuItem)
        {
            NavigationManager.NavigateTo(JSRuntime, $"{Config.PathList.SetCookieLanguage}?acceptLanguage={menuItem.Key}&returnUrl={NavigationManager.Uri}");
        }
    }
}
