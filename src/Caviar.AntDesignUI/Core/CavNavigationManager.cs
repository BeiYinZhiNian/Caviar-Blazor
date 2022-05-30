// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Caviar.AntDesignUI.Core
{
    public static class CavNavigationManager
    {
        public static void NavigateTo(this NavigationManager navigationManager, IJSRuntime jSRuntime, string uri, bool forceLoad = false, bool replace = false)
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
                navigationManager.NavigateTo(uri, forceLoad, replace);
            }
        }
    }
}
