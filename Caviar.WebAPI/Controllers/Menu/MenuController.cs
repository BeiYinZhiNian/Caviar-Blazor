using Caviar.Control;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Caviar.WebAPI.Controllers
{
    public class MenuController : BaseController
    {
        [HttpGet]
        public IActionResult GetLeftSideMenus()
        {
            var menuAction = CreateModel<SysPowerMenuAction>();
            ResultMsg.Data = menuAction.GetMenus(u => u.MenuType == MenuType.Catalog || u.MenuType == MenuType.Menu);
            return ResultOK();
        }

        [HttpGet]
        public IActionResult GetCatalogMenus()
        {
            var menuAction = CreateModel<SysPowerMenuAction>();
            ResultMsg.Data = menuAction.GetMenus(u => u.MenuType == MenuType.Catalog);
            return ResultOK();
        }
    }
}