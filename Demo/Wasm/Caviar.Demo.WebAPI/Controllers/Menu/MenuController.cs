using Caviar.Control;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.Demo.WebAPI.Controllers
{
    public class MenuController : BaseController
    {
        SysPowerMenuAction _action;
        SysPowerMenuAction Action 
        {
            get 
            {
                if (_action == null)
                {
                    _action = CreateModel<SysPowerMenuAction>();
                }
                return _action;
            }
            set
            {
                _action = value;
            }
        }

        [HttpGet]
        public IActionResult GetLeftSideMenus()
        {
            
            ResultMsg.Data = Action.GetViewMenus(u => u.MenuType == MenuType.Catalog || u.MenuType == MenuType.Menu);
            return ResultOK();
        }

        [HttpGet]
        public IActionResult GetMenus()
        {
            
            ResultMsg.Data = Action.GetViewMenus(u => true);
            return ResultOK();
        }
        /// <summary>
        /// 获取该页面的按钮
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetButtons(string url)
        {
            if(url == null)
            {
                return ResultErrorMsg("请输入正确地址");
            }
            var Slash = url.Replace("/","").ToLower();
            var menus = Action.GetEntitys(u => u.Url.Replace("/", "").ToLower() == Slash).FirstOrDefault();
            if (menus == null)
            {
                ResultMsg.Data = new List<SysPowerMenu>();
                return ResultOK();
            }
            var buttons = Action.GetEntitys(u => u.MenuType == MenuType.Button && u.UpLayerId == menus.Id);
            ResultMsg.Data = buttons;
            return ResultOK();
        }

        [HttpPost]
        public async Task<IActionResult> AddEntity(SysPowerMenuAction MenuAction)
        {
            var count = await MenuAction.AddEntity();
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
            Action.AutoAssign(viewMen);
            List<ViewPowerMenu> viewMenuList = new List<ViewPowerMenu>();
            Action.ViewToModel(viewMen, viewMenuList);
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
                count = await Action.UpdateEntity(menus);
                if(count != viewMenuList.Count)
                {
                    return ResultErrorMsg("删除菜单失败,子菜单移动失败");
                }
            }
            count = await Action.DeleteEntity();
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
            List<ViewPowerMenu> viewMenuList = new List<ViewPowerMenu>();
            Action.ViewToModel(viewMen, viewMenuList);//获取子菜单集合
            viewMenuList.Add(viewMen);//将自己添加入删除集合
            List<SysPowerMenu> menus = new List<SysPowerMenu>();
            menus.ListAutoAssign(viewMenuList);//将view转为sys
            var count = await Action.DeleteEntity(menus);
            if(count == menus.Count)
            {
                return ResultOK();
            }
            return ResultErrorMsg("批量删除数据失败");
        }



        
    }
}