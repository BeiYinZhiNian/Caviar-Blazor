using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Caviar.Control
{
    public partial class MenuAction : SysPowerMenu
    {
        public MenuAction()
        {
            TransformationEvent += MenuAction_TransformationEvent;
        }

        private PageData<ViewMenu> MenuAction_TransformationEvent(PageData<SysPowerMenu> model)
        {
            var pages = CommonHelper.AToB<PageData<ViewMenu>, PageData<SysPowerMenu>>(model);
            if (pages.Total != 0)
            {
                pages.Rows = ModelToView(model.Rows);
            }
            return pages;
        }

        public virtual List<SysPowerMenu> GetEntitys(Expression<Func<SysPowerMenu, bool>> where)
        {
            var menus = BC.DC.GetEntityAsync(where).OrderBy(u => u.Id).ToList();
            return menus;
        }

        public virtual List<ViewMenu> GetViewMenus(Expression<Func<SysPowerMenu, bool>> where)
        {
            var menus = GetEntitys(where);
            return ModelToView(menus);
        }
        protected virtual List<ViewMenu> ModelToView(List<SysPowerMenu> menus)
        {
            //将获取到的sys转为view
            var viewMenus = new List<ViewMenu>().ListAutoAssign(menus);
            var resultViewMenuList = new List<ViewMenu>();
            foreach (var item in viewMenus)
            {
                if (item.UpLayerId == 0)
                {
                    resultViewMenuList.Add(item);
                }
                else
                {
                    viewMenus.SingleOrDefault(u => u.Id == item.UpLayerId)?.Children.Add(item);
                }
            }
            return resultViewMenuList;
        }


        public void ViewToModel(ViewMenu data, List<ViewMenu> menus)
        {
            foreach (var item in data.Children)
            {
                menus.Add(item);
                if (item.Children != null && item.Children.Count != 0)
                {
                    ViewToModel(item, menus);
                }
            }
        }
    }
}