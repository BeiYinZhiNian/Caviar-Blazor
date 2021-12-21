using AntDesign;
using Caviar.AntDesignUI.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
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
        public UserConfig UserConfig { get; set; }
        public NavLinkMatch RouterMatch { get; set; } = NavLinkMatch.Prefix;
        public List<(string CultureName, string ResourceName)> LanguageList { get; set; }

        protected override Task OnInitializedAsync()
        {
            LanguageList = UserConfig.LanguageService.GetLanguageList();
            return base.OnInitializedAsync();
        }

        public void SelectLanguage(MenuItem menuItem)
        {
            UserConfig.LanguageService.SetLanguage(menuItem.Key);
        }
    }
}
