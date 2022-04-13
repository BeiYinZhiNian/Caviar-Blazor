using AntDesign;
using Caviar.AntDesignUI.Core;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Pages.User
{
    public partial class UserDataTemplate
    {
        [Inject]
        HttpService HttpService { get; set; }


        protected override async Task OnInitializedAsync()
        {
            ParentMenuName = UserConfig.LanguageService[$"{ CurrencyConstant.Page }.{ CurrencyConstant.AllocationDepartment}"];
            await base.OnInitializedAsync();
            UserGroupViews = await GetUserGroups();
        }

        private List<SysUserGroupView> UserGroupViews = new List<SysUserGroupView>();


        async Task<List<SysUserGroupView>> GetUserGroups()
        {

            var result = await HttpService.GetJson<PageData<SysUserGroupView>>($"{UrlConfig.UserGroupIndex}?pageSize={Config.MaxPageSize}");
            if (result.Status != HttpStatusCode.OK) return null;
            if (DataSource.Entity.UserGroupId > 0)
            {
                List<SysUserGroupView> listData = new List<SysUserGroupView>();
                result.Data.Rows.TreeToList(listData);
                var parent = listData.SingleOrDefault(u => u.Id == DataSource.Entity.UserGroupId);
                if (parent != null)
                {
                    ParentMenuName = parent.Entity.Name;
                }
            }
            return result.Data.Rows;
        }

        string ParentMenuName { get; set; }
        void EventRecord(TreeEventArgs<SysUserGroupView> args)
        {
            ParentMenuName = args.Node.Title;
            DataSource.Entity.UserGroupId = int.Parse(args.Node.Key);
        }
    }
}
