using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using AntDesign;
using Caviar.SharedKernel.View;
using System.Text;
using System.Web;
using System.Text.Json;
using Caviar.AntDesignUI.Helper;
using AntDesign.JsInterop;
using Microsoft.JSInterop;

namespace Caviar.AntDesignUI.Shared
{
    partial class CavMainLayout
    {
        /// <summary>
        /// logo图片地址
        /// </summary>
        string LogoImgSrc;

        string LogoImg;
        string LogoImgIco;

        string HeaderStyle { get; set; } = "margin-left: 200px";
        [Inject]
        public UserConfig UserConfig { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        public void StateHasAction()
        {
            StateHasChanged();
        }

        /// <summary>
        /// 面包屑数据同步
        /// </summary>
        public MenuItem BreadcrumbItemCav;
        protected override async Task OnParametersSetAsync()
        {
            UserConfig.StateHasAction = StateHasAction;
            await base.OnParametersSetAsync();
        }
        protected override async Task OnInitializedAsync()
        {
            LogoImg = "_content/Caviar.AntDesignUI/Images/logo.png";
            LogoImgIco = "_content/Caviar.AntDesignUI/Images/logo-Ico.png";
            LogoImgSrc = LogoImg;
            await base.OnInitializedAsync();
        }

        bool _collapsed;
        /// <summary>
        /// 控制左侧菜单是否折叠
        /// </summary>
        bool Collapsed
        {
            set
            {
                _collapsed = value;
                CollapseCallback(value);
            }
            get
            {
                return _collapsed;
            }
        }

        int CollapsedWidth { get; set; } = 0;

        /// <summary>
        /// 按钮点击时触发
        /// </summary>
        void Toggle()
        {
            Collapsed = !Collapsed;
        }

        /// <summary>
        /// 只有熔断触发，Toggle不触发
        /// </summary>
        /// <param name="collapsed"></param>
        void OnCollapse(bool collapsed)
        {
            Collapsed = collapsed;
        }
        /// <summary>
        /// 菜单栏缩放时百分百触发
        /// </summary>
        /// <param name="collapsed"></param>
        async void CollapseCallback(bool collapsed)
        {
            var clientWidth = await JSRuntime.InvokeAsync<int>("getClientWidth");
            if (collapsed)
            {
                if (clientWidth >= 576)
                {
                    HeaderStyle = "margin-left: 80px";
                    LogoImgSrc = LogoImgIco;
                    CollapsedWidth = 80;
                }
                else
                {
                    HeaderStyle = "margin-left: 0px";
                    LogoImgSrc = LogoImgIco;
                    CollapsedWidth = 0;
                }
            }
            else
            {
                HeaderStyle = "margin-left: 200px";
                LogoImgSrc = LogoImg;
            }
            StateHasChanged();
        }
    }
}
