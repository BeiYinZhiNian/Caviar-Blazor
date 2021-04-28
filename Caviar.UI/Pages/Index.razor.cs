using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caviar.Models.SystemData;
using Caviar.UI.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;

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
            var a = UserConfig.Router.GetObjValue("Routes");
            var json = JsonConvert.SerializeObject(a);
            var test = (IEnumerable)a;
            foreach (var item in test)
            {
                
            }
        }
    }


}