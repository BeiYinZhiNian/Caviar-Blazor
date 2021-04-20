using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Caviar.Control
{
    public partial class SysPowerMenuAction : SysPowerMenu
    {
        public virtual List<ViewPowerMenu> GetLeftSideMenus()
        {
            var menuList = BaseControllerModel.DataContext.GetEntityAsync<SysPowerMenu>(u => u.MenuType == MenuType.Menu || u.MenuType == MenuType.Catalog).OrderBy(u => u.Id).ToList();
            //将获取到的sys转为view
            var viewMenuList = new List<ViewPowerMenu>().ListAutoAssign(menuList);
            var resultViewMenuList = new List<ViewPowerMenu>();
            foreach (var item in viewMenuList)
            {
                if (item.UpLayerId == 0)
                {
                    resultViewMenuList.Add(item);
                }
                else
                {
                    viewMenuList.SingleOrDefault(u => u.Id == item.UpLayerId)?.Children.Add(item);
                }
            }
            return resultViewMenuList;
        }
    }
}