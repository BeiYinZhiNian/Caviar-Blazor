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
        public List<SysMenu> GetButton(string url)
        {
            var menus = BC.UserData.Menus.Where(u => u.Url?.ToLower() == url.ToLower()).FirstOrDefault();
            if (menus == null)
            {
                return null;
            }
            var buttons = BC.UserData.Menus.Where(u => u.MenuType == MenuType.Button && u.ParentId == menus.Id).OrderBy(u => u.Number);
            return buttons.ToList();
        }
        public override async Task<PageData<ViewMenu>> GetPages(Expression<Func<SysMenu, bool>> where, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = false)
        {
            var pages = new PageData<SysMenu>(BC.UserData.Menus);
            var list = ModelToViewModel(pages.Rows);
            var viewPage = new PageData<ViewMenu>(list);
            return viewPage;
        }
        /// <summary>
        /// 将列表转为树的形式
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override List<ViewMenu> ModelToViewModel(List<SysMenu> model)
        {
            model.AToB(out List<ViewMenu> outModel);
            var viewMenus = outModel.ListToTree();
            return viewMenus;
        }

        public async Task<int> DeleteEntityAll(ViewMenu view)
        {
            List<ViewMenu> viewMenuList = new List<ViewMenu>();
            view.TreeToList(viewMenuList);
            viewMenuList.AToB(out List<SysMenu> menus);
            var count = await base.DeleteEntity(menus);
            return count;
        }

        public async Task<int> DeleteEntity(ViewMenu view)
        {
            List<ViewMenu> viewMenuList = new List<ViewMenu>();
            view.TreeToList(viewMenuList,false);
            var count = 0;
            if (viewMenuList != null && viewMenuList.Count > 1)
            {
                viewMenuList.AToB(out List<SysMenu> menus);
                foreach (var item in menus)
                {
                    if (item.ParentId == view.Id)
                    {
                        item.ParentId = view.ParentId;
                    }
                }
                count = await base.UpdateEntity(menus);
                if (count != viewMenuList.Count)
                {
                    throw new Exception("删除菜单失败,子菜单移动失败");
                }
            }
            count = await base.DeleteEntity();
            return count;
        }

    }
}