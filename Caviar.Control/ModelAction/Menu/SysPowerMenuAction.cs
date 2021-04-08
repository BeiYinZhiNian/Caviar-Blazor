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
        IBaseControllerModel _controllerModel;
        public SysPowerMenuAction()
        {
            _controllerModel = this.GetControllerModel();
        }
        public virtual List<SysPowerMenu> GetMenus()
        {
            var menuList = _controllerModel.DataContext.GetAllAsync<SysPowerMenu>();
            return menuList.ToList();
        }
    }
}