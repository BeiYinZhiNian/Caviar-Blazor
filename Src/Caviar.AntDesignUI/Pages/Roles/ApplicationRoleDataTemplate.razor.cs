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
    public partial class ApplicationRoleDataTemplate
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        [Inject]
        CavModal CavModal { get; set; }

        public async void CheckMenuType()
        {
            if(DataSource.Entity.DataRange == DataRange.Custom)
            {
                Dictionary<string, object> paramenter = new Dictionary<string, object>();
                paramenter.Add("DataSource", DataSource);
                paramenter.Add(CurrencyConstant.ControllerName, UrlConfig.DataRange);
                await CavModal.Create(UrlConfig.DataRange, "自定义数据范围", null, paramenter);
            }
        }
    }
}
