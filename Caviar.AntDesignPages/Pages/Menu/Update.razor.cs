using Caviar.AntDesignPages.Helper;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Pages.Menu
{
    public partial class Update : ITableTemplate
    {
        [Inject]
        HttpHelper Http { get; set; }

        DataTemplate Template { get; set; }

        public ViewMenu Model { get; set; }

        [Parameter]
        public int Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var result = await Http.GetJson<ViewMenu>("Menu/GetEntity?Id=" + Id);
            if (result.Status != 200) return;
            Model = result.Data;
        }

        public async Task<bool> Submit()
        {
            return await Template.Submit();
        }
    }
}
