using Caviar.Models.SystemData;
using Caviar.UI.Helper;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.UI.Shared
{
    public partial class TableData<TData>
    {
        [Parameter]
        public List<TData> DataSource { get; set; }
        [Parameter]
        public string ModelHeaderName { get; set; }
        [Parameter]
        public Func<TData, IEnumerable<TData>> TreeChildren { get; set; } = _ => Enumerable.Empty<TData>();
        [Parameter]
        public List<ViewModelHeader> ViewModelHeader { get; set; }
        [Inject]
        HttpHelper Http { get; set; }
        

        protected override async Task OnInitializedAsync()
        {
            if (!string.IsNullOrEmpty(ModelHeaderName))
            {
                var modelNameList = await Http.GetJson<List<ViewModelHeader>>("Base/GetModelHeader?name=" + ModelHeaderName);
                if (modelNameList.Status == 200)
                {
                    ViewModelHeader = modelNameList.Data;
                }
            }
            
        }
    }
}
