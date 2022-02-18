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
            throw new NotImplementedException();
        }
    }
}
