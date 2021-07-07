using AntDesign;
using Caviar.AntDesignPages.Helper;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Pages.PersonalCenter
{
    public partial class Index
    {
        public ViewUser UserData { get; set; }
        [Inject]
        public HttpHelper Http { get; set; }
        [Inject]
        CavModal CavModal { get; set; }
        public string UploadUrl { get; set; }
        public Dictionary<string,string> Headers { get; set; }
        void OnClick(string icon)
        {
            Console.WriteLine($"icon {icon} is clicked");
        }

        async void UpdateUserData()
        {
            List<KeyValuePair<string, object?>> paramenter = new List<KeyValuePair<string, object?>>();
            //因为引用类型，这里进行一次转换，相当于深度复制
            //否则更改内容然后取消，列表会发生改变
            UserData.AToB(out ViewUser dataSource);
            paramenter.Add(new KeyValuePair<string, object?>("DataSource", dataSource));
            paramenter.Add(new KeyValuePair<string, object?>("Url", "User/MyDetails"));
            await CavModal.Create("User/MyDetailsUpdate", "修改信息", HandleOk, paramenter);

        }

        async void HandleOk()
        {
            await OnInitializedAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            UploadUrl = Http.Http.BaseAddress.AbsoluteUri + "Enclosure/Upload";
            Headers = new Dictionary<string, string>();
            var token = await Http.GetToken();
            Headers.Add(Http.TokenName, token);
            await base.OnInitializedAsync();
            var result = await Http.GetJson<ViewUser>("User/MyDetails");
            if (result.Status != 200) return;
            UserData = result.Data;
            StateHasChanged();
        }

        void OnSingleCompleted(UploadInfo fileinfo)
        {
            if (fileinfo.File.State == UploadState.Success)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                var result = fileinfo.File.GetResponse<ResultMsg<ViewEnclosure>>(options);
                
            }
        }
    }
}
