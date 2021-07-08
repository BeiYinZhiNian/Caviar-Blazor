using AntDesign;
using Caviar.AntDesignPages.Helper;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages
{
    public partial class IndexComponentBase<ViewT> : CavComponentBase where ViewT:class, new()
    {

        #region 属性
        /// <summary>
        /// 数据源
        /// </summary>
        protected List<ViewT> DataSource { get; set; } = new List<ViewT>();
        /// <summary>
        /// 总计
        /// </summary>
        protected int Total { get; set; }
        /// <summary>
        /// 页数
        /// </summary>
        protected int PageIndex { get; set; }
        /// <summary>
        /// 页面大小
        /// </summary>
        protected int PageSize { get; set; }
        /// <summary>
        /// 按钮
        /// </summary>
        protected List<ViewMenu> Buttons { get; set; } = new List<ViewMenu>();
        /// <summary>
        /// 模型字段
        /// </summary>
        protected List<ViewModelFields> ViewModelFields { get; set; } = new List<ViewModelFields>();
        public ViewQuery Query { get; set; } = new ViewQuery();

        protected string BaseController { get; set; }
        [Parameter]
        public string Url { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="isOrder"></param>
        /// <returns></returns>
        protected virtual async Task<List<ViewT>> GetPages(int pageIndex = 1, int pageSize = 10, bool isOrder = true)
        {
            var result = await Http.GetJson<PageData<ViewT>>($"{Url}?pageIndex={pageIndex}&pageSize={pageSize}&isOrder={isOrder}");
            if (result.Status != 200) return null;
            if (result.Data != null)
            {
                DataSource = result.Data.Rows;
                Total = result.Data.Total;
                PageIndex = result.Data.PageIndex;
                PageSize = result.Data.PageSize;
                return DataSource;
            }
            return null;
        }
        /// <summary>
        /// 获取按钮
        /// 获取按钮需要到Menu控制器下
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<List<ViewMenu>> GetPowerButtons()
        {
            var result = await Http.GetJson<List<ViewMenu>>("Menu/GetButtons?url=" + Url);
            if (result.Status != 200) return null;
            Buttons = result.Data;
            return Buttons;
        }
        /// <summary>
        /// 获取模型字段
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<List<ViewModelFields>> GetModelFields()
        {
            var result = await Http.GetJson<List<ViewModelFields>>($"{BaseController}/GetFields");
            if (result.Status != 200) return null;
            ViewModelFields = result.Data;
            return ViewModelFields;
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="data"></param>
        protected virtual async Task<bool> Delete(string url, ViewT data)
        {
            //删除单条
            var result = await Http.PostJson(url, data);
            if (result.Status != 200) return false;
            Message.Success("删除成功");
            return true;
        }
        #endregion

        #region 回调
        [Inject]
        CavModal CavModal { get; set; }
        protected virtual async Task RowCallback(RowCallbackData<ViewT> row)
        {
            switch (row.Menu.MenuName)
            {
                case "删除":
                    await Delete(row.Menu.Url, row.Data);
                    break;
                case "修改":
                    break;
                case "新增":
                    break;
                default:
                    break;
            }
            Refresh();
        }
        /// <summary>
        /// 模糊查询
        /// </summary>
        /// <param name="Query"></param>
        protected virtual async void FuzzyQueryCallback()
        {
            var result = await Http.PostJson<ViewQuery, List<ViewT>>(BaseController + "/FuzzyQuery", Query);
            if (result.Status != 200) return;
            DataSource = result.Data;
            StateHasChanged();
        }
        /// <summary>
        /// 分页回调
        /// </summary>
        /// <param name="args"></param>
        protected virtual async Task PageIndexChanged(PaginationEventArgs args)
        {
            await GetPages(args.Page, args.PageSize);
        }
        #endregion

        #region 重写
        protected override async Task OnInitializedAsync()
        {
            if (string.IsNullOrEmpty(Url))
            {
                var url = NavigationManager.Uri.Replace(NavigationManager.BaseUri, "");
                Url = url;
            }
            BaseController = CommonHelper.GetLeftText(Url, "/");
            await GetModelFields();//获取模型字段
            await GetPages();//获取数据源
            await GetPowerButtons();//获取按钮
        }
        #endregion
    }
}
