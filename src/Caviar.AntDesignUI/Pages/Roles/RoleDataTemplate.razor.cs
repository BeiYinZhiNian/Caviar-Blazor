// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Collections.Generic;
using System.Threading.Tasks;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Microsoft.AspNetCore.Components;

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
        /// <summary>
        /// 标识CheckMenuType是否为首次触发
        /// </summary>
        bool IsCheckMenuOne;

        public async void CheckMenuType(DataRange dataRange)
        {
            if (!IsCheckMenuOne)
            {
                IsCheckMenuOne = true;
                return;
            }
            if (dataRange == DataRange.Custom)
            {
                Dictionary<string, object> paramenter = new Dictionary<string, object>();
                paramenter.Add(CurrencyConstant.DataSource, DataSource);
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
