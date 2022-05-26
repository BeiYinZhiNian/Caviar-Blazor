// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AntDesign;
using Caviar.AntDesignUI.Shared;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;

namespace Caviar.AntDesignUI.Core
{
    public partial class IndexComponentBase<ViewT> : ComponentBase, IReuseTabsPage where ViewT : class, new()
    {

        #region 属性
        protected CatTableOptions<ViewT> TableOptions { get; set; } = new CatTableOptions<ViewT>()
        {
            DataSource = new List<ViewT>(),
            Buttons = new List<SysMenuView>(),
            ViewFields = new List<FieldsView>(),
        };
        [Inject]
        private UserConfig UserConfig { get; set; }
        [Inject]
        private HttpService HttpService { get; set; }
        [Inject]
        private MessageService MessageService { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        protected ILanguageService LanguageService { get; set; }
        /// <summary>
        /// 当前Api列表
        /// </summary>
        public List<SysMenuView> APIList { get; set; }

        [Parameter]
        public string IndexUrl { get; set; }
        #endregion

        #region 方法

        protected string GetPageUrl(string key)
        {
            try
            {
                var controllerName = IndexUrl.Split('/')[0];
                return $"{controllerName}/{key}";
            }
            catch
            {
                MessageService.Error("Index Url解析错误");
                return null;
            }
        }
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="isOrder"></param>
        /// <returns></returns>
        protected virtual async Task<List<ViewT>> GetPages(int pageIndex = 1, int pageSize = 10, bool isOrder = true)
        {
            var result = await HttpService.GetJson<PageData<ViewT>>($"{IndexUrl}?pageIndex={pageIndex}&pageSize={pageSize}&isOrder={isOrder}");
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
            var queryButton = APIList.SingleOrDefault(u => !string.IsNullOrEmpty(u.Entity.Url) && u.Entity.Url.Contains(CurrencyConstant.Query));
            if (queryButton != null)
            {
                TableOptions.IsOpenQuery = true;
                TableOptions.IsAdvancedQuery = true;
            }
            var buttons = APIList.Where(u => u.Entity.MenuType == MenuType.Button).ToList();
            return Task.FromResult(buttons);
        }
        /// <summary>
        /// 获取模型字段
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<List<FieldsView>> GetModelFields()
        {
            var result = await HttpService.GetJson<List<FieldsView>>(GetPageUrl(CurrencyConstant.GetFieldsKey));
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

        public virtual RenderFragment GetPageTitle() => builder =>
        {
            var menu = UserConfig.Menus.FirstOrDefault(u => u.Entity.Url == IndexUrl);
            if (menu != null)
            {
                var index = 0;
                builder.OpenElement(index++, "div");
                if (!string.IsNullOrEmpty(menu.Entity.Icon))
                {
                    IEnumerable<KeyValuePair<string, object>> paramenter = new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("Type",menu.Entity.Icon)
                    };

                    builder.OpenComponent(index++, typeof(Icon));
                    builder.AddMultipleAttributes(index++, paramenter);
                    builder.CloseComponent();
                }
                builder.AddMarkupContent(index++, menu.DisplayName);
                builder.CloseElement();
            }
            else
            {
                var index = 0;
                builder.OpenElement(index++, "div");
                builder.AddMarkupContent(index++, IndexUrl);
                builder.CloseElement();
            }
        };

        /// <summary>
        /// 刷新
        /// </summary>
        /// <returns></returns>
        public virtual async void Refresh()
        {
            await OnInitializedAsync();
            StateHasChanged();
        }
        #endregion

        #region 回调
        /// <summary>
        /// 获取API
        /// 获取该页面下的API
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<List<SysMenuView>> GetApiList()
        {
            var menus = await HttpService.GetJson<List<SysMenuView>>($"{UrlConfig.GetApis}?indexUrl={IndexUrl}");
            if (menus.Status == HttpStatusCode.OK)
            {
                return menus.Data;
            }
            return new List<SysMenuView>();
        }
        protected virtual async Task RowCallback(RowCallbackData<ViewT> row)
        {
            switch (row.Menu.Entity.MenuName)
            {
                //case "Menu Key"
                case CurrencyConstant.DeleteEntityKey:
                    await Delete(GetPageUrl(row.Menu.Entity.MenuName), row.Data);
                    TableOptions.DataSource.Remove(row.Data);
                    return;
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
            var result = await HttpService.PostJson<QueryView, PageData<ViewT>>(GetPageUrl(CurrencyConstant.Query), query);
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
            UserConfig.RefreshCurrentPage = Refresh;
            if (string.IsNullOrEmpty(IndexUrl))
            {
                IndexUrl = NavigationManager.Uri;
                var uri = new Uri(IndexUrl);
                IndexUrl = uri.LocalPath[1..];
            }
            TableOptions.Loading = true;
            await base.OnInitializedAsync();
            var pagesTask = GetPages();//获取数据源
            var fieldsTask = GetModelFields();//获取模型字段
            APIList = await GetApiList();
            var buttonTask = LoadButton();//加载按钮,按钮加载需要等待apiList加载完毕后加载
            //先请求后获取结果，加快请求速度
            TableOptions.Buttons = await buttonTask;
            TableOptions.ViewFields = await fieldsTask;
            TableOptions.DataSource = await pagesTask;
            TableOptions.Loading = false;
        }
        #endregion
    }
}
