using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control.Menu
{
    public class NewMenuAction:MenuAction
    {
        public override async Task<PageData<ViewMenu>> GetPages(Expression<Func<SysMenu, bool>> where, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = false)
        {
            var pages = new PageData<SysMenu>()
            {
                Rows = BC.Menus,
                Total = BC.Menus.Count,
            };
            var ViewPages = ModelToViewModel(pages);
            return ViewPages;
        }
    }
}
