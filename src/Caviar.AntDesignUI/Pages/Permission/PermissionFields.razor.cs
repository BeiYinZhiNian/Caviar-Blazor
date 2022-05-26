// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Collections.Generic;
using System.Threading.Tasks;
using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;

namespace Caviar.AntDesignUI.Pages.Permission
{
    public partial class PermissionFields
    {
        [Parameter]
        public int RoleId { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        UserConfig UserConfig { get; set; }

        [Inject]
        MessageService MessageService { get; set; }
        [Inject]
        HttpService HttpService { get; set; }
        [Inject]
        ILanguageService LanguageService { get; set; }



        List<FieldsView> Models = new List<FieldsView>();
        List<FieldsView> Fields { get; set; }
        FieldsView CurrentModel { get; set; }
        string FieldName { get; set; } = "";
        int PageIndex = 1;

        ApplicationRoleView Role { get; set; }


        protected override async Task OnInitializedAsync()
        {
            var result = await HttpService.GetJson<ApplicationRoleView>(UrlConfig.RoleFindById + $"?id={RoleId}");
            if (result.Status == System.Net.HttpStatusCode.OK)
            {
                Role = result.Data;
            }
            await base.OnInitializedAsync();
            await GetModels();
        }


        public async Task GetModels()
        {
            var result = await HttpService.GetJson<List<FieldsView>>(UrlConfig.GetEntitys);
            if (result.Status != System.Net.HttpStatusCode.OK) return;
            Models = result.Data;
        }

        public async Task GetFields(FieldsView model)
        {
            var result = await HttpService.GetJson<List<FieldsView>>($"{UrlConfig.GetFields}?name={model.Entity.FieldName}&fullName={model.Entity.FullName}&roleName={Role.Entity.Name}");
            if (result.Status != System.Net.HttpStatusCode.OK) return;
            PageIndex = 1;
            CurrentModel = model;
            FieldName = model.DisplayName + "-" + LanguageService[$"{CurrencyConstant.EntitysName}.DataField"];
            Fields = result.Data;
        }

        string editId;
        void startEdit(string id)
        {
            editId = id;
        }

        void stopEdit(FieldsView model)
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
            var result = await HttpService.PostJson($"{UrlConfig.SaveRoleFields}?roleName={Role.Entity.Name}", Fields);
            if (result.Status != System.Net.HttpStatusCode.OK) return;
            _ = MessageService.Success(result.Title);
        }

    }
}
