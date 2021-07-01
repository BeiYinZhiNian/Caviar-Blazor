using AntDesign;
using Caviar.Models.SystemData;
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
                    row.Data.Password = CommonHelper.SHA256EncryptString("123456");//密码不能为空，所以构建一个初始密码
                    await Delete(row.Menu.Url, row.Data);
                    break;
            }
        }
        List<ViewUserGroup> ViewUserGroups { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var result = await Http.GetJson<PageData<ViewUserGroup>>("UserGroup/Index?pageSize=100");
            if (result.Status != 200) return;
            ViewUserGroups = result.Data.Rows;
        }
        string ParentName = "请选择部门";
        void RemoveRecord(string key)
        {
            ParentName = "请选择部门";
            Query.QueryData[key] = "";
        }

        void EventRecord(TreeEventArgs<ViewUserGroup> args,string key)
        {
            ParentName = args.Node.Title;
            Query.QueryData[key] = args.Node.Key;
        }
    }
}
