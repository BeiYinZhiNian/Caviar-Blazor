using AntDesign;
using Caviar.SharedKernel.View;
using Caviar.AntDesignUI.Helper;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;

namespace Caviar.AntDesignUI.Pages.Code
{
    public partial class CodeFileGenerate
    {

        List<ViewFields> Entitys { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        HttpHelper Http { get; set; }
        [Inject]
        MessageService _message { get; set; }
        public string Url { get; set; }
        protected override async Task OnInitializedAsync()
        {
            #if DEBUG
            await GetModels();
            Url = NavigationManager.Uri.Replace(NavigationManager.BaseUri, "");
            #else
            _message.Error("代码生成只能在debug模式下进行！");
            #endif
        }


        public async Task GetModels()
        {
            var result = await Http.GetJson<List<ViewFields>>("Permission/GetEntitys");
            if (result.Status != StatusCodes.Status200OK) return;
            Entitys = result.Data;
        }
        static string[] _pageOptions = { "列表","数据模板" };
        static string[] _webApi = { "控制器" ,"模型", "模型操作器" };
        static string[] _configOptions = { "覆盖" , "创建按钮" };
        void OnPageChange(string[] checkedValues)
        {
            GenerateData.Page = checkedValues;
        }

        void OnConfigChange(string[] checkedValues)
        {
            GenerateData.Config = checkedValues;
        }

        void OnWebApiChange(string[] checkedValues)
        {
            GenerateData.WebApi = checkedValues;
        }


        CodeGenerateOptions GenerateData = new CodeGenerateOptions() { 
            Page = _pageOptions,
            WebApi = _webApi ,Config = new string[] { "创建按钮" } 
        };
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
                if((GenerateData.Page == null || GenerateData.Page.Length == 0) && (GenerateData.WebApi == null || GenerateData.WebApi.Length == 0))
                {
                    await _message.Error("请在前后端至少选择一个进行生成");
                    return;
                }
                var result = await Http.PostJson<CodeGenerateOptions,List<PreviewCode>>($"{Url}?isPerview=true", GenerateData);
                if (result.Status == StatusCodes.Status200OK)
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
            var result = await Http.PostJson<CodeGenerateOptions, List<PreviewCode>>($"{Url}?isPerview=false", GenerateData);
            if (result.Status == StatusCodes.Status200OK)
            {
                ResultStatus = "success";
                ReusltTitle = "代码生成完毕";
                ResultSubTitle = "代码生效需要关闭程序重新编译运行";
            }
            else
            {
                ResultStatus = "error";
                ReusltTitle = result.Title;
                ResultSubTitle = result.Detail;
            }
            OnNextClick();
        }


        List<PreviewCode> lstTabs { get; set; } = new List<PreviewCode>();
        string nKey { get; set; } = "1";
    }
}
