// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Caviar.AntDesignUI.Core
{
    public class PrismHighlighter : IPrismHighlighter
    {
        private readonly IJSRuntime _jsRuntime;

        public PrismHighlighter(IJSRuntime jsRuntime)
        {
            this._jsRuntime = jsRuntime;
        }

        public async Task<string> HighlightAsync(string code, string language)
        {
            string highlighted = await _jsRuntime.InvokeAsync<string>("AntDesign.Prism.highlight", code, language);

            return highlighted;
        }

        public async Task HighlightAllAsync()
        {
            await _jsRuntime.InvokeVoidAsync("AntDesign.Prism.highlightAll");
        }
    }
}
