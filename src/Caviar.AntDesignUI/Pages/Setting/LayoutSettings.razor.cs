using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Caviar.AntDesignUI.Core;
using AntDesign;
using Caviar.SharedKernel.Entities;

namespace Caviar.AntDesignUI.Pages.Setting
{
    public partial class LayoutSettings
    {
        [Inject]
        UserConfig UserConfig { get; set; }
        [Inject]
        CavLayout CavLayout { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        [Inject]
        ILanguageService LanguageService { get; set; }
        [Inject]
        MessageService MessageService { get; set; }
        private async void Preservation()
        {
            var json = JsonConvert.SerializeObject(CavLayout);
            await JSRuntime.InvokeVoidAsync(CurrencyConstant.SetCookie, CurrencyConstant.LayoutTheme, json, 999);
            _ = MessageService.Success("主题保存成功");
        }
    }
}
