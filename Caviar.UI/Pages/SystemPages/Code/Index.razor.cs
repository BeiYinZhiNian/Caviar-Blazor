using AntDesign;
using Caviar.Models.SystemData;
using Caviar.UI.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Caviar.UI.Pages.SystemPages.Code
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
                                Models.Add(new ModelData {Name = t.Name, DisplayName = displayName });
                            });
                    }));
            
        }
        static string[] pageOptions = { "列表","新增" };
        static string[] buttonOptions = { "新增", "删除", "查询" };
        static string[] configOptions = { "覆盖" };
        void OnPageChange(string[] checkedValues)
        {
            Generate.Page = checkedValues;
        }

        void OnButtonChange(string[] checkedValues)
        {
            Generate.Button = checkedValues;
        }

        void OnConfigChange(string[] checkedValues)
        {
            Generate.Config = checkedValues;
        }



        CodeGenerateData Generate = new CodeGenerateData() { Page = pageOptions, Button = buttonOptions };
        void OnPreClick()
        {
            current--;
        }

        async void OnNextClick()
        {
            if (current == 1)
            {
                if(Generate.Page==null || Generate.Page.Length == 0)
                {
                    await _message.Error("请至少选择一个页面进行生成");
                    return;
                }
                if (Generate.OutName == "")
                {
                    await _message.Error("生成目录不可为空");
                    return;
                }
                var result = await Http.PostJson<CodeGenerateData,List<TabItem>>("Base/CodeGenerate",Generate);
                if (result.Status == 200)
                {
                    lstTabs = result.Data;
                }
            }
            current++;
            StateHasChanged();
        }
        List<TabItem> lstTabs { get; set; } = new List<TabItem>();
        string nKey { get; set; } = "1";

        class ModelData
        {
            [DisplayName("表名称")]
            public string Name { get; set; }
            [DisplayName("备注")]
            public string DisplayName { get; set; }
        }
    }
}
