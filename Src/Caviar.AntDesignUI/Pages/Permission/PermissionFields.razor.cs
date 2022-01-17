using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Pages.Permission
{
    public partial class PermissionFields
    {
        List<ViewFields> Models = new List<ViewFields>();
        List<ViewFields> Fields { get; set; }
        ViewFields CurrentModel { get; set; }
        string FieldName { get; set; } = "请选择模型";

        ApplicationRoleView Role { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ControllerList.Add("Permission");
            await base.OnInitializedAsync();
            await GetModels();
        }


        public async Task GetModels()
        {
            var result = await HttpService.GetJson<List<ViewFields>>(Url["GetEntitys"]);
            if (result.Status != System.Net.HttpStatusCode.OK) return;
            Models = result.Data;
        }

        public async Task GetFields(ViewFields model)
        {
            var result = await HttpService.GetJson<List<ViewFields>>($"{Url}?modelName={model.Entity.FieldName}&roleId={Role.Entity.Id}");
            if (result.Status != System.Net.HttpStatusCode.OK) return;
            CurrentModel = model;
            FieldName = model.DisplayName + "-数据字段";
            Fields = result.Data;
        }

        string editId;
        void startEdit(string id)
        {
            editId = id;
        }

        void stopEdit(ViewFields model)
        {
            if (string.IsNullOrEmpty(model.Entity.TableWidth) || int.TryParse(model.Entity.TableWidth, out int result))
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
            if (Fields == null || CurrentModel == null)
            {
                await MessageService.Error("请先选择所要保存的模型");
                return;
            }
            var result = await HttpService.PostJson($"{Url}?fullName={CurrentModel.Entity.FieldName}&roleId={Role.Entity.Id}", Fields);
            if (result.Status != System.Net.HttpStatusCode.OK) return;
            await MessageService.Success("保存完毕");
        }

    }
}
