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
        public bool Loading { get; set; }
        /// <summary>
        /// 模型字段
        /// </summary>
        [Parameter]
        public List<FieldsView> Fields { get; set; }

        [Parameter]
        public EventCallback<QueryView> FuzzyQueryCallback { get; set; }

        protected QueryModel QueryModel { get; set; } = new QueryModel() { QuerType = QuerType.Contain };

        async void OnSearch()
        {
            Loading = true;
            QueryView query = new QueryView()
            {
                QueryModels = new List<QueryModel>() 
                {
                    QueryModel
                }
            };
            if (FuzzyQueryCallback.HasDelegate)
            {
                await FuzzyQueryCallback.InvokeAsync(query);
            }
            Loading = false;
        }
    }
}
