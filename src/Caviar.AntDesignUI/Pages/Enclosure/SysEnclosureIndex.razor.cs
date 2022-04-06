using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Pages.Enclosure
{
    public partial class SysEnclosureIndex
    {
        [Inject]
        HttpService HttpService { get; set; }
        [Inject]
        MessageService MessageService { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        protected override Task OnInitializedAsync()
        {
            TableOptions.CreateButtons = CreateButtons;
            return base.OnInitializedAsync();
        }
        protected override async Task RowCallback(RowCallbackData<SysEnclosureView> row)
        {
            switch (row.Menu.Entity.MenuName)
            {
                //case "Menu Key"
                case CurrencyConstant.DownloadKey:
                    var result = await HttpService.PostJson<SysEnclosureView, string>(UrlConfig.Download, row.Data);
                    if(result.Status == System.Net.HttpStatusCode.OK)
                    {
                        _ = JSRuntime.InvokeVoidAsync("open_blank", result.Data);
                    }
                    break;
            }
            await base.RowCallback(row);
        }

        void OnSingleCompleted(UploadInfo fileinfo)
        {
            if (fileinfo.File.State == UploadState.Success)
            {
                var result = fileinfo.File.GetResponse<ResultMsg>();
                if(result.Status == System.Net.HttpStatusCode.OK)
                {
                    MessageService.Success(result.Title);
                    Refresh();
                }
                else
                {
                    MessageService.Error(result.Title);
                }
            }
        }

    }
}
