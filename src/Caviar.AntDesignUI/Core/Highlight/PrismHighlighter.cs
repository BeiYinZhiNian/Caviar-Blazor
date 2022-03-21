﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Caviar.AntDesignUI.Core
{
    public class PrismHighlighter : IPrismHighlighter
    {
        private readonly IJSRuntime jsRuntime;

        public PrismHighlighter(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        public async ValueTask<MarkupString> HighlightAsync(string code, string language)
        {
            string highlighted = await jsRuntime.InvokeAsync<string>("AntDesign.Prism.highlight", code, language);

            return new MarkupString(highlighted);
        }

        public async Task HighlightAllAsync()
        {
            await jsRuntime.InvokeVoidAsync("AntDesign.Prism.highlightAll");
        }
    }
}