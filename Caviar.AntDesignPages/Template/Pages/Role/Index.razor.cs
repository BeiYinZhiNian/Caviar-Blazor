using AntDesign;
using Caviar.Models.SystemData;
using Caviar.AntDesignPages.Helper;
using Caviar.AntDesignPages.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using Caviar.Models.SystemData;
/// <summary>
/// 生成者：未登录用户
/// 生成时间：2021/5/31 11:39:27
/// 代码由代码生成器自动生成，更改的代码可能被进行替换
/// 可在上层目录使用partial关键字进行扩展
/// </summary>
namespace Caviar.AntDesignPages.Pages.Role
{
    [DisplayName("角色列表")]
    public partial class Index
    {
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

        #region 属性
        /// <summary>
        /// 数据源
        /// </summary>
        List<ViewRole> DataSource { get; set; } = new List<ViewRole>();
        /// <summary>
        /// 总计
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 页数
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 页面大小
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 按钮
        /// </summary>
        List<ViewMenu> Buttons { get; set; } = new List<ViewMenu>();
        #endregion

        #region 方法
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="isOrder"></param>
        /// <returns></returns>
        partial void PratialGetPages(ref bool isContinue, ref int pageIndex,ref int pageSize,ref bool isOrder);
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="isOrder"></param>
        /// <returns></returns>
        async void GetPages(int pageIndex = 1, int pageSize = 10, bool isOrder = true)
        {
            bool isContinue = true;
            PratialGetPages(ref isContinue, ref pageIndex, ref pageSize, ref isOrder);
            if (!isContinue) return;
            var url = NavigationManager.Uri.Replace(NavigationManager.BaseUri,"");
            var result = await Http.GetJson<PageData<ViewRole>>($"{url}?pageIndex={pageIndex}&pageSize={pageSize}&isOrder={isOrder}");
            if (result.Status != 200) return;
            if (result.Data != null)
            {
                DataSource = result.Data.Rows;
                Total = result.Data.Total;
                PageIndex = result.Data.PageIndex;
                PageSize = result.Data.PageSize;
                StateHasChanged();
            }
        }
        /// <summary>
        /// 获取按钮
        /// </summary>
        /// <returns></returns>
        async void GetPowerButtons()
        {
            string url = NavigationManager.Uri.Replace(NavigationManager.BaseUri, "");
            var result = await Http.GetJson<List<ViewMenu>>("Menu/GetButtons?url=" + url);
            if (result.Status != 200) return;
            Buttons = result.Data;
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="data"></param>
        async void Delete(string url,ViewRole data)
        {
            //删除单条
            var result = await Http.PostJson(url, data);
            if (result.Status == 200)
            {
                Message.Success("删除成功");
            }
            Refresh();
        }
        #endregion

        #region 回调
        /// <summary>
        /// 按钮行回调
        /// </summary>
        /// <param name="isContinue"></param>
        /// <param name="row"></param>
        partial void PratialRowCallback(ref bool isContinue, RowCallbackData<ViewRole> row);
        /// <summary>
        /// 翻页按钮回调
        /// </summary>
        /// <param name="isContinue"></param>
        /// <param name="args"></param>
        partial void PratialPageIndexChanged(ref bool isContinue, PaginationEventArgs args);

        [Inject]
        CavModal CavModal { get; set; }
        async void RowCallback(RowCallbackData<ViewRole> row)
        {
            bool isContinue = true;
            PratialRowCallback(ref isContinue, row);
            if (!isContinue) return;
            switch (row.Menu.MenuName)
            {
                case "删除":
                    Delete(row.Menu.Url,row.Data);
                    break;
                case "修改":
                    Refresh();
                    break;
                case "新增":
                    Refresh();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 分页回调
        /// </summary>
        /// <param name="args"></param>
        void PageIndexChanged(PaginationEventArgs args)
        {
            bool isContinue = true;
            PratialPageIndexChanged(ref isContinue, args);
            if (!isContinue) return;
            GetPages(args.Page,args.PageSize);
        }
        #endregion

        #region 重写
        partial void PratialOnInitializedAsync(ref bool isContinue);
        partial void PratialOnAfterRender(ref bool isContinue, bool firstRender);
        partial void PratialOnAfterRenderAsync(ref bool isContinue, bool firstRender);
        partial void PratialOnInitialized(ref bool isContinue);
        partial void PratialOnParametersSet(ref bool isContinue);
        partial void PratialOnParametersSetAsync(ref bool isContinue);
        partial void PratialShouldRender(ref bool isContinue,ref bool isRender);
        protected override async Task OnInitializedAsync()
        {
            //交予开发者去实现，如果开发者不实现则由框架处理
            bool isContinue = true;
            PratialOnInitializedAsync(ref isContinue);
            if (!isContinue) return;
            GetPages();//获取数据源
            GetPowerButtons();//获取按钮
        }

        protected override void OnAfterRender(bool firstRender)
        {
            //交予开发者去实现，如果开发者不实现则由框架处理
            bool isContinue = true;
            PratialOnAfterRender(ref isContinue,firstRender);
            if (!isContinue) return;
            base.OnAfterRender(firstRender);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //交予开发者去实现，如果开发者不实现则由框架处理
            bool isContinue = true;
            PratialOnAfterRenderAsync(ref isContinue, firstRender);
            if (!isContinue) return;
            await base.OnAfterRenderAsync(firstRender);
        }

        protected override void OnInitialized()
        {
            //交予开发者去实现，如果开发者不实现则由框架处理
            bool isContinue = true;
            PratialOnInitialized(ref isContinue);
            if (!isContinue) return;
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            //交予开发者去实现，如果开发者不实现则由框架处理
            bool isContinue = true;
            PratialOnParametersSet(ref isContinue);
            if (!isContinue) return;
            base.OnParametersSet();
        }

        protected override async Task OnParametersSetAsync()
        {
            //交予开发者去实现，如果开发者不实现则由框架处理
            bool isContinue = true;
            PratialOnParametersSetAsync(ref isContinue);
            if (!isContinue) return;
            await base.OnParametersSetAsync();
        }

        protected override bool ShouldRender()
        {
            //交予开发者去实现，如果开发者不实现则由框架处理
            bool isContinue = true;
            bool isRender = base.ShouldRender();
            PratialShouldRender(ref isContinue,ref isRender);
            if (!isContinue) return isRender;
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