using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Core
{
    /// <summary>
    /// iframe与框架通信类
    /// </summary>
    public class IframeMessage
    {
        /// <summary>
        /// 调用的模式
        /// </summary>
        public Pattern Pattern { get; set; }
        /// <summary>
        /// 转到的地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 传输的数据
        /// </summary>
        public ServerToWasmExchange ExchangeData { get; set; }


        public delegate void JSScheduling(IframeMessage message);

        public static event JSScheduling SwitchWasm;

        public static event JSScheduling ForceLoad;

        [JSInvokable]
        public static void JsNavigation(IframeMessage message)
        {
            switch (message.Pattern)
            {
                case Pattern.Wasm:
                    SwitchWasm?.Invoke(message);
                    break;
                case Pattern.ForceLoad:
                    ForceLoad?.Invoke(message);
                    break;
                default:
                    break;
            }
        }
    }

    public enum Pattern
    {
        Wasm,
        ForceLoad
    }

    public class ServerToWasmExchange
    {
        public string[] SelectedKeys { get; set; }
        public string[] OpenKeysNav { get; set; }
        public string[] BreadcrumbItemArr { get; set; }
    }
}
