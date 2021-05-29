using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Caviar.Control
{
    public partial class MenuAction : SysMenu
    {
        public MenuAction()
        {
            TransformationEvent += MenuAction_TransformationEvent;
        }

        private PageData<ViewMenu> MenuAction_TransformationEvent(PageData<SysMenu> model)
        {
            var pages = CommonHelper.AToB<PageData<ViewMenu>, PageData<SysMenu>>(model);
            if (pages.Total != 0)
            {
                var viewMenus = new List<ViewMenu>().ListAutoAssign(model.Rows);
                pages.Rows = viewMenus.ListToTree();
            }
            return pages;
        }

        public virtual IQueryable<SysMenu> GetEntitys(Expression<Func<SysMenu, bool>> where)
        {
            var menus = BC.DC.GetEntityAsync(where).OrderBy(u => u.Id);
            return menus;
        }

        public virtual List<ViewMenu> GetViewMenus(Expression<Func<SysMenu, bool>> where)
        {
            var menus = GetEntitys(where).ToList();
            var viewMenus = new List<ViewMenu>().ListAutoAssign(menus);
            return viewMenus.ListToTree();
        }
    }
}