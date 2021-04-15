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
        public virtual List<SysPowerMenu> GetLeftSideMenus()
        {
            var menuList = BaseControllerModel.DataContext.GetEntityAsync<SysPowerMenu>(u => u.MenuType == MenuType.Menu || u.MenuType == MenuType.Catalog);
            return menuList.ToList();
        }
    }
}