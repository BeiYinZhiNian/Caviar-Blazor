using Caviar.Models.SystemData;
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

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        List<ModelData> Models = new List<ModelData>();

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
        string[] plainOptions = { "列表","新增", "删除", "查询" };

        void OnChange(string[] checkedValues)
        {
            Console.WriteLine($"checked = {JsonSerializer.Serialize(checkedValues)}");
        }
        class ModelData
        {
            [DisplayName("表名称")]
            public string Name { get; set; }
            [DisplayName("备注")]
            public string DisplayName { get; set; }
        }
    }
}
