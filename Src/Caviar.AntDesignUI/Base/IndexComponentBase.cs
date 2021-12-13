using AntDesign;
using Caviar.AntDesignUI.Helper;
using Caviar.SharedKernel;
using Caviar.SharedKernel.Entities.View;
using Caviar.SharedKernel.View;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI
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
        
        protected List<SysMenuView> Buttons { get; set; } = new List<SysMenuView>();
        /// <summary>
        /// 模型字段
        /// </summary>
        protected List<ViewFields> ViewFields { get; set; } = new List<ViewFields>();
        public ViewQuery Query { get; set; }
        protected string BaseController { get; set; }
        public bool Loading { get; set; }
        
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
            if (result.Status != StatusCodes.Status200OK) return null;
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
        /// 加载按钮
        /// </summary>
        /// <returns></returns>
        protected virtual void LoadButton()
        {
            var queryButton = UrlList["FuzzyQuery"];
            if (queryButton != null)
            {
                Query = new ViewQuery();
            }
            Buttons = APIList.Where(u => u.Entity.ControllerName == BaseController).ToList();
        }
        /// <summary>
        /// 获取模型字段
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<List<ViewFields>> GetModelFields()
        {
            var result = await Http.GetJson<List<ViewFields>>(UrlList["GetFields"]);
            if (result.Status != StatusCodes.Status200OK) return null;
            ViewFields = result.Data;
            return ViewFields;
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="data"></param>
        protected virtual async Task<bool> Delete(string url, ViewT data)
        {
            //删除单条
            var result = await Http.PostJson(url, data);
            if (result.Status != StatusCodes.Status200OK) return false;
            await Message.Success("删除成功");
            return true;
        }
        #endregion

        #region 回调
        [Inject]
        CavModal CavModal { get; set; }
        protected virtual async Task RowCallback(RowCallbackData<ViewT> row)
        {
            switch (row.Menu.Entity.MenuName)
            {
                case "删除":
                    await Delete(row.Menu.Entity.MenuName, row.Data);
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
            var result = await Http.PostJson<ViewQuery, PageData<ViewT>>(UrlList["FuzzyQuery"], Query);
            if (result.Status != StatusCodes.Status200OK) return;
            DataSource = result.Data.Rows;
            Total = result.Data.Total;
            PageIndex = result.Data.PageIndex;
            PageSize = result.Data.PageSize;
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
            await base.OnInitializedAsync();
            Loading = true;
            BaseController = CommonHelper.GetLeftText(Url, "/");
            LoadButton();//加载按钮
            await GetModelFields();//获取模型字段
            await GetPages();//获取数据源
            Loading = false;
        }
        #endregion
    }
}
