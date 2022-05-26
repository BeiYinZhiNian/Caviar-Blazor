// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using Microsoft.AspNetCore.Components;

namespace Caviar.AntDesignUI.Shared
{
    partial class CavEmptyLayout
    {
        [Parameter]
        public string Style { get; set; } = "min-height:100vh;";

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }
        EventCallback _layoutStyleCallBack = EventCallback.Empty;
        EventCallback LayoutStyleCallBack
        {
            get
            {
                if (_layoutStyleCallBack.Equals(EventCallback.Empty))
                    _layoutStyleCallBack = EventCallback.Factory.Create(this, SetStyle);
                return _layoutStyleCallBack;
            }
        }

        public void SetStyle(object style)
        {
            if (style != null)
            {
                Style = style.ToString();
            }
        }

    }
}
