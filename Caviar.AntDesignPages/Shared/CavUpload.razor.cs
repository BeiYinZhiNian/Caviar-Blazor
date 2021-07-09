using AntDesign;
using Caviar.AntDesignPages.Helper;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Shared
{
    public partial class CavUpload:Upload
    {
        [Inject]
        public HttpHelper Http { get; set; }
        [Parameter]
        public EventCallback<ViewEnclosure> OnEnclosureCallback { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            OnSingleCompleted = EventCallback.Factory.Create<UploadInfo>(this, SingleCallback);
            Action = Http.Http.BaseAddress.AbsoluteUri + "Enclosure/Upload";
            Headers = new Dictionary<string, string>();
            var token = await Http.GetToken();
            Headers.Add(Http.TokenName, token);
        }

        public void SingleCallback(UploadInfo fileinfo)
        {
            if (fileinfo.File.State == UploadState.Success)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                var result = fileinfo.File.GetResponse<ResultMsg<ViewEnclosure>>(options);
                Http.Response(result);
                if (result.Status != HttpState.OK) return;
                result.Data.Path = Http.Http.BaseAddress.AbsoluteUri.Replace("/api/","") + CurrencyConstant.Enclosure + "/" + CurrencyConstant.HeadPortrait + "/" + result.Data.Name;
                if (OnEnclosureCallback.HasDelegate)
                {
                    OnEnclosureCallback.InvokeAsync(result.Data);//回传服务器结果
                }
            }
        }
    }
}
