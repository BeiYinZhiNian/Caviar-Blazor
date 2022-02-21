using AntDesign;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
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

        private FormValidationRule[] UserGroupRule = new FormValidationRule[]
        {
            new FormValidationRule() { Type=FormFieldType.Number,Min=1,Message="请为用户分配部门" },
        };

        private FormValidationRule[] EmailRule = new FormValidationRule[]
        {
            new FormValidationRule(){ Type=FormFieldType.Email,Required=true,Message="请输入正确的邮箱"},
        };

        protected override async Task OnInitializedAsync()
        {
            ControllerList.Add(CurrencyConstant.SysUserGroupKey);
            await base.OnInitializedAsync();
            UserGroupViews = await GetUserGroups();
        }

        private List<SysUserGroupView> UserGroupViews = new List<SysUserGroupView>();


        async Task<List<SysUserGroupView>> GetUserGroups()
        {

            var result = await HttpService.GetJson<PageData<SysUserGroupView>>($"{Url[CurrencyConstant.SysUserGroupKey]}?pageSize=100");
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

        string ParentMenuName { get; set; } = "请分配部门";
        void EventRecord(TreeEventArgs<SysUserGroupView> args)
        {
            ParentMenuName = args.Node.Title;
            DataSource.Entity.UserGroupId = int.Parse(args.Node.Key);
        }
    }
}
