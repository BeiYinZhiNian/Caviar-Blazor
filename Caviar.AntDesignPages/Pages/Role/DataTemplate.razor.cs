using AntDesign;
using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Pages.Role
{
    public partial class DataTemplate
    {

        string ParentName { get; set; } = "无上层角色";
        void EventRecord(TreeEventArgs<ViewRole> args)
        {
            ParentName = args.Node.Title;
            DataSource.ParentId = int.Parse(args.Node.Key);
        }

        void RemoveRecord()
        {
            ParentName = "无上层角色";
            DataSource.ParentId = 0;
        }

        public List<ViewRole> ViewRoles { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ViewRoles = await GetPowerMenus();
        }

        async Task<List<ViewRole>> GetPowerMenus()
        {
            var result = await Http.GetJson<PageData<ViewRole>>($"Role/GetPages?pageSize=100");
            if (result.Status != 200) return new List<ViewRole>();
            if (DataSource.ParentId > 0)
            {
                List<ViewRole> listData = new List<ViewRole>();
                result.Data.Rows.TreeToList(listData);
                var parent = listData.SingleOrDefault(u => u.Id == DataSource.ParentId);
                if (parent != null)
                {
                    ParentName = parent.RoleName;
                }
            }
            return result.Data.Rows;
        }
    }
}
