using AntDesign;
using Caviar.AntDesignUI.Shared;
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
        protected CatTableOptions<ViewT> TableOptions { get; set; } = new CatTableOptions<ViewT>() 
        { 
            DataSource = new List<ViewT>(),
            Buttons = new List<SysMenuView>(),
            ViewFields = new List<FieldsView>(),
        };
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
                TableOptions.Total = result.Data.Total;
                TableOptions.PageIndex = result.Data.PageIndex;
                TableOptions.PageSize = result.Data.PageSize;
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
            var queryButton = Url[CurrencyConstant.Query];
            if (queryButton != null)
            {
                TableOptions.IsOpenQuery = true;
                TableOptions.IsAdvancedQuery = true;
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
            _ = MessageService.Success(result.Title);
            return true;
        }
        #endregion

        #region 回调
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
        /// 查询
        /// </summary>
        /// <param name="Query"></param>
        protected virtual async Task QueryCallback(QueryView query)
        {
            var result = await HttpService.PostJson<QueryView, PageData<ViewT>>(Url[CurrencyConstant.Query], query);
            if (result.Status != HttpStatusCode.OK) return;
            TableOptions.DataSource = result.Data.Rows;
            TableOptions.Total = result.Data.Total;
            TableOptions.PageIndex = result.Data.PageIndex;
            TableOptions.PageSize = result.Data.PageSize;
            StateHasChanged();
        }
        /// <summary>
        /// 分页回调
        /// </summary>
        /// <param name="args"></param>
        protected virtual async Task PageIndexChanged(PaginationEventArgs args)
        {
            TableOptions.DataSource = await GetPages(args.Page, args.PageSize);
        }
        #endregion

        #region 重写
        protected override async Task OnInitializedAsync()
        {
            TableOptions.Loading = true;
            await base.OnInitializedAsync();
            var buttonTask = LoadButton();//加载按钮
            var fieldsTask = GetModelFields();//获取模型字段
            var pagesTask = GetPages();//获取数据源
            //先请求后获取结果，加快请求速度
            TableOptions.Buttons = await buttonTask;
            TableOptions.ViewFields = await fieldsTask;
            TableOptions.DataSource = await pagesTask;
            TableOptions.Loading = false;
        }
        #endregion
    }
}
