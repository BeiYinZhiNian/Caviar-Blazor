using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Caviar.AntDesignUI.Core
{
    public static class CavNavigationManager
    {
        public static void NavigateTo(this NavigationManager navigationManager,IJSRuntime jSRuntime,string uri, bool replace = false)
        {
            if (Config.IsServer)
            {
                var iframeMessage = new IframeMessage();
                iframeMessage.Pattern = Pattern.ForceLoad;
                iframeMessage.Url = uri;
                _ = jSRuntime.InvokeVoidAsync(CurrencyConstant.JsIframeMessage, iframeMessage);
            }
            else
            {
                navigationManager.NavigateTo(uri, true, replace);
            }
        }
    }
}
