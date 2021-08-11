using Caviar.AntDesignUI.Helper;
using Caviar.SharedKernel.View;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AntDesign;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Http;

namespace Caviar.AntDesignUI.Pages.Permission
{
    public partial class PermissionFields
    {
        List<ViewFields> Models = new List<ViewFields>();
        List<ViewFields> Fields { get; set; }
        ViewFields CurrentModel { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        HttpHelper Http { get; set; }
        [Inject]
        MessageService MessageService { get; set; }
        string FieldName { get; set; } = "请选择模型";

        ViewRole Role { get; set; }
        string Url { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var query = new Uri(NavigationManager.Uri).Query;
            Url = NavigationManager.Uri.Replace(NavigationManager.BaseUri, "").Replace(query,"");
            if (QueryHelpers.ParseQuery(query).TryGetValue("Parameter", out var Parameter))
            {
                Role = JsonSerializer.Deserialize<ViewRole>(Parameter);
            }
            await GetModels();
        }


        public async Task GetModels()
        {
            var result = await Http.GetJson<List<ViewFields>>("Permission/GetModels?isView=true");
            if (result.Status != StatusCodes.Status200OK) return;
            Models = result.Data;
        }

        public async Task GetFields(ViewFields model)
        {
            var result = await Http.GetJson<List<ViewFields>>($"{Url}?modelName={model.Entity.FieldName}&roleId={Role.Id}");
            if (result.Status != StatusCodes.Status200OK) return;
            CurrentModel = model;
            FieldName = model.Entity.DisplayName + "-数据字段";
            Fields = result.Data;
        }

        string editId;
        void startEdit(string id)
        {
            editId = id;
        }

        void stopEdit(ViewFields model)
        {
            if(string.IsNullOrEmpty(model.Entity.TableWidth) || int.TryParse(model.Entity.TableHeight, out int result))
            {
                editId = null;
            }
            else
            {
                MessageService.Error("列宽只能为整数,请重新编写");
            }
        }

        async void Preservation()
        {
            if (editId != null) return;
            if (Fields == null || CurrentModel==null)
            {
                await MessageService.Error("请先选择所要保存的模型");
                return;
            }
            var result = await Http.PostJson($"{Url}?fullName={CurrentModel.Entity.FieldName}&roleId={Role.Id}", Fields);
            if (result.Status != StatusCodes.Status200OK) return;
            await MessageService.Success("保存完毕");
        }

    }
}
