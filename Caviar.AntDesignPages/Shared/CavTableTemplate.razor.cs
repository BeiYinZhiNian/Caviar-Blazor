using AntDesign;
using Caviar.Models.SystemData;
using Caviar.AntDesignPages.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Web;

namespace Caviar.AntDesignPages.Shared
{
    public partial class CavTableTemplate<TData>
    {
        [Parameter]
        public List<TData> DataSource { get; set; }
        [Parameter]
        public int Total { get; set; }
        [Parameter]
        public int PageIndex { get; set; }
        [Parameter]
        public int PageSize { get; set; }

        [Parameter]
        public List<ViewMenu> Buttons { get; set; }

        [Parameter]
        public string ModelName { get; set; }
        [Parameter]
        public Func<TData, IEnumerable<TData>> TreeChildren { get; set; } = _ => Enumerable.Empty<TData>();
        [Parameter]
        public List<ViewModelFields> ViewModelFields { get; set; }
        [Inject]
        HttpHelper Http { get; set; }
        [Parameter]
        public EventCallback<PaginationEventArgs> PageIndexChanged { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Query.QueryObj = CommonHelper.GetCavBaseType(typeof(TData)).Name;
            if (!string.IsNullOrEmpty(ModelName))
            {
                var modelNameList = await Http.GetJson<List<ViewModelFields>>("Permission/GetFields?modelName=" + ModelName);
                if (modelNameList.Status == 200)
                {
                    ViewModelFields = modelNameList.Data;
                }
            }
            
        }

        [Parameter]
        public EventCallback<RowCallbackData<TData>> RowCallback { get; set; }

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
        async void ButtonClick(ViewMenu menu, TData data)
        {
            CurrRow = new RowCallbackData<TData>()
            {
                Menu = menu,
                Data = data,
            };
            switch (menu.TargetType)
            {
                case TargetType.CurrentPage:
                    var parameter = "";
                    if (menu.ButtonPosition == ButtonPosition.Row)
                    {
                        parameter = $"?parameter={HttpUtility.UrlEncode(JsonSerializer.Serialize(CurrRow.Data))}"; 
                    }
                    Navigation.NavigateTo(menu.Url + parameter);
                    break;
                case TargetType.EjectPage:
                    List<KeyValuePair<string, object?>> paramenter = new List<KeyValuePair<string, object?>>();
                    if (menu.ButtonPosition == ButtonPosition.Row)
                    {
                        //因为引用类型，这里进行一次转换，相当于深度复制
                        //否则更改内容然后取消，列表会发生改变
                        CurrRow.Data.AToB(out TData dataSource);
                        paramenter.Add(new KeyValuePair<string, object?>("DataSource", dataSource));
                    }
                    paramenter.Add(new KeyValuePair<string, object?>("Url", menu.Url));
                    await CavModal.Create(menu.Url, menu.MenuName, HandleOk, paramenter);
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

        public async void HandleOk()
        {
            RoleAction(CurrRow);
        }

        #region 查询条件
        [Parameter]
        public bool IsOpenQuery { get; set; } = true;
        string HideQuery = "~hide~";//该字段是防止在刷新的过程中删除掉对象导致报错
        IEnumerable<string> _selectedValues;
        ViewQuery Query = new ViewQuery();
        void OnSelectedItemsChanged(IEnumerable<string> list)
        {
            if (list == null)
            {
                foreach (var item in Query.QueryData)
                {
                    Query.QueryData[item.Key] = HideQuery;
                }
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
            }
        }
        void OnRangeChange(DateRangeChangedEventArgs args)
        {
            Query.StartTime = args.Dates[0];
            Query.EndTime = args.Dates[1];
        }

        [Parameter]
        public EventCallback<ViewQuery> FuzzyQueryCallback { get; set; }

        /// <summary>
        /// 模糊搜索
        /// </summary>
        public async void FuzzyQuery()
        {
            if (FuzzyQueryCallback.HasDelegate)
            {
                await FuzzyQueryCallback.InvokeAsync(Query);
            }
            var result = await Http.PostJson<ViewQuery, List<TData>>(Query.QueryObj + "/FuzzyQuery", Query);
            if (result.Status != 200) return;
            DataSource = result.Data;
            StateHasChanged();
        }
        #endregion
    }
}
