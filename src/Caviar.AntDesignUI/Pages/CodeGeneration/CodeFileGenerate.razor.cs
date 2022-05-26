// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;

namespace Caviar.AntDesignUI.Pages.CodeGeneration
{
    public partial class CodeFileGenerate
    {
        [Inject]
        UserConfig UserConfig { get; set; }
        [Inject]
        CavLayout Layout { get; set; }
        [Inject]
        MessageService MessageService { get; set; }
        [Inject]
        HttpService HttpService { get; set; }
        public StepItem[] steps;
        protected override async Task OnInitializedAsync()
        {
            steps = new StepItem[]
            {
                new StepItem { Title = LanguageService[$"{ CurrencyConstant.Page }.{ CurrencyConstant.SelectGenerationModule}"], Content = "" },
                new StepItem { Title = LanguageService[$"{ CurrencyConstant.Page }.{ CurrencyConstant.FunctionConfiguration}"], Content = "" },
                new StepItem { Title = LanguageService[$"{ CurrencyConstant.Page }.{ CurrencyConstant.ViewCode}"], Content = "" },
                new StepItem { Title = LanguageService[$"{ CurrencyConstant.Page }.{ CurrencyConstant.GenerateResults}"], Content = "" }
            };
            if (Config.IsDebug)
            {
                await base.OnInitializedAsync();
            }
            else
            {
                await MessageService.Error(LanguageService[$"{CurrencyConstant.Page}.{CurrencyConstant.DebugErrorMsg}"]);
            }
        }


        protected override async Task<List<FieldsView>> GetPages(int pageIndex = 1, int pageSize = 10, bool isOrder = true)
        {
            var result = await HttpService.GetJson<List<FieldsView>>(UrlConfig.GetEntitys);
            if (result.Status != HttpStatusCode.OK) return null;
            TableOptions.Total = result.Data.Count;
            TableOptions.PageSize = result.Data.Count;
            return result.Data;
        }

        protected override Task RowCallback(RowCallbackData<FieldsView> row)
        {
            switch (row.Menu.Entity.MenuName)
            {
                //case "Menu Key"
                case "Select":
                    GenerateData.EntitieName = row.Data.Entity.FieldName;
                    GenerateData.FullName = row.Data.Entity.FullName;
                    OnNextClick();
                    break;
            }
            return base.RowCallback(row);
        }

        CodeGenerateOptions GenerateData = new CodeGenerateOptions();
        Form<CodeGenerateOptions> GenerateFrom;
        void OnPreClick()
        {
            current--;
        }

        async void OnNextClick()
        {
            if (current == 1)
            {
                if (!GenerateFrom.Validate())
                {
                    return;
                }
                var result = await HttpService.PostJson<CodeGenerateOptions, List<PreviewCode>>($"{UrlConfig.CodeFileGenerate}?isPerview=true", GenerateData);
                if (result.Status == HttpStatusCode.OK)
                {
                    lstTabs = result.Data;
                }
            }
            current++;
            StateHasChanged();
        }

        string ResultStatus = "";
        string ReusltTitle = "";
        string ResultSubTitle = "";
        async void OnGenerateClick()
        {
            var result = await HttpService.PostJson<CodeGenerateOptions, string>($"{UrlConfig.CodeFileGenerate}?isPerview=false", GenerateData);
            if (result.Status == HttpStatusCode.OK)
            {
                ResultStatus = "success";
                ReusltTitle = result.Title;
                ResultSubTitle = LanguageService[$"{CurrencyConstant.Page}.{CurrencyConstant.ResultSubTitle}"];
            }
            else
            {
                ResultStatus = "error";
                ReusltTitle = result.Title;
                foreach (var item in result.Detail)
                {
                    ResultSubTitle += $"{item.Key}:{item.Value}";
                }
            }
            OnNextClick();
        }


        List<PreviewCode> lstTabs { get; set; } = new List<PreviewCode>();
        string nKey { get; set; } = "1";
    }
}
