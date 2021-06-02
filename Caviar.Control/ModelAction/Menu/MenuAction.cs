using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Caviar.Control.Menu
{
    public partial class MenuAction : SysMenu
    {
        partial void PartialModelToViewModel(ref bool isContinue, PageData<SysMenu> model, ref PageData<ViewMenu> outModel)
        {
            outModel = CommonHelper.AToB<PageData<ViewMenu>, PageData<SysMenu>>(model);
            if (outModel.Total != 0)
            {
                var viewMenus = new List<ViewMenu>().ListAutoAssign(model.Rows);
                outModel.Rows = viewMenus.ListToTree();
            }
            isContinue = false;
        }

        public List<SysMenu> GetEntitys(Expression<Func<SysMenu, bool>> where)
        {
            var menus = BC.DC.GetEntityAsync(where).OrderBy(u => u.Id).ToList();
            return menus;
        }

        public async virtual Task<List<ViewMenu>> GetViewMenus(Expression<Func<SysMenu, bool>> where)
        {
            var menus = GetEntitys(where).ToList();
            var viewMenus = new List<ViewMenu>().ListAutoAssign(menus);
            return viewMenus.ListToTree();
        }
    }
}