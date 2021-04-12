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
            ResultMsg.Data = menuAction.GetLeftSideMenus();
            return ResultOK();
        }
    }
}