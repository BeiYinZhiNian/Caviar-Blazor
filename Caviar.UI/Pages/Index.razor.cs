using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Caviar.Models.SystemData;
using Caviar.Pages.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Caviar.UI.Pages
{
    partial class Index
    {

        [CascadingParameter]
        public EventCallback LayoutStyleCallBack { get; set; }

        [Inject]
        HttpHelper Http { get; set; }

        [Inject]
        UserConfigHelper UserConfig { get; set; }

        [Inject]
        IJSRuntime JsRuntime { get; set; }
        public async Task Test()
        {
            
        }
    }
}