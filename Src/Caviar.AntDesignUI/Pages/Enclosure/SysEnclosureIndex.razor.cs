using AntDesign;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Pages.Enclosure
{
    public partial class SysEnclosureIndex
    {
        protected override Task RowCallback(RowCallbackData<SysEnclosureView> row)
        {
            switch (row.Menu.Entity.Key)
            {
                //case "Menu Key"
                case CurrencyConstant.UploadKey:
                    break;
            }
            return base.RowCallback(row);
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
