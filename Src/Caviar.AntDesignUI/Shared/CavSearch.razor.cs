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

        async void OnSearch()
        {
            Loading = true;
            if (string.IsNullOrEmpty(QueryModel.Key))
            {
                _ = MessageService.Error("请选择要查询的字段");
                return;
            }
            QueryView query = new QueryView()
            {
                QueryModels = new Dictionary<Guid, QueryModel>() 
                {
                    {Guid.NewGuid(),QueryModel }
                }

            };
            if (QueryCallback.HasDelegate)
            {
                await QueryCallback.InvokeAsync(query);
            }
            Loading = false;
        }
    }
}
