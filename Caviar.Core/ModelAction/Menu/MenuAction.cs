using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Caviar.Core.Menu
{
    public partial class MenuAction
    {
        public ResultMsg<List<SysMenu>> GetButton(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return Error<List<SysMenu>>("请输入正确地址");
            }
            var menus = BC.UserData.Menus.Where(u => u.Url?.ToLower() == url.ToLower()).FirstOrDefault();
            if (menus == null)
            {
                return Ok(new List<SysMenu>());
            }
            var buttons = BC.UserData.Menus.Where(u => (u.MenuType == MenuType.Button || u.MenuType == MenuType.API) && u.ParentId == menus.Id).OrderBy(u => u.Number);
            return Ok(buttons.ToList());
        }
        public ResultMsg<List<ViewMenu>> GetMenus()
        {
            var menus = BC.UserData.Menus.Where(u => u.MenuType == MenuType.Menu || u.MenuType == MenuType.Catalog).ToList();
            var viewModel = ToViewModel(menus);
            return Ok(viewModel);
        }
        public override async Task<ResultMsg<PageData<ViewMenu>>> GetPages(Expression<Func<SysMenu, bool>> where, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = false)
        {
            var pages = new PageData<SysMenu>(BC.UserData.Menus);
            var list = ToViewModel(pages.Rows);
            var viewPage = new PageData<ViewMenu>(list);
            return Ok(viewPage);
        }
        /// <summary>
        /// 将列表转为树的形式
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override List<ViewMenu> ToViewModel(List<SysMenu> model)
        {
            model.AToB(out List<ViewMenu> outModel);
            var viewMenus = outModel.ListToTree();
            return viewMenus;
        }

        public async Task<ResultMsg> DeleteEntityAll(ViewMenu view)
        {
            List<ViewMenu> viewMenuList = new List<ViewMenu>();
            view.TreeToList(viewMenuList);
            viewMenuList.AToB(out List<SysMenu> menus);
            var result = await base.DeleteEntity(menus);
            return result;
        }

        public async Task<ResultMsg> DeleteEntity(ViewMenu view)
        {
            List<ViewMenu> viewMenuList = new List<ViewMenu>();
            view.TreeToList(viewMenuList,false);
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
                var result = await base.UpdateEntity(menus);
                if (result.Status != HttpState.OK)
                {
                    return Error("删除菜单失败,子菜单移动失败");
                }
            }
            return await base.DeleteEntity(view);
        }

    }
}