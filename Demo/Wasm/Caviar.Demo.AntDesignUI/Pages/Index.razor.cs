using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Caviar.SharedKernel;
using Caviar.AntDesignUI.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Caviar.AntDesignUI.Shared;
using AntDesign;

namespace Caviar.Demo.AntDesignUI.Pages
{
    partial class Index
    {

        [CascadingParameter]
        public EventCallback LayoutStyleCallBack { get; set; }

        [Inject]
        HttpHelper Http { get; set; }

        [Inject]
        UserConfig UserConfig { get; set; }

        [Inject]
        IJSRuntime JsRuntime { get; set; }

        CavDataTemplate CavData { get; set; }

        RenderFragment test;

        protected override Task OnInitializedAsync()
        {

            return base.OnInitializedAsync();
        }

        public async Task Test()
        {

            StateHasChanged();
        }


    }
}