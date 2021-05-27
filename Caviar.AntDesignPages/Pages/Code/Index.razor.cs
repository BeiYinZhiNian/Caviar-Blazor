using AntDesign;
using Caviar.Models.SystemData;
using Caviar.AntDesignPages.Helper;
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

namespace Caviar.AntDesignPages.Pages.Code
{
    public partial class Index
    {

        List<ModelData> Models = new List<ModelData>();

        [Inject]
        HttpHelper Http { get; set; }
        [Inject]
        MessageService _message { get; set; }


        protected override void OnInitialized()
        {
            GetModels();
        }


        public void GetModels()
        {
            CommonHelper.GetAssembly()
                    //遍历查找
                    .ForEach((t =>
                    {
                        //获取所有对象
                        t.GetTypes()
                            //查找是否包含IBaseModel接口的类
                            .Where(u => u.GetInterfaces().Contains(typeof(IBaseModel)))
                            //判断是否是类
                            .Where(u => u.IsClass)
                            //转换成list
                            .ToList()
                            //循环,并添注入
                            .ForEach(t =>
                            {
                                var displayName = t.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                                Models.Add(new ModelData {Name = t.Name, DisplayName = displayName,FullName = t.FullName.Replace("."+t.Name,"") });
                            });
                    }));
            
        }
        static string[] _pageOptions = { "列表","新增","修改","数据模板" };
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


        CodeGenerateData GenerateData = new CodeGenerateData() { Page = _pageOptions,WebApi = _webApi ,Config = new string[] { "创建按钮" } };
        void OnPreClick()
        {
            current--;
        }

        async void OnNextClick()
        {
            if (current == 1)
            {
                if((GenerateData.Page == null || GenerateData.Page.Length == 0) && (GenerateData.WebApi == null || GenerateData.WebApi.Length == 0))
                {
                    await _message.Error("请在前后端至少选择一个进行生成");
                    return;
                }
                if (GenerateData.OutName == "")
                {
                    await _message.Error("生成目录不可为空");
                    return;
                }
                var result = await Http.PostJson<CodeGenerateData,List<TabItem>>("CaviarBase/CodePreview", GenerateData);
                if (result.Status == 200)
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
            var result = await Http.PostJson<CodeGenerateData, List<TabItem>>("CaviarBase/CodeFileGenerate", GenerateData);
            if (result.Status == 200)
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

        class ModelData
        {
            [DisplayName("表名称")]
            public string Name { get; set; }
            [DisplayName("备注")]
            public string DisplayName { get; set; }
            [DisplayName("命名空间")]
            public string FullName { get; set; }
        }
    }
}
