using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Pages.Roles
{
    public partial class RoleDataTemplate
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        [Inject]
        CavModal CavModal { get; set; }

        public async void CheckMenuType(DataRange dataRange)
        {
            if(dataRange == DataRange.Custom)
            {
                Dictionary<string, object> paramenter = new Dictionary<string, object>();
                paramenter.Add("DataSource", DataSource);
                paramenter.Add(CurrencyConstant.ControllerName, UrlConfig.DataRange);
                CavModalOptions options = new CavModalOptions()
                {
                    Url = UrlConfig.DataRange,
                    Title = "自定义数据范围",
                    Paramenter = paramenter
                };
                await CavModal.Create(options);
            }
        }
    }
}
