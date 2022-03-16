using Caviar.SharedKernel.Entities.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Pages.UserGroup
{
    public partial class SysUserGroupIndex
    {
        protected override void OnInitialized()
        {
            TableOptions.TreeChildren = u => u.Children;
            base.OnInitialized();
        }

        protected override Task<List<SysUserGroupView>> GetPages(int pageIndex = 1, int pageSize = 10, bool isOrder = true)
        {
            // 当使用树形组件时，需要获取全部数据
            // 也可改成GetAll
            pageSize = MaxPageSize;
            return base.GetPages(pageIndex, pageSize, isOrder);
        }
    }
}
