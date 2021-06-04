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
            var pages = new PageData<SysMenu>(BC.Menus);
            var list = ModelToViewModel(pages.Rows);
            var viewPage = new PageData<ViewMenu>(list);
            return viewPage;
        }
    }
}
