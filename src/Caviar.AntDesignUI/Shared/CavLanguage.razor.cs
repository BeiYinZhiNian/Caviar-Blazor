using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
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
        NavigationManager NavigationManager { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        [Inject]
        ILanguageService LanguageService { get; set; }
        NavLinkMatch RouterMatch { get; set; } = NavLinkMatch.Prefix;
        List<(string CultureName, string ResourceName)> LanguageList { get; set; }
        protected override Task OnInitializedAsync()
        {
            LanguageList = LanguageService.GetLanguageList();
            return base.OnInitializedAsync();
        }

        public void SelectLanguage(MenuItem menuItem)
        {
            NavigationManager.NavigateTo(JSRuntime, $"{CurrencyConstant.Api}{UrlConfig.SetCookieLanguage}?acceptLanguage={menuItem.Key}&returnUrl=/");
        }
    }
}
