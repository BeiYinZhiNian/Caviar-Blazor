using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
