using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Shared
{
    public partial class AdvancedQuery: ITableTemplate
    {
        /// <summary>
        /// 模型字段
        /// </summary>
        [Parameter]
        public List<FieldsView> Fields { get; set; }
        [Inject]
        MessageService MessageService { get; set; }
        [Inject]
        UserConfig UserConfig { get; set; }
        [Parameter]
        public EventCallback<QueryView> QueryCallback { get; set; }
        [Parameter]
        public QueryView QueryView { get; set; }

        /// <summary>
        /// 是否为搜索状态
        /// </summary>
        [Parameter]
        public bool IsQueryState { get; set; }
        [Parameter]
        public EventCallback<bool> IsQueryStateChanged { get; set; }


        void OnSelectItem(Guid trackId,string item)
        {
            var field = Fields.Single(u => u.Entity.FieldName == item);
            var model = QueryView.QueryModels[trackId];
            model.ComponentStatus.Field = field;
        }

        void AddCondition()
        {
            var model = new QueryModel() { ComponentStatus = new ComponentStatus()};
            QueryView.QueryModels.Add(Guid.NewGuid(),model);
            
        }

        void RemoveCondition(Guid trackId)
        {
            QueryView.QueryModels.Remove(trackId);
        }

        void OnValueChange<T>(QueryModel queryModel,T value)
        {
            queryModel.Value = value.ToString();
            Console.WriteLine(queryModel.Value);
        }

        void OnDateTimeChange(QueryModel queryModel, DateTimeChangedEventArgs dateTimeChanged)
        {
            OnValueChange(queryModel, dateTimeChanged.DateString);
        }

        void AndOr(QueryModel queryModel, bool value)
        {
            if (value)
            {
                queryModel.QuerySplicings = QueryModel.QuerySplicing.And;
            }
            else
            {
                queryModel.QuerySplicings = QueryModel.QuerySplicing.Or;
            }
        }
        

        /// <summary>
        /// 开始搜索
        /// </summary>
        /// <returns></returns>
        async Task OnSearch()
        {
            if (QueryCallback.HasDelegate)
            {
                IsQueryState = true;
                if (IsQueryStateChanged.HasDelegate)
                {
                    await IsQueryStateChanged.InvokeAsync(IsQueryState);
                }
                await QueryCallback.InvokeAsync(QueryView);
            }
        }

        protected override void OnInitialized()
        {
            if(QueryView == null)
            {
                QueryView = new QueryView();
            }
            if(QueryView.QueryModels == null)
            {
                QueryView.QueryModels = new Dictionary<Guid, QueryModel>();
            }
            if(QueryView.QueryModels.Count == 0)
            {
                AddCondition();
            }
            base.OnInitialized();
        }

        public async Task<bool> Validate()
        {
            foreach (var item in QueryView.QueryModels)
            {
                if (string.IsNullOrEmpty(item.Value.Key))
                {
                    _ = MessageService.Error("请选择要查询的字段");
                    return false;
                }
            }
            await OnSearch();
            return true;
        }
    }

    
}
