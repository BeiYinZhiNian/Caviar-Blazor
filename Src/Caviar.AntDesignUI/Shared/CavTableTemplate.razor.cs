using AntDesign;
using Caviar.SharedKernel.View;
using Caviar.AntDesignUI.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;
using Caviar.SharedKernel;
using Caviar.SharedKernel.Entities.View;

namespace Caviar.AntDesignUI.Shared
{
    public partial class CavTableTemplate<TData>
    {
        /// <summary>
        /// http帮助类
        /// </summary>
        [Inject]
        HttpHelper Http { get; set; }
        [Inject]
        MessageService MessageService { get; set; }
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
        public List<ViewFields> ViewFields { get; set; }
        /// <summary>
        /// 翻页回调
        /// </summary>
        [Parameter]
        public EventCallback<PaginationEventArgs> PageIndexChanged { get; set; }
        /// <summary>
        /// 获取列表组件
        /// </summary>
        [Parameter]
        public Func<ViewFields, RenderFragment> GetTableItems { get; set; }
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
        IJSRuntime JSRuntime { get; set; }
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
                    paramenter.Add("Url", menu.Entity.Url);
                    await CavModal.Create(menu.Entity.Url, menu.Entity.MenuName, HandleOk, paramenter);
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

        #region 查询条件
        static string HideQuery = Guid.NewGuid().ToString();//该字段是防止在刷新的过程中删除掉对象导致报错
        IEnumerable<string> _selectedValues;
        [Parameter]
        public ViewQuery Query { get; set; }
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
