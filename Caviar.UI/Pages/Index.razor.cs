
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caviar.Models.SystemData;
using Caviar.UI.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
namespace Caviar.UI.Pages
{
    partial class Index
    {

        [CascadingParameter]
        public EventCallback LayoutStyleCallBack { get; set; }

        [Inject]
        HttpHelper Http { get; set; }


        [Inject]
        IJSRuntime JsRuntime { get; set; }
        public async Task Test()
        {

        }
    }


}