using AntDesign;
using Caviar.AntDesignUI.Helper;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI
{
    public partial class CavComponentBase : ComponentBase
    {
        #region 属性注入
        /// <summary>
        /// HttpClient
        /// </summary>
        [Inject]
        protected HttpHelper Http { get; set; }
        /// <summary>
        /// 全局提示
        /// </summary>
        [Inject]
        protected MessageService Message { get; set; }
        /// <summary>
        /// 导航管理器
        /// </summary>
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        #endregion

        /// <summary>
        /// 刷新
        /// </summary>
        /// <returns></returns>
        public virtual async void Refresh()
        {
            await OnInitializedAsync();
            StateHasChanged();
        }
    }
}
