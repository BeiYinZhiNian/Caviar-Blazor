using AntDesign;

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;
using System.Threading.Tasks;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities.View;
using Caviar.SharedKernel.Entities;

namespace Caviar.AntDesignUI.Shared
{
    public partial class CavTableTemplate<TData>
    {
        [Inject]
        UserConfig UserConfig { get; set; }
        [Inject]
        public HttpService HttpService { get; set; }
        /// <summary>
        /// 表格
        /// </summary>
        [Parameter]
        public Table<TData> Table { get; set; }
        /// 固定右侧操作列，设为空则不固定
        /// </summary>
        [Parameter]
        public string Fixed { get; set; } = "right";

        [Parameter]
        public EventCallback<IEnumerable<TData>> SelectedRowsChanged { get; set; }

        [Parameter]
        public bool IsSelectedRows { get; set; }

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

        [Parameter]
        public string ScrollX { get; set; }
        /// <summary>
        /// 包含边界
        /// </summary>
        [Parameter]
        public bool Bordered { get; set; } = false;

        /// <summary>
        /// 当字符超过n个自动忽略显示，并且使用Tooltip文字提示
        /// 默认20个字符
        /// </summary>
        [Parameter]
        public int EllipsisLen { get; set; } = 20;
        /// <summary>
        /// 当发生忽略时显示的长度
        /// </summary>
        [Parameter]
        public int DisplayCount { get; set; } = 7;
        /// <summary>
        /// 操作按钮列所占最大宽度
        /// </summary>
        [Parameter]
        public string ActionColumnMaxWidth { get; set; } = "220";
        /// <summary>
        /// 操作按钮列所占最小宽度
        /// </summary>
        [Parameter]
        public string ActionColumnMinWidth { get; set; } = "100";
        /// <summary>
        /// 数据源
        /// </summary>
        [Parameter]
        public List<TData> DataSource { get; set; }
        /// <summary>
        /// 总计
        /// </summary>
        [Parameter]
        public int Total { get; set; }
        /// <summary>
        /// 当前页数
        /// </summary>
        [Parameter]
        public int PageIndex { get; set; }
        /// <summary>
        /// 页面大小
        /// </summary>
        [Parameter]
        public int PageSize { get; set; }
        /// <summary>
        /// 按钮
        /// </summary>
        [Parameter]
        public List<SysMenuView> Buttons { get; set; }
        /// <summary>
        /// 树形组件
        /// </summary>
        [Parameter]
        public Func<TData, IEnumerable<TData>> TreeChildren { get; set; } = _ => Enumerable.Empty<TData>();
        /// <summary>
        /// 模型字段
        /// </summary>
        [Parameter]
        public List<FieldsView> ViewFields { get; set; } = new List<FieldsView>();
        /// <summary>
        /// 翻页回调
        /// </summary>
        [Parameter]
        public EventCallback<PaginationEventArgs> PageIndexChanged { get; set; }
        /// <summary>
        /// 获取列表组件
        /// </summary>
        [Parameter]
        public Func<FieldsView, RenderFragment> GetTableItems { get; set; }
        /// <summary>
        /// 创建按钮回调
        /// </summary>
        [Parameter]
        public Func<SysMenuView,TData,RenderFragment> CreateButtons { get; set; }
        /// <summary>
        /// 获取搜索组件
        /// </summary>
        [Parameter]
        public Func<string, RenderFragment> GetQueryItems { get; set; }

        [Parameter]
        public EventCallback<RowCallbackData<TData>> RowCallback { get; set; }
        [Parameter]
        public bool Loading { get; set; }
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
                    var parameter = "";
                    if (menu.Entity.ButtonPosition == ButtonPosition.Row)
                    {
                        parameter = $"?parameter={HttpUtility.UrlEncode(JsonSerializer.Serialize(CurrRow.Data))}"; 
                    }
                    Navigation.NavigateTo(menu.Entity.Url + parameter);
                    break;
                case TargetType.EjectPage:
                    Dictionary<string,object> paramenter = new Dictionary<string, object>();
                    if (menu.Entity.ButtonPosition == ButtonPosition.Row)
                    {
                        //因为引用类型，这里进行一次转换，相当于深度复制
                        //否则更改内容然后取消，列表会发生改变
                        CurrRow.Data.AToB(out TData dataSource);
                        paramenter.Add("DataSource", dataSource);
                    }
                    paramenter.Add(CurrencyConstant.ControllerName, menu.Entity.Url);
                    await CavModal.Create(menu.Entity.Url, menu.DisplayName, HandleOk, paramenter);
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
            showTotal = ctx => $"{ctx.Range.Item1}-{ctx.Range.Item2} {UserConfig.LanguageService[$"{CurrencyConstant.Page}.{CurrencyConstant.Total}"]} {ctx.Total} {UserConfig.LanguageService[$"{CurrencyConstant.Page}.{CurrencyConstant.Record}"]}";
            if (PageIndex == 0)
            {
                PageIndex = 1;
            }
            if (PageSize == 0)
            {
                PageSize = 10;
            }
            return base.OnInitializedAsync();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (string.IsNullOrEmpty(ScrollX) && ViewFields?.Count != 0)
            {
                var count = ViewFields?.Count(u => u.Entity.IsPanel && u.IsPermission);
                ScrollX = (count.Value * 200).ToString();
            }
        }

        #region 搜索
        [Parameter]
        public bool IsOpenQuery { get; set; }

        [Parameter]
        public EventCallback<QueryView> FuzzyQueryCallback { get; set; }
        #endregion
    }
}
