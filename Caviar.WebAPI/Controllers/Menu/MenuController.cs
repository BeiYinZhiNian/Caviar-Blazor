using Caviar.Control;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        [HttpPost]
        public async Task<IActionResult> AddEntity(SysPowerMenuAction action)
        {
            var count = await action.AddEntity();
            if (count > 0)
            {
                return ResultOK();
            }
            return ResultErrorMsg("添加菜单失败");
        }

        /// <summary>
        /// 删除并将子菜单移动到上一层
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> MoveEntity(ViewPowerMenu viewMen)
        {
            var menuAction = CreateModel<SysPowerMenuAction>();
            menuAction.AutoAssign(viewMen);
            List<ViewPowerMenu> viewMenuList = new List<ViewPowerMenu>();
            menuAction.RecursionGetMenu(viewMen, viewMenuList);
            var count = 0;
            if(viewMenuList!=null && viewMenuList.Count != 0)
            {
                List<SysPowerMenu> menus = new List<SysPowerMenu>();
                menus.ListAutoAssign(viewMenuList);
                foreach (var item in menus)
                {
                    if (item.UpLayerId == viewMen.Id)
                    {
                        item.UpLayerId = viewMen.UpLayerId;
                    }
                }
                count = await menuAction.UpdateEntity(menus);
                if(count != viewMenuList.Count)
                {
                    return ResultErrorMsg("删除菜单失败,子菜单移动失败");
                }
            }
            count = await menuAction.DeleteEntity();
            if (count > 0)
            {
                return ResultOK();
            }
            return ResultErrorMsg("删除菜单失败");
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Menu"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteEntity(SysPowerMenuAction Menu)
        {
            var count = await Menu.DeleteEntity();
            if (count > 0)
            {
                return ResultOK();
            }
            return ResultErrorMsg("删除菜单失败");
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteAllEntity(ViewPowerMenu viewMen)
        {
            var menuAction = CreateModel<SysPowerMenuAction>();

            List<ViewPowerMenu> viewMenuList = new List<ViewPowerMenu>();
            menuAction.RecursionGetMenu(viewMen, viewMenuList);//获取子菜单集合
            viewMenuList.Add(viewMen);//将自己添加入删除集合
            List<SysPowerMenu> menus = new List<SysPowerMenu>();
            menus.ListAutoAssign(viewMenuList);//将view转为sys
            var count = await menuAction.DeleteEntity(menus);
            if(count == menus.Count)
            {
                return ResultOK();
            }
            return ResultErrorMsg("批量删除数据失败");
        }



        
    }
}