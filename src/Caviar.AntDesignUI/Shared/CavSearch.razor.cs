// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Collections.Generic;
using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;

namespace Caviar.AntDesignUI.Shared
{
    public partial class CavSearch
    {
        private bool Loading { get; set; }
        /// <summary>
        /// 模型字段
        /// </summary>
        [Parameter]
        public List<FieldsView> Fields { get; set; }
        [Inject]
        MessageService MessageService { get; set; }
        [Inject]
        UserConfig UserConfig { get; set; }
        [Inject]
        ILanguageService LanguageService { get; set; }
        [Parameter]
        public EventCallback<QueryView> QueryCallback { get; set; }
        [Parameter]
        public QueryModel QueryModel { get; set; } = new QueryModel() { QuerTypes = QueryModel.QuerType.Contains };
        /// <summary>
        /// 是否为搜索状态
        /// </summary>
        [Parameter]
        public bool IsQueryState { get; set; }
        [Parameter]
        public EventCallback<bool> IsQueryStateChanged { get; set; }
        async void OnSearch()
        {
            if (string.IsNullOrEmpty(QueryModel.Key))
            {
                _ = MessageService.Error(LanguageService[$"{CurrencyConstant.Page}.{CurrencyConstant.SelectQueryFields}"]);
                return;
            }
            Loading = true;
            QueryView query = new QueryView()
            {
                QueryModels = new Dictionary<Guid, QueryModel>()
                {
                    {Guid.NewGuid(),QueryModel }
                }

            };
            if (QueryCallback.HasDelegate)
            {
                if (IsQueryStateChanged.HasDelegate)
                {
                    await IsQueryStateChanged.InvokeAsync(IsQueryState);
                }
                await QueryCallback.InvokeAsync(query);
            }
            Loading = false;
            StateHasChanged();
        }
    }
}
