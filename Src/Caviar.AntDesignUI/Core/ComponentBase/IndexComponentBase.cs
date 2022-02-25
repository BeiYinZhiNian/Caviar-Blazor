using AntDesign;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Core
{
    public partial class IndexComponentBase<ViewT> : CavComponentBase where ViewT:class, new()
    {

        #region 属性
        /// <summary>
        /// 数据源
        /// </summary>
        protected List<ViewT> IndexDataSource { get; set; } = new List<ViewT>();
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
        protected List<FieldsView> ViewFields { get; set; } = new List<FieldsView>();
        public QueryView Query { get; set; }
        protected string BaseController { get; set; }
        
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
            var result = await HttpService.GetJson<PageData<ViewT>>($"{SubmitUrl}?pageIndex={pageIndex}&pageSize={pageSize}&isOrder={isOrder}");
            if (result.Status != HttpStatusCode.OK) return null;
            if (result.Data != null)
            {
                Total = result.Data.Total;
                PageIndex = result.Data.PageIndex;
                PageSize = result.Data.PageSize;
                return result.Data.Rows;
            }
            return null;
        }
        /// <summary>
        /// 加载按钮
        /// </summary>
        /// <returns></returns>
        protected virtual Task<List<SysMenuView>> LoadButton()
        {
            var queryButton = Url["FuzzyQuery"];
            if (queryButton != null)
            {
                Query = new QueryView();
            }
            var buttons = APIList.Where(u => u.Entity.ControllerName == ControllerName).ToList();
            return Task.FromResult(buttons);
        }
        /// <summary>
        /// 获取模型字段
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<List<FieldsView>> GetModelFields()
        {
            var result = await HttpService.GetJson<List<FieldsView>>(Url[CurrencyConstant.GetFieldsKey]);
            if (result.Status != HttpStatusCode.OK) return null;
            return result.Data;
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="data"></param>
        protected virtual async Task<bool> Delete(string url, ViewT data)
        {
            //删除单条
            var result = await HttpService.PostJson(url, data);
            if (result.Status != HttpStatusCode.OK) return false;
            _ = MessageService.Success("删除成功");
            return true;
        }
        #endregion

        #region 回调
        [Inject]
        CavModal CavModal { get; set; }
        protected virtual async Task RowCallback(RowCallbackData<ViewT> row)
        {
            switch (row.Menu.Entity.Key)
            {
                //case "Menu Key"
                case CurrencyConstant.DeleteEntityKey:
                    await Delete(Url[row.Menu.Entity.Key], row.Data);
                    break;
                case CurrencyConstant.UpdateEntityKey:
                    break;
                case CurrencyConstant.CreateEntityKey:
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
            var result = await HttpService.PostJson<QueryView, PageData<ViewT>>(Url["FuzzyQuery"], Query);
            if (result.Status != HttpStatusCode.OK) return;
            IndexDataSource = result.Data.Rows;
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
            IndexDataSource = await GetPages(args.Page, args.PageSize);
        }
        #endregion

        #region 重写
        protected override async Task OnInitializedAsync()
        {
            Loading = true;
            await base.OnInitializedAsync();
            var buttonTask = LoadButton();//加载按钮
            var fieldsTask = GetModelFields();//获取模型字段
            var pagesTask = GetPages();//获取数据源
            //先请求后获取结果，加快请求速度
            Buttons = await buttonTask;
            ViewFields = await fieldsTask;
            IndexDataSource = await pagesTask;
            Loading = false;
        }
        #endregion
    }
}
