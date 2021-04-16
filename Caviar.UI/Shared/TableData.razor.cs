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
        public string ModelName { get; set; }
        [Parameter]
        public Func<TData, IEnumerable<TData>> TreeChildren { get; set; } = _ => Enumerable.Empty<TData>();

        [Inject]
        HttpHelper Http { get; set; }

        List<ViewModelName> ViewModelName { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var modelNameList = await Http.GetJson<List<ViewModelName>>("Base/GetModelName?name=" + ModelName);
            if (modelNameList.Status == 200)
            {
                ViewModelName = modelNameList.Data;
            }
        }
    }
}
