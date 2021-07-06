using Caviar.AntDesignPages.Helper;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Pages.PersonalCenter
{
    public partial class Index
    {
        public ViewUser UserData { get; set; }
        [Inject]
        public HttpHelper Http { get; set; }
        void OnClick(string icon)
        {
            Console.WriteLine($"icon {icon} is clicked");
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var result = await Http.GetJson<ViewUser>("User/GetDetails");
            if (result.Status != 200) return;
            UserData = result.Data;
        }
    }
}
