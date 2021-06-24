using AntDesign;
using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Pages.User
{
    public partial class DataTemplate
    {
        partial void PratialOnInitializedAsync(ref bool IsContinue)
        {
            var password = "123456";//创建的初始密码为123456，修改时候也提交这个密码，字段权限会自动过滤掉
            DataSource.Password = CommonHelper.SHA256EncryptString(password);//设置默认密码
            GetViewUserGroups();
        }

        private List<ViewUserGroup> ViewUserGroups = new List<ViewUserGroup>();
        string ParentName { get; set; } = "未分配部门";

        async void GetViewUserGroups()
        {
            var result = await Http.GetJson<PageData<ViewUserGroup>>("UserGroup/Index?pageSize=100");
            if (result.Status != 200) return;
            if (DataSource.UserGroupId > 0)
            {
                List<ViewUserGroup> listData = new List<ViewUserGroup>();
                result.Data.Rows.TreeToList(listData);
                var parent = listData.SingleOrDefault(u => u.Id == DataSource.UserGroupId);
                if (parent != null)
                {
                    ParentName = parent.Name;
                }
            }
            ViewUserGroups = result.Data.Rows;
            StateHasChanged();
        }



        void EventRecord(TreeEventArgs<ViewUserGroup> args)
        {
            ParentName = args.Node.Title;
            DataSource.UserGroupId = int.Parse(args.Node.Key);
        }

        void RemoveRecord()
        {
            ParentName = "未分配部门";
            DataSource.UserGroupId = null;
        }
    }
}
