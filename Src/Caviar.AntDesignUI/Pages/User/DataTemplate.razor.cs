using AntDesign;
using Caviar.SharedKernel;
using Caviar.SharedKernel.View;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Pages.User
{
    public partial class DataTemplate
    {
        protected override async Task OnInitializedAsync()
        {
            var password = "123456";//创建的初始密码为123456，修改时候也提交这个密码，字段权限会自动过滤掉
            DataSource.Entity.PasswordHash = CommonHelper.SHA256EncryptString(password);//设置默认密码
            await GetViewUserGroups();
            await base.OnInitializedAsync();
        }
        string ParentName { get; set; } = "未分配部门";

        async Task GetViewUserGroups()
        {
            var result = await Http.GetJson<PageData<ViewUserGroup>>("UserGroup/Index?pageSize=100");
            if (result.Status != StatusCodes.Status200OK) return;
            if (DataSource.Entity.UserGroupId > 0)
            {
                List<ViewUserGroup> listData = new List<ViewUserGroup>();
                result.Data.Rows.TreeToList(listData);
                var parent = listData.SingleOrDefault(u => u.Id == DataSource.Entity.UserGroupId);
                if (parent != null)
                {
                    ParentName = parent.Entity.Name;
                }
            }
            ViewUserGroups = result.Data.Rows;
            StateHasChanged();
        }



        void EventRecord(TreeEventArgs<ViewUserGroup> args)
        {
            ParentName = args.Node.Title;
            DataSource.Entity.UserGroupId = int.Parse(args.Node.Key);
        }

        void RemoveRecord()
        {
            ParentName = "未分配部门";
            DataSource.Entity.UserGroupId = 0;
        }
    }
}
