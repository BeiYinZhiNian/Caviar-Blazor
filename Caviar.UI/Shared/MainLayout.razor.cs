using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using AntDesign;

namespace Caviar.UI.Shared
{
    partial class MainLayout
    {
        [Inject]
        IConfiguration Configuration { get; set; }
        /// <summary>
        /// logo图片地址
        /// </summary>
        string LogoImgSrc;

        string LogoImg;
        string LogoImgIco;

        string HeaderStyle { get; set; }

        /// <summary>
        /// 面包屑数据同步
        /// </summary>
        public MenuItem BreadcrumbItemNav;

        protected override void OnInitialized()
        {
            LogoImg = Configuration["Logo:LogoPath:logo"];
            LogoImgIco = Configuration["Logo:LogoPath:logo-Ico"];
            LogoImgSrc = LogoImg;
            base.OnInitialized();
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
                HeaderStyle = "margin-left: 80px";
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
