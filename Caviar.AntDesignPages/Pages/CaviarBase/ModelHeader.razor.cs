using Caviar.AntDesignPages.Helper;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Pages.CaviarBase
{
    public partial class ModelHeader
    {
        List<ViewModelHeader> Models = new List<ViewModelHeader>();
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        HttpHelper Http { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await GetModels();
        }


        public async Task GetModels()
        {
            var result = await Http.GetJson<List<ViewModelHeader>>("ModelHeader/GetModels");
            if (result.Status != 200) return;
            Models = result.Data;
        }

        string editId;
        void startEdit(string id)
        {
            editId = id;
        }

        void stopEdit()
        {
            editId = null;
        }


    }
}
