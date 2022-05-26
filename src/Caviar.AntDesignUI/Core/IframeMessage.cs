// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Threading.Tasks;
using Caviar.SharedKernel.Entities;
using Microsoft.JSInterop;

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
                    if (SwitchWasm == null)
                    {
                        return Task.FromResult(CurrencyConstant.NotReady);
                    }
                    SwitchWasm?.Invoke(message);
                    break;
                default:
                    return Task.FromResult(CurrencyConstant.NonexistentInstruction);
            }
            return Task.FromResult(CurrencyConstant.Success);
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
