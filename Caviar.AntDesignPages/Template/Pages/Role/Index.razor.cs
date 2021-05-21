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
using Caviar.Models.SystemData;
/// <summary>
/// 生成者：未登录用户
/// 生成时间：2021/5/21 16:32:54
/// 代码由代码生成器自动生成，更改的代码可能被进行替换
/// 可在上层目录使用partial关键字进行扩展
/// </summary>
namespace Caviar.AntDesignPages.Pages.Role
{
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
        /// 用户配置
        /// </summary>
        [Inject]
        UserConfigHelper UserConfig { get; set; }
        /// <summary>
        /// 导航管理器
        /// </summary>
        [Inject]
        NavigationManager NavigationManager { get; set; }
        /// <summary>
        /// 确认框
        /// </summary>
        [Inject]
        ConfirmService Confirm { get; set; }
        #endregion

        #region 属性
        /// <summary>
        /// 数据源
        /// </summary>
        List<ViewRole> DataSource { get; set; }
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
        /// 表头
        /// </summary>
        List<ViewModelHeader> ViewModelHeaders { get; set; }
        /// <summary>
        /// 按钮
        /// </summary>
        List<ViewMenu> Buttons { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <returns></returns>
        async Task GetViewRole()
        {
            var result = await Http.GetJson<PageData<ViewRole>>("Role/GetPages");
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
        async Task<List<ViewMenu>> GetPowerButtons()
        {
            string url = NavigationManager.Uri.Replace(NavigationManager.BaseUri, "");
            var result = await Http.GetJson<List<ViewMenu>>("Menu/GetButtons?url=" + url);
            if (result.Status != 200) return new List<ViewMenu>();
            return result.Data;
        }
        #endregion

        #region 回调
        [Inject]
        CavModal CavModal { get; set; }
        async void RowCallback(RowCallbackData<ViewRole> row)
        {
            switch (row.Menu.MenuName)
            {
                case "删除":
                    Delete(row.Data);
                    break;
                case "修改":
                    await CavModal.Create("/Role/Update/{Id:int}", row.Menu.MenuName,Refresh,
                        new List<KeyValuePair<string, object?>> { 
                            new KeyValuePair<string, object?>("Id",row.Data.Id)
                        });
                    break;
                case "新增":
                    Refresh();
                    break;
            }
        }
        async void Delete(ViewRole data)
        {
            //删除单条
            var result = await Http.PostJson<ViewRole, object>("Menu/MoveEntity", data);
            if (result.Status == 200)
            {
                Message.Success("删除成功");
            }
            Refresh();
        }
        #endregion

        #region 重写
        protected override async Task OnInitializedAsync()
        {
            await GetViewRole();//获取数据源
            Buttons = await GetPowerButtons();//获取按钮
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
