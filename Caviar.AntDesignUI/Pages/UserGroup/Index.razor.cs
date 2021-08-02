using Caviar.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Pages.UserGroup
{
    public partial class Index
    {
        protected override Task<List<ViewUserGroup>> GetPages(int pageIndex = 1, int pageSize = 10, bool isOrder = true)
        {
            pageSize = 100;
            return base.GetPages(pageIndex, pageSize, isOrder);
        }
    }
}
