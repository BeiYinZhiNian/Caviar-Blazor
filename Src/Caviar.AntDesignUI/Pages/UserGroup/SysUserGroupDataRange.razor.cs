using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Pages.UserGroup
{
    public partial class SysUserGroupDataRange : ITableTemplate
    {
        [Parameter]
        public ApplicationRoleView DataSource { get; set; }


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            UserGroups = await GetMenus();
            if(DataSource.Entity.DataList != null && DataSource.Entity.DataList.Count() > 0)
            {
                var ids = DataSource.Entity.DataList.Split(CurrencyConstant.CustomDataSeparator);
                foreach (var item in UserGroups)
                {
                    item.IsPermission = ids.Contains(item.Id.ToString());
                }
            }
            
        }

        private List<SysUserGroupView> UserGroups = new List<SysUserGroupView>();


        async Task<List<SysUserGroupView>> GetMenus()
        {
            var result = await HttpService.GetJson<PageData<SysUserGroupView>>($"{Url[CurrencyConstant.SysUserGroupKey]}?pageSize=100");
            if (result.Status != HttpStatusCode.OK) return null;
            return result.Data.Rows;
        }


        public Task<bool> Validate()
        {
            var ids = UserGroups.Where(u => u.IsPermission).Select(u => u.Id).ToList();
            StringBuilder dataList = new StringBuilder();
            foreach (var item in ids)
            {
                dataList.Append($"{item.ToString()}{CurrencyConstant.CustomDataSeparator}");
            }
            DataSource.Entity.DataList = dataList.ToString();
            return Task.FromResult(true);
        }
    }
}
