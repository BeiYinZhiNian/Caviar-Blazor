using Caviar.Models.SystemData;
using Caviar.UI.Helper;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.UI.Shared
{
    public partial class NewComponents
    {
        [Parameter]
        public string ModelHeader { get; set; }

        [Inject]
        HttpHelper Http { get; set; }

        [Parameter]
        public List<ViewModelHeader> ViewModelHeader { get; set; }
        protected override async Task OnInitializedAsync()
        {
            if (!string.IsNullOrEmpty(ModelHeader))
            {
                var modelNameList = await Http.GetJson<List<ViewModelHeader>>("Base/GetModelHeader?name=" + ModelHeader);
                if (modelNameList.Status == 200)
                {
                    ViewModelHeader = modelNameList.Data;
                }
            }

        }
    }
}
