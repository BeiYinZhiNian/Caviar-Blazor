using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Shared
{
    public partial class CavSearch
    {
        private bool Loading { get; set; }
        /// <summary>
        /// 模型字段
        /// </summary>
        [Parameter]
        public List<FieldsView> Fields { get; set; }
        [Inject]
        MessageService MessageService { get; set; }
        [Parameter]
        public EventCallback<QueryView> QueryCallback { get; set; }
        [Parameter]
        public QueryModel QueryModel { get; set; } = new QueryModel() { QuerTypes = QueryModel.QuerType.Contains };
        /// <summary>
        /// 是否为搜索状态
        /// </summary>
        [Parameter]
        public bool IsQueryState { get; set; }
        [Parameter]
        public EventCallback<bool> IsQueryStateChanged { get; set; }
        async void OnSearch()
        {
            if (string.IsNullOrEmpty(QueryModel.Key))
            {
                _ = MessageService.Error("请选择要查询的字段");
                return;
            }
            Loading = true;
            QueryView query = new QueryView()
            {
                QueryModels = new Dictionary<Guid, QueryModel>() 
                {
                    {Guid.NewGuid(),QueryModel }
                }

            };
            if (QueryCallback.HasDelegate)
            {
                if (IsQueryStateChanged.HasDelegate)
                {
                    await IsQueryStateChanged.InvokeAsync(IsQueryState);
                }
                await QueryCallback.InvokeAsync(query);
            }
            Loading = false;
            StateHasChanged();
        }
    }
}
