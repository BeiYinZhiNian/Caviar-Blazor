using AntDesign;
using Caviar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Pages.User
{
    public partial class Index
    {
        protected override async Task RowCallback(RowCallbackData<ViewUser> row)
        {
            switch (row.Menu.MenuName)
            {
                case "删除":
                    row.Data.Password = CommonlyHelper.SHA256EncryptString("123456");//密码不能为空，所以构建一个初始密码
                    await Delete(row.Menu.Url, row.Data);
                    Refresh();
                    break;
                default:
                    await base.RowCallback(row);
                    break;
            }
        }
        List<ViewUserGroup> ViewUserGroups { get; set; }
        Dictionary<string, string> MappingQuery { get; set; }
        protected override async Task OnInitializedAsync()
        {
            MappingQuery = new Dictionary<string, string>();
            MappingQuery.Add("UserGroupName", "UserGroupId");//将映射字段加入到字典
            await base.OnInitializedAsync();
            var result = await Http.GetJson<PageData<ViewUserGroup>>("UserGroup/Index?pageSize=100");
            if (result.Status != HttpState.OK) return;
            ViewUserGroups = result.Data.Rows;
        }
        string UserGroupName = "请选择部门";
        void RemoveRecord(string key)
        {
            UserGroupName = "请选择部门";
            Query.QueryData[key] = "";
        }

        void EventRecord(TreeEventArgs<ViewUserGroup> args,string key)
        {
            UserGroupName = args.Node.Title;
            Query.QueryData[key] = args.Node.Key;
        }
    }
}
