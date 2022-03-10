using AntDesign;
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
    public partial class AdvancedQuery
    {
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

        void AddCondition()
        {
            QueryView.QueryModels.Add(new QueryModel());
        }

        void RemoveCondition(Guid trackId)
        {
            var model = QueryView.QueryModels.Single(u => u.TrackId == trackId);
            QueryView.QueryModels.Remove(model);
            SelectItems.Remove(trackId);
        }

        void OnChange<T>(QueryModel queryModel, T value)
        {
            queryModel.Value = value.ToString();
            Console.WriteLine(queryModel.Value);
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
            if (QueryCallback.HasDelegate)
            {
                await QueryCallback.InvokeAsync(QueryView);
            }
        }
    }
}
