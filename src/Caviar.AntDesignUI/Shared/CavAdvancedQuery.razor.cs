// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;

namespace Caviar.AntDesignUI.Shared
{
    public partial class CavAdvancedQuery : ITableTemplate
    {
        [Parameter]
        public string CurrentUrl { get; set; }
        /// <summary>
        /// 模型字段
        /// </summary>
        [Parameter]
        public List<FieldsView> Fields { get; set; }
        [Parameter]
        public EventCallback<QueryView> QueryCallback { get; set; }
        [Parameter]
        public QueryView QueryView { get; set; }
        [Inject]
        MessageService MessageService { get; set; }
        [Inject]
        UserConfig UserConfig { get; set; }
        [Inject]
        ILanguageService LanguageService { get; set; }


        void OnSelectItem(Guid trackId, string item)
        {
            var field = Fields.Single(u => u.Entity.FieldName == item);
            var model = QueryView.QueryModels[trackId];
            model.ComponentStatus.Field = field;
        }



        void AddCondition()
        {
            var model = new QueryModel() { ComponentStatus = new ComponentStatus() };
            QueryView.QueryModels.Add(Guid.NewGuid(), model);

        }

        void RemoveCondition(Guid trackId)
        {
            QueryView.QueryModels.Remove(trackId);
        }

        void OnValueChange<T>(QueryModel queryModel, T value)
        {
            queryModel.Value = value.ToString();
            Console.WriteLine(queryModel.Value);
        }


        void OnDateTimeChange(QueryModel queryModel, DateTimeChangedEventArgs dateTimeChanged)
        {
            OnValueChange(queryModel, dateTimeChanged.DateString);
        }

        void AndOr(QueryModel queryModel, bool value)
        {
            if (value)
            {
                queryModel.QuerySplicings = QueryModel.QuerySplicing.And;
            }
            else
            {
                queryModel.QuerySplicings = QueryModel.QuerySplicing.Or;
            }
        }


        /// <summary>
        /// 开始搜索
        /// </summary>
        /// <returns></returns>
        async Task OnSearch()
        {
            if (QueryCallback.HasDelegate)
            {
                await QueryCallback.InvokeAsync(QueryView);
            }
        }

        protected override void OnInitialized()
        {
            if (QueryView == null)
            {
                QueryView = new QueryView();
            }
            if (QueryView.QueryModels == null)
            {
                QueryView.QueryModels = new Dictionary<Guid, QueryModel>();
            }
            if (QueryView.QueryModels.Count == 0)
            {
                AddCondition();
            }
            base.OnInitialized();
        }

        public async Task<bool> Validate()
        {
            foreach (var item in QueryView.QueryModels)
            {
                if (string.IsNullOrEmpty(item.Value.Key))
                {
                    _ = MessageService.Error(LanguageService[$"{CurrencyConstant.Page}.{CurrencyConstant.SelectQueryFields}"]);
                    return false;
                }
            }
            await OnSearch();
            return true;
        }
    }


}
