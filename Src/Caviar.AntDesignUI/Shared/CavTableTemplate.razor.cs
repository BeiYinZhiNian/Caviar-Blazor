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
        /// <summary>
        /// http帮助类
        /// </summary>
        [Inject]
        HttpService Http { get; set; }
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
        public string ScrollX { get; set; }
        /// <summary>
        /// 包含边界
        /// </summary>
        [Parameter]
        public bool Bordered { get; set; } = false;

        [Inject]
        MessageService MessageService { get; set; }

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
        /// 模型名称
        /// </summary>
        [Parameter]
        public string ModelName { get; set; }
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

        protected override Task OnInitializedAsync()
        {
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
                var count = ViewFields?.Count(u => u.Entity.IsPanel);
                if(count > 5)
                {
                    ScrollX = (count.Value * 200).ToString();
                }
            }
        }

        #region 查询条件
        static string HideQuery = Guid.NewGuid().ToString();//该字段是防止在刷新的过程中删除掉对象导致报错
        IEnumerable<string> _selectedValues;
        [Parameter]
        public QueryView Query { get; set; }
        void OnSelectedItemsChanged(IEnumerable<string> list)
        {
            if (list == null)
            {
                //清空查询，重置查询
                foreach (var item in Query.QueryData)
                {
                    Query.QueryData[item.Key] = HideQuery;
                }
                QuerySubstitution(false);
                return;
            }
            var selectCount = list.Count();
            var CurrQuery = Query.QueryData.Where(u => u.Value != HideQuery);
            var count = selectCount - CurrQuery.Count();
            if(count > 0)
            {
                var item = _selectedValues.Last();
                if (Query.QueryData.ContainsKey(item))
                {
                    Query.QueryData[item] = "";
                }
                else
                {
                    Query.QueryData.Add(item, "");
                }
            }
            else if(count < 0)
            {
                var keys = Query.QueryData.Keys.Except(list);
                foreach (var item in keys)
                {
                    Query.QueryData[item] = HideQuery;
                }
                StateHasChanged();
            }
        }
        void OnRangeChange(DateRangeChangedEventArgs args)
        {
            Query.StartTime = args.Dates[0];
            Query.EndTime = args.Dates[1];
        }

        [Parameter]
        public EventCallback FuzzyQueryCallback { get; set; }
        [Parameter]
        public Dictionary<string,string> MappingQuery { get; set; }

        private List<TData> CacheDataSource { get; set; }
        private int CacheTotal { get; set; }
        private int CachePageIndex { get; set; }
        private int CachePageSize { get; set; }
        /// <summary>
        /// 是否在查询中
        /// </summary>
        public bool IsQuery { get; set; }

        /// <summary>
        /// 模糊搜索
        /// </summary>
        public async void FuzzyQuery()
        {

            var currQuery = new Dictionary<string, string>();
            foreach (var item in Query.QueryData)
            {
                if(item.Value != HideQuery)
                {
                    string key = item.Key;
                    string value = item.Value;//value不需要变
                    if (MappingQuery != null)
                    {
                        if (MappingQuery.ContainsKey(key))
                        {
                            key = MappingQuery[key];//将映射字段替换为需要的key
                        }
                    }
                    currQuery.Add(key, value);
                }
            }
            if (currQuery.Count == 0)
            {
                await MessageService.Warn("请至少选择一个字段进行查询");
                return;
            }
            QuerySubstitution(true);
            Query.QueryData = currQuery;
            if (FuzzyQueryCallback.HasDelegate)
            {
                await FuzzyQueryCallback.InvokeAsync();
            }
        }
        /// <summary>
        /// 缓存查询状态或者恢复查询前状态
        /// </summary>
        /// <param name="startQuery"></param>
        private void QuerySubstitution(bool startQuery)
        {
            if (!IsQuery && startQuery)//开始查询并且不在查询中
            {
                IsQuery = true;//在查询状态
                CacheDataSource = DataSource;
                CachePageIndex = PageIndex;
                CachePageSize = PageSize;
                CacheTotal = Total;
            }
            else if (IsQuery && !startQuery)//正在查询中,且停止查询
            {
                IsQuery = false;//停止查询状态
                DataSource = CacheDataSource;
                PageIndex = CachePageIndex;
                PageSize = CachePageSize;
                Total = CacheTotal;
            }
        }
        #endregion
    }
}
