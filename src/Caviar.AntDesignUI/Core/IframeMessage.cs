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

        [JSInvokable]
        public static Task<string> JsNavigation(IframeMessage message)
        {
            switch (message.Pattern)
            {
                case Pattern.Wasm:
                    SwitchWasm?.Invoke(message);
                    break;
                default:
                    break;
            }
            return Task.FromResult("ok");
        }
    }

    public enum Pattern
    {
        Wasm,
        ForceLoad
    }

    public class ServerToWasmExchange
    {
        /// <summary>
        /// 选择的菜单key
        /// </summary>
        public string[] SelectedKeys { get; set; }
        /// <summary>
        /// 打开的目录
        /// </summary>
        public string[] OpenKeysNav { get; set; }
        /// <summary>
        /// 面包屑内容
        /// </summary>
        public string[] BreadcrumbItemArr { get; set; }
        /// <summary>
        /// 布局
        /// </summary>
        public string Layout { get; set; }
    }
}
