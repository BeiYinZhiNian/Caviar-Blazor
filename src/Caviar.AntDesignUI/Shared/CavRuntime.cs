using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Shared
{
    public partial class CavRuntime
    {
        private string Runtime
        {
            get
            {
                var runtime = System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier;
                if (runtime == "browser-wasm")
                {
                    return "Wasm";
                }
                else
                {
                    return "Server";
                }
            }
        }

        [Inject]
        IJSRuntime JSRuntime { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        CavLayout Layout { get; set; }
        [Inject]
        MessageService MessageService { get; set; }

        private void SwithWasm()
        {
            if (!Config.IsServer) return;
            var iframeMessage = new IframeMessage();
            iframeMessage.Pattern = Pattern.Wasm;
            iframeMessage.Url = NavigationManager.BaseUri;
            iframeMessage.ExchangeData = new ServerToWasmExchange()
            {
                Layout = JsonConvert.SerializeObject(Layout)
            };
            _ = JSRuntime.InvokeVoidAsync(CurrencyConstant.JsIframeMessage, iframeMessage);
            Task.Run(() =>
            {
                //一秒钟后如果还存在，则代表wasm还没加载成功，给个友好提示哈哈
                Task.Delay(500).Wait();
                MessageService.Warning("wasm未准备就绪，请稍后在试");
            });
            
        }

        private void SwithServer()
        {
            if (Config.IsServer) return;
            _ = JSRuntime.InvokeVoidAsync(CurrencyConstant.SwitchServer, NavigationManager.BaseUri);
        }
    }
}
