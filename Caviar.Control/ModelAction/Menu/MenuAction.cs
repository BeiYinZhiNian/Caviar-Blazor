using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Caviar.Control.Menu
{
    public partial class MenuAction
    {
        public override async Task<PageData<ViewMenu>> GetPages(Expression<Func<SysMenu, bool>> where, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = false)
        {
            var pages = new PageData<SysMenu>(BC.Menus);
            var list = ModelToViewModel(pages.Rows);
            var viewPage = new PageData<ViewMenu>(list);
            return viewPage;
        }
        public override List<ViewMenu> ModelToViewModel(List<SysMenu> model)
        {
            model.AToB(out List<ViewMenu> outModel);
            var viewMenus = new List<ViewMenu>().ListAutoAssign(outModel).ToList();
            viewMenus = viewMenus.ListToTree();
            return viewMenus;
        }

        public async Task<List<SysMenu>> GetPermissionMenu(List<SysPermission> permissions)
        {
            HashSet<SysMenu> menus = new HashSet<SysMenu>();
            if (BC.IsAdmin)
            {
                return await BC.DC.GetAllAsync<SysMenu>();
            }
            else
            {
                permissions = permissions.Where(u => u.PermissionType == PermissionType.Menu).ToList();
                foreach (var item in permissions)
                {
                    var menu = await BC.DC.GetFirstEntityAsync<SysMenu>(u => u.Id == item.PermissionId);
                    if (menu == null) continue;
                    menus.Add(menu);
                }
            }
            return menus.OrderBy(u => u.Number).ToList();
        }


    }
}