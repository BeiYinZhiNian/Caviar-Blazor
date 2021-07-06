using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using AntDesign;
using Caviar.Models.SystemData;
using System.Text;
using System.Web;
using System.Text.Json;
using Microsoft.JSInterop;
using Caviar.AntDesignPages.Helper;

namespace Caviar.AntDesignPages.Shared
{
    partial class CavMainLayout
    {
        /// <summary>
        /// logo图片地址
        /// </summary>
        string LogoImgSrc;

        string LogoImg;
        string LogoImgIco;

        string HeaderStyle { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// 面包屑数据同步
        /// </summary>
        public MenuItem BreadcrumbItemCav;
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
        }
        protected override async Task OnInitializedAsync()
        {
            LogoImg = "_content/Caviar.AntDesignPages/Images/logo.png";
            LogoImgIco = "_content/Caviar.AntDesignPages/Images/logo-Ico.png";
            LogoImgSrc = LogoImg;
            await base.OnInitializedAsync();
        }

        bool _collapsed;
        bool Collapsed
        {
            set
            {
                CollapseCallback(value);
                _collapsed = value;
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
            this.Collapsed = collapsed;
        }

        /// <summary>
        /// 菜单栏缩放时百分百触发
        /// </summary>
        /// <param name="collapsed"></param>
        void CollapseCallback(bool collapsed)
        {
            if (collapsed)
            {
                HeaderStyle = "margin-left: 0px";
                LogoImgSrc = LogoImgIco;
            }
            else
            {
                HeaderStyle = "margin-left: 200px";
                LogoImgSrc = LogoImg;
            }
        }
    }

    public class MainLayoutStyle
    {
        public bool Loading { get; set; }
    }
}
