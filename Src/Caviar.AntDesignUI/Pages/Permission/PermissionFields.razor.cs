using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
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
        string FieldName { get; set; } = "";

        ApplicationRoleView Role { get; set; }


        protected override async Task OnInitializedAsync()
        {
            var query = new Uri(NavigationManager.Uri).Query;
            if (QueryHelpers.ParseQuery(query).TryGetValue("Parameter", out var Parameter))
            {
                Role = JsonConvert.DeserializeObject<ApplicationRoleView>(Parameter);
            }
            ControllerList.Add(CurrencyConstant.PermissionKey);
            await base.OnInitializedAsync();
            await GetModels();
        }


        public async Task GetModels()
        {
            var result = await HttpService.GetJson<List<ViewFields>>(Url[CurrencyConstant.GetEntitysKey]);
            if (result.Status != System.Net.HttpStatusCode.OK) return;
            Models = result.Data;
        }

        public async Task GetFields(ViewFields model)
        {
            var result = await HttpService.GetJson<List<ViewFields>>($"{Url[CurrencyConstant.GetFieldsKey, CurrencyConstant.PermissionKey]}?name={model.Entity.FieldName}&fullName={model.Entity.FullName}&roleName={Role.Entity.Name}");
            if (result.Status != System.Net.HttpStatusCode.OK) return;
            CurrentModel = model;
            FieldName = model.DisplayName + "-" + LanguageService[$"{CurrencyConstant.EntitysName}.DataField"];
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
                _ = MessageService.Error("请先选择所要保存的模型");
                return;
            }
            var result = await HttpService.PostJson($"{Url[CurrencyConstant.SaveRoleFields, CurrencyConstant.PermissionKey]}?roleName={Role.Entity.Name}", Fields);
            if (result.Status != System.Net.HttpStatusCode.OK) return;
            _ = MessageService.Success("保存完毕");
        }

    }
}
