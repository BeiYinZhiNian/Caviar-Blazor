// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Linq;
using System.Threading.Tasks;
using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components;
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
        UserConfig UserConfig { get; set; }
        [Inject]
        CavLayout Layout { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// 面包屑数据同步
        /// </summary>
        string[] BreadcrumbItemArr = new string[] { };

        [Inject]
        NotificationService NotificationService { get; set; }
        [Inject]
        HostAuthenticationStateProvider HostAuthenticationStateProvider { get; set; }
        protected override async Task OnInitializedAsync()
        {
            UserConfig.LayoutPage = Refresh;
            LogoImg = "_content/Caviar.AntDesignUI/images/logo.png";
            LogoImgIco = "_content/Caviar.AntDesignUI/images/logo-Ico.png";
            LogoImgSrc = LogoImg;
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && Config.IsServer)
            {
                var slot = "";
                var description = "";
                var timeSlot = CommonHelper.GetTimeSlot();
                switch (timeSlot)
                {
                    case TimeSlot.Morning:
                        slot = "上午好";
                        description = "又是元气满满的一天";
                        break;
                    case TimeSlot.Noon:
                        slot = "中午好";
                        description = "午安，该休息了";
                        break;
                    case TimeSlot.Afternoon:
                        slot = "下午好";
                        description = "在累也要注意休息";
                        break;
                    case TimeSlot.Night:
                        slot = "晚上好";
                        description = "去外面走一走吧，不要忽略了风景";
                        break;
                    case TimeSlot.Midnight:
                        slot = "夜深了";
                        description = "这么晚了，一定有很多心事吧";
                        break;
                    default:
                        break;
                }
                var userInfo = await HostAuthenticationStateProvider.GetCurrentUser();
                _ = NotificationService.Open(new NotificationConfig()
                {
                    Message = $"{slot} {userInfo.Claims.SingleOrDefault(u => u.Type == CurrencyConstant.AccountName)?.Value}",
                    Description = description,
                    NotificationType = NotificationType.Success
                });
            }
            await base.OnAfterRenderAsync(firstRender);
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
        /// 是否使用遮罩
        /// </summary>
        bool IsDrawer { get; set; } = false;

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
            IsDrawer = clientWidth < 576;
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
                if (!IsDrawer)//是否使用遮罩
                    HeaderStyle = "margin-left: 200px";
                LogoImgSrc = LogoImg;
            }
            StateHasChanged();
        }
        /// <summary>
        /// 刷新布局
        /// </summary>
        void Refresh()
        {
            StateHasChanged();
        }
    }
}
