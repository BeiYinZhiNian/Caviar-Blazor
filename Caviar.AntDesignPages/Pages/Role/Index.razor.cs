using Caviar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Pages.Role
{
    public partial class Index
    {
        protected override Task<List<ViewRole>> GetPages(int pageIndex = 1, int pageSize = 10, bool isOrder = true)
        {
            pageSize = 100;
            return base.GetPages(pageIndex, pageSize, isOrder);
        }
    }
}
