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

        [Inject]
        HttpHelper Http { get; set; }

        List<ViewModelName> ViewPowerMenus { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var modelNameList = await Http.GetJson<List<ViewModelName>>("Base/GetModelName?name=SyspowerMenu");
            ViewPowerMenus = modelNameList.Data;
        }
    }
}
