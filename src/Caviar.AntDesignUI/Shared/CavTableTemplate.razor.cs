// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;

namespace Caviar.AntDesignUI.Shared
{
    public partial class CavTableTemplate<TData>
    {
        [Parameter]
        public CatTableOptions<TData> TableOptions { get; set; }
        [Inject]
        UserConfig UserConfig { get; set; }
        [Inject]
        public HttpService HttpService { get; set; }

        [Parameter]
        public EventCallback<IEnumerable<TData>> SelectedRowsChanged { get; set; }



        [Parameter]
        public IEnumerable<TData> SelectedRows
        {
            get { return _selectedRows; }
            set
            {
                if (_selectedRows == value) return;
                _selectedRows = value;
                if (SelectedRowsChanged.HasDelegate)
                {
                    SelectedRowsChanged.InvokeAsync(value);
                }
            }
        }

        IEnumerable<TData> _selectedRows;
        /// <summary>
        /// 翻页回调
        /// </summary>
        [Parameter]
        public EventCallback<PaginationEventArgs> PageIndexChanged { get; set; }


        [Parameter]
        public EventCallback<RowCallbackData<TData>> RowCallback { get; set; }
        /// <summary>
        /// 是否为搜索状态
        /// </summary>
        [Parameter]
        public bool IsQueryState { get; set; }
        [Parameter]
        public EventCallback<bool> IsQueryStateChanged { get; set; }
        async void RoleAction(RowCallbackData<TData> data)
        {
            if (RowCallback.HasDelegate)
            {
                await RowCallback.InvokeAsync(data);
            }

        }
        [Inject]
        NavigationManager Navigation { get; set; }
        [Inject]
        CavModal CavModal { get; set; }
        [Inject]
        ILanguageService LanguageService { get; set; }
        RowCallbackData<TData> CurrRow { get; set; }
        async void ButtonClick(SysMenuView menu, TData data)
        {
            CurrRow = new RowCallbackData<TData>()
            {
                Menu = menu,
                Data = data,
            };
            switch (menu.Entity.TargetType)
            {
                case TargetType.CurrentPage:
                    var suffix = "";
                    var entity = CurrRow.Data.GetObjValue("entity");
                    if (entity == null)
                    {
                        entity = CurrRow.Data;
                    }
                    var id = entity.GetObjValue("Id");
                    if (id != null)
                    {
                        suffix = "/" + id.ToString();
                    }
                    Navigation.NavigateTo(menu.Entity.Url + suffix);
                    break;
                case TargetType.EjectPage:
                    Dictionary<string, object> paramenter = new Dictionary<string, object>();
                    if (menu.Entity.ButtonPosition == ButtonPosition.Row)
                    {
                        //因为引用类型，这里进行一次转换，相当于深度复制
                        //否则更改内容然后取消，列表会发生改变
                        CurrRow.Data.AToB(out TData dataSource);
                        paramenter.Add(CurrencyConstant.DataSource, dataSource);
                    }
                    paramenter.Add(CurrencyConstant.CurrentUrl, menu.Entity.Url);//不提供url时候默认url一致
                    var options = new CavModalOptions()
                    {
                        Url = menu.Entity.Url,
                        Paramenter = paramenter,
                        ActionOK = HandleOk,
                        Title = menu.DisplayName
                    };
                    await CavModal.Create(options);
                    break;
                case TargetType.NewLabel:
                    //await JSRuntime.InvokeVoidAsync("open", menu.Url, "_blank");
                    break;
                case TargetType.Callback:
                    RoleAction(CurrRow);
                    break;
                default:
                    break;
            }
        }

        public void HandleOk()
        {
            RoleAction(CurrRow);
        }

        Func<PaginationTotalContext, string> showTotal;
        protected override Task OnInitializedAsync()
        {
            showTotal = ctx => $"{ctx.Range.Item1}-{ctx.Range.Item2} {LanguageService[$"{CurrencyConstant.Page}.{CurrencyConstant.Total}"]} {ctx.Total} {LanguageService[$"{CurrencyConstant.Page}.{CurrencyConstant.Record}"]}";
            return base.OnInitializedAsync();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (string.IsNullOrEmpty(TableOptions.ScrollX) && TableOptions.ViewFields?.Count != 0)
            {
                var count = TableOptions.ViewFields?.Count(u => u.Entity.IsPanel && u.IsPermission);
                if (count == null)
                {
                    TableOptions.ScrollX = null;
                }
                else
                {
                    TableOptions.ScrollX = (count.Value * 200).ToString();
                }
            }
        }

        #region 搜索
        // 数据拷贝，等关闭搜索时，用于恢复数据
        private List<TData> _dataSourceCopy = null;
        private int _totalCopy = 0;
        private int _pageIndexCopy = 0;
        private int _pageSizeCopy = 0;
        [Parameter]
        public EventCallback<QueryView> QueryCallback { get; set; }

        public async Task QueryStart(QueryView query)
        {
            if (QueryCallback.HasDelegate)
            {
                if (!IsQueryState)
                {
                    IsQueryState = true;
                    //开始数据备份
                    _dataSourceCopy = TableOptions.DataSource;
                    _totalCopy = TableOptions.Total;
                    _pageIndexCopy = TableOptions.PageIndex;
                    _pageSizeCopy = TableOptions.PageSize;
                }
                await QueryCallback.InvokeAsync(query);
            }
        }

        private void CloseQuery()
        {
            //恢复数据
            TableOptions.DataSource = _dataSourceCopy;
            TableOptions.Total = _totalCopy;
            TableOptions.PageIndex = _pageIndexCopy;
            TableOptions.PageSize = _pageSizeCopy;
            IsQueryState = false;
            StateHasChanged();
        }
        #endregion
    }

    public class CatTableOptions<TData>
    {
        /// <summary>
        /// 表格
        /// </summary>
        public Table<TData> Table { get; set; }
        /// 固定右侧操作列，设为空则不固定
        /// </summary>
        public string Fixed { get; set; } = "right";
        /// <summary>
        /// 是否开启筛选
        /// </summary>
        public bool IsSelectedRows { get; set; }
        /// <summary>
        /// 总宽度
        /// </summary>
        public string ScrollX { get; set; }
        /// <summary>
        /// 包含边界
        /// </summary>
        public bool Bordered { get; set; } = false;

        /// <summary>
        /// 当字符超过n个自动忽略显示，并且使用Tooltip文字提示
        /// 默认20个字符
        /// </summary>
        public int EllipsisMaxLen { get; set; } = 20;
        /// <summary>
        /// Tooltip显示最大长度
        /// </summary>
        public int TooltipMaxLen { get; set; } = 100;
        /// <summary>
        /// 当发生忽略时显示的长度
        /// </summary>
        public int DisplayLen { get; set; } = 7;
        /// <summary>
        /// 操作按钮列所占最大宽度
        /// </summary>
        public string ActionColumnMaxWidth { get; set; } = "220";
        /// <summary>
        /// 操作按钮列所占最小宽度
        /// </summary>
        public string ActionColumnMinWidth { get; set; } = "100";
        /// <summary>
        /// 数据源
        /// </summary>
        public List<TData> DataSource { get; set; }
        /// <summary>
        /// 总计
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 当前页数
        /// </summary>
        public int PageIndex { get; set; } = 1;
        /// <summary>
        /// 页面大小
        /// </summary>
        public int PageSize { get; set; } = 10;
        /// <summary>
        /// 加载指示
        /// </summary>
        public bool Loading { get; set; }
        /// <summary>
        /// 是否开启查询
        /// </summary>
        public bool IsOpenQuery { get; set; }
        /// <summary>
        /// 开启高级查询
        /// </summary>
        public bool IsAdvancedQuery { get; set; }
        /// <summary>
        /// 按钮
        /// </summary>
        public List<SysMenuView> Buttons { get; set; }
        /// <summary>
        /// 树形组件
        /// </summary>
        public Func<TData, IEnumerable<TData>> TreeChildren { get; set; } = _ => Enumerable.Empty<TData>();
        /// <summary>
        /// 模型字段
        /// </summary>
        public List<FieldsView> ViewFields { get; set; } = new List<FieldsView>();
        /// <summary>
        /// 获取列表组件
        /// </summary>
        public Func<FieldsView, TData, RenderFragment> GetTableItems { get; set; }
        /// <summary>
        /// 创建按钮回调
        /// </summary>
        public Func<SysMenuView, TData, RenderFragment> CreateButtons { get; set; }
        /// <summary>
        /// 获取搜索组件
        /// </summary>
        public Func<RenderFragment> GetQueryItems { get; set; }
    }
}
