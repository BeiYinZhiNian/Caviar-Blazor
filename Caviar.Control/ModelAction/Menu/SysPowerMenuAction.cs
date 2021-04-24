using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Caviar.Control
{
    public partial class SysPowerMenuAction : SysPowerMenu
    {

        public virtual List<ViewPowerMenu> GetMenus(Expression<Func<SysPowerMenu, bool>> where)
        {
            var menus = BaseControllerModel.DataContext.GetEntityAsync(where).OrderBy(u => u.Id).ToList();
            return ModelToView(menus);
        }


        protected virtual List<ViewPowerMenu> ModelToView(List<SysPowerMenu> sysPowerMenus)
        {
            //将获取到的sys转为view
            var viewMenus = new List<ViewPowerMenu>().ListAutoAssign(sysPowerMenus);
            var resultViewMenuList = new List<ViewPowerMenu>();
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
    }
}