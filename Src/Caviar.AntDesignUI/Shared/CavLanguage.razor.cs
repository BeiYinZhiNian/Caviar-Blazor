using Caviar.AntDesignUI.Helper;
using Microsoft.AspNetCore.Components;
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
        protected override Task OnInitializedAsync()
        {
            var list = UserConfig.LanguageService.GetLanguageList();
            return base.OnInitializedAsync();
        }
    }
}
