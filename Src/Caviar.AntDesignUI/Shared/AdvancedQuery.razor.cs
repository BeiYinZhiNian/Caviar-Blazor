using AntDesign;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Shared
{
    public partial class AdvancedQuery
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

        Dictionary<Guid, FieldsView> SelectItems = new Dictionary<Guid, FieldsView>();

        void OnSelectItem(Guid trackId,string item)
        {
            var field = Fields.Single(u => u.Entity.FieldName == item);
            if (!SelectItems.TryAdd(trackId, field))
            {
                SelectItems[trackId] = field;
            }
        }

        QueryView QueryView { get; set; } = new QueryView()
        {
            QueryModels = new List<QueryModel>()
            {
                new QueryModel() { QuerTypes = QueryModel.QuerType.Contains },
            }
        };

        async void OnSearch()
        {
            Loading = true;
            if (QueryCallback.HasDelegate)
            {
                await QueryCallback.InvokeAsync(QueryView);
            }
            Loading = false;
        }
    }
}
