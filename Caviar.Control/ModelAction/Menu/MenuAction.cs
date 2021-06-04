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

        public async Task<List<SysMenu>> GetPermissionMenu(List<SysPermission> permissions,bool isAdmin =false)
        {
            HashSet<SysMenu> menus = new HashSet<SysMenu>();
            if (isAdmin)
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