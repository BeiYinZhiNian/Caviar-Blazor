using AntDesign;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Pages.UserGroup
{
    public partial class SysUserGroupDataTemplate
    {

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
            if (DataSource.ParentId > 0)
            {
                List<SysUserGroupView> listData = new List<SysUserGroupView>();
                result.Data.Rows.TreeToList(listData);
                var parent = listData.SingleOrDefault(u => u.Id == DataSource.ParentId);
                if (parent != null)
                {
                    ParentMenuName = parent.Entity.Name;
                }
            }
            return result.Data.Rows;
        }

        string ParentMenuName { get; set; } = "无上层目录";
        void EventRecord(TreeEventArgs<SysUserGroupView> args)
        {
            ParentMenuName = args.Node.Title;
            DataSource.Entity.ParentId = int.Parse(args.Node.Key);
        }

        void RemoveRecord()
        {
            ParentMenuName = "无上层目录";
            DataSource.Entity.ParentId = 0;
        }
    }
}
