using AntDesign;
using Caviar.Models.SystemData;
using Caviar.AntDesignPages.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Caviar.Models.SystemData;
/// <summary>
/// 生成者：admin
/// 生成时间：2021/6/23 18:04:56
/// 代码由代码生成器自动生成，更改的代码可能被进行替换
/// 可在上层目录使用partial关键字进行扩展
/// </summary>
namespace Caviar.AntDesignPages.Pages.Role
{
    [DisplayName("角色数据模板")]
    public partial class DataTemplate: ITableTemplate
    {
        #region 参数
        [Parameter]
        public ViewRole DataSource { get; set; } = new ViewRole() { Number = "999" };
        
        [Parameter]
        public string Url { get; set; }

        [Parameter]
        public string SuccMsg { get; set; } = "操作成功";
        #endregion

        #region 属性注入
        /// <summary>
        /// HttpClient
        /// </summary>
        [Inject]
        HttpHelper Http { get; set; }
        /// <summary>
        /// 全局提示
        /// </summary>
        [Inject]
        MessageService Message { get; set; }
        /// <summary>
        /// 导航管理器
        /// </summary>
        [Inject]
        NavigationManager NavigationManager { get; set; }
        #endregion

        
        #region 回调
        private Form<ViewRole> _meunForm;

        
        public async Task<bool> Submit()
        {
            //数据效验
            if(_meunForm.Validate())
            {
                return await FormSubmit();
            }
            return false;
        }

        async Task<bool> FormSubmit()
        {
            var result = await Http.PostJson(Url, DataSource);
            if (result.Status == 200)
            {
                Message.Success(SuccMsg);
                return true;
            }
            return false;
        }
        #endregion
        #region 重写
        partial void PratialOnInitializedAsync(ref bool IsContinue);
        partial void PratialOnAfterRender(ref bool IsContinue, bool firstRender);
        partial void PratialOnAfterRenderAsync(ref bool IsContinue, bool firstRender);
        partial void PratialOnInitialized(ref bool IsContinue);
        partial void PratialOnParametersSet(ref bool IsContinue);
        partial void PratialOnParametersSetAsync(ref bool IsContinue);
        partial void PratialShouldRender(ref bool IsContinue,ref bool isRender);
        protected override async Task OnInitializedAsync()
        {
            //交予开发者去实现，如果开发者不实现则由框架处理
            bool IsContinue = true;
            PratialOnInitializedAsync(ref IsContinue);
            if (!IsContinue) return;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            //交予开发者去实现，如果开发者不实现则由框架处理
            bool IsContinue = true;
            PratialOnAfterRender(ref IsContinue,firstRender);
            if (!IsContinue) return;
            base.OnAfterRender(firstRender);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //交予开发者去实现，如果开发者不实现则由框架处理
            bool IsContinue = true;
            PratialOnAfterRenderAsync(ref IsContinue, firstRender);
            if (!IsContinue) return;
            await base.OnAfterRenderAsync(firstRender);
        }

        protected override void OnInitialized()
        {
            //交予开发者去实现，如果开发者不实现则由框架处理
            bool IsContinue = true;
            PratialOnInitialized(ref IsContinue);
            if (!IsContinue) return;
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            //交予开发者去实现，如果开发者不实现则由框架处理
            bool IsContinue = true;
            PratialOnParametersSet(ref IsContinue);
            if (!IsContinue) return;
            base.OnParametersSet();
        }

        protected override async Task OnParametersSetAsync()
        {
            //交予开发者去实现，如果开发者不实现则由框架处理
            bool IsContinue = true;
            PratialOnParametersSetAsync(ref IsContinue);
            if (!IsContinue) return;
            await base.OnParametersSetAsync();
        }

        protected override bool ShouldRender()
        {
            //交予开发者去实现，如果开发者不实现则由框架处理
            bool IsContinue = true;
            bool isRender = base.ShouldRender();
            PratialShouldRender(ref IsContinue,ref isRender);
            if (!IsContinue) return isRender;
            return isRender;
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <returns></returns>
        public async void Refresh()
        {
            await OnInitializedAsync();
            StateHasChanged();
        }
        #endregion
    }
}
