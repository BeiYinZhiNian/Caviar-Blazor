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

        public List<SysMenu> GetPermissionMenu(List<SysPermission> permissions)
        {
            List<SysMenu> menus = new List<SysMenu>();
            permissions = permissions.Where(u => u.PermissionType == PermissionType.Menu).ToList();
            foreach (var item in permissions)
            {
                var menu = BC.DC.GetEntityAsync<SysMenu>(u => u.Id == item.PermissionId).FirstOrDefault();
                if (menu == null) continue;
                menus.Add(menu);
            }
            return menus;
        }


    }
}