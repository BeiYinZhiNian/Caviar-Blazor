using AntDesign;
using Caviar.SharedKernel;
using Caviar.AntDesignUI.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace Caviar.AntDesignUI.Pages.CaviarBase
{
    public partial class CodeFileGenerate
    {

        List<ViewModelFields> Models = new List<ViewModelFields>();
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
            var result = await Http.GetJson<List<ViewModelFields>>("Permission/GetModels");
            if (result.Status != HttpState.OK) return;
            Models = result.Data;
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


        CodeGenerateData GenerateData = new CodeGenerateData() { 
            Page = _pageOptions,
            WebApi = _webApi ,Config = new string[] { "创建按钮" } 
        };
        Form<CodeGenerateData> GenerateFrom;
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
                var result = await Http.PostJson<CodeGenerateData,List<TabItem>>($"{Url}?isPerview=true", GenerateData);
                if (result.Status == HttpState.OK)
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
            var result = await Http.PostJson<CodeGenerateData, List<TabItem>>($"{Url}?isPerview=false", GenerateData);
            if (result.Status == HttpState.OK)
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


        List<TabItem> lstTabs { get; set; } = new List<TabItem>();
        string nKey { get; set; } = "1";
    }
}
