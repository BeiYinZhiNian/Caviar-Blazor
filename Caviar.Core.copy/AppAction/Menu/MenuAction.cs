using Caviar.SharedKernel;
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
        public ResultMsg<List<ViewMenu>> GetButton(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return Error<List<ViewMenu>>("请输入正确地址");
            }
            var menus = Interactor.UserData.Menus.Where(u => u.Url?.ToLower() == url.ToLower()).FirstOrDefault();
            if (menus == null)
            {
                return Ok(new List<ViewMenu>());
            }
            var buttons = Interactor.UserData.Menus.Where(u => (u.MenuType == MenuType.Button || u.MenuType == MenuType.API) && u.ParentId == menus.Id).OrderBy(u => u.Number);
            return Ok(buttons.ToList());
        }
        public ResultMsg<List<ViewMenu>> GetMenus()
        {
            var menus = Interactor.UserData.Menus.Where(u => u.MenuType == MenuType.Menu || u.MenuType == MenuType.Catalog).ToList();
            return Ok(menus);
        }
        public override async Task<ResultMsg<PageData<ViewMenu>>> GetPages(Expression<Func<ViewMenu, bool>> where, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = false)
        {
            var pages = new PageData<ViewMenu>(Interactor.UserData.Menus);
            return Ok(pages);
        }
        /// <summary>
        /// 将列表转为树的形式
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override List<ViewMenu> ToViewModel(List<ViewMenu> model)
        {
            model.AToB(out List<ViewMenu> outModel);
            var viewMenus = outModel.ListToTree();
            return viewMenus;
        }

        public async Task<ResultMsg> DeleteEntityAll(ViewMenu view)
        {
            List<ViewMenu> viewMenuList = new List<ViewMenu>();
            view.TreeToList(viewMenuList);
            var result = await base.DeleteEntity(viewMenuList);
            return result;
        }

        public override async Task<ResultMsg> DeleteEntity(ViewMenu view)
        {
            List<ViewMenu> viewMenuList = new List<ViewMenu>();
            view.TreeToList(viewMenuList,false);
            if (viewMenuList != null && viewMenuList.Count > 1)
            {
                foreach (var item in viewMenuList)
                {
                    if (item.ParentId == view.Id)
                    {
                        item.ParentId = view.ParentId;
                    }
                }
                var result = await base.UpdateEntity(viewMenuList);
                if (result.Status != HttpState.OK)
                {
                    return Error("删除菜单失败,子菜单移动失败");
                }
            }
            return await base.DeleteEntity(view);
        }

    }
}