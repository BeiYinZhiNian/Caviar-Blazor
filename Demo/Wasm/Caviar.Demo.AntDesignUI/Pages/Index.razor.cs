using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Caviar.Models.SystemData;
using Caviar.AntDesignPages.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Caviar.AntDesignPages.Shared;
using AntDesign;

namespace Caviar.Demo.AntDesignUI.Pages
{
    partial class Index
    {

        [CascadingParameter]
        public EventCallback LayoutStyleCallBack { get; set; }

        [Inject]
        HttpHelper Http { get; set; }

        [Inject]
        UserConfigHelper UserConfig { get; set; }

        [Inject]
        IJSRuntime JsRuntime { get; set; }

        CavDataTemplate CavData { get; set; }

        RenderFragment test;
        public async Task Test()
        {
            test = RenderForm();
            StateHasChanged();
        }

        SysPowerMenu menu = new SysPowerMenu();
        public RenderFragment RenderForm() => builder =>
        {
            
            var ComponentType = typeof(Form<SysPowerMenu>);
            var index = 0;
            builder.OpenComponent(index++, ComponentType);
            builder.AddAttribute(index++, "Model", menu);
            builder.AddAttribute(index++, "ChildContent ", RederFormItem());
            builder.AddComponentReferenceCapture(index++, SetFormRef);
            builder.CloseComponent();

        };

        object form;

        void SetFormRef(object obj)
        {
            form = obj;
        }
        public RenderFragment<SysPowerMenu> RederFormItem() => builder =>
        {

            return RederFormData();
        };

        public RenderFragment RederFormData() => builder =>
        {
            var ComponentType = typeof(FormItem);
            var index = 0;
            builder.OpenComponent(index++, ComponentType);
            builder.AddAttribute(index++, "Label", "²âÊÔ");
            builder.CloseComponent();

        };

        public RenderFragment RederInput() => builder =>
        {
            var ComponentType = typeof(Input<string>);
            var index = 0;
            builder.OpenComponent(index++, ComponentType);
            builder.CloseComponent();

        };

        public RenderFragment RederCascadingValue() => builder =>
        {
            var ComponentType = typeof(CascadingValue<Form<SysPowerMenu>>);
            var index = 0;
            builder.OpenComponent(index++, ComponentType);
            builder.AddAttribute(index++, "Value", form);
            builder.AddAttribute(index++, "Name", "Form");
            builder.AddContent(index++, RederFormItem());
            builder.CloseComponent();

        };
    }
}