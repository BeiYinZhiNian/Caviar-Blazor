using Caviar.Control;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.Control.Menu
{
    public partial class MenuController : CaviarBaseController
    {
        /// <summary>
        /// 获取该页面的按钮
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetButtons(string url)
        {
            if(url == null)
            {
                return ResultErrorMsg("请输入正确地址");
            }
            var menus = Action.GetEntitys(u => u.Url.ToLower() == url.ToLower()).FirstOrDefault();
            if (menus == null)
            {
                ResultMsg.Data = new List<SysMenu>();
                return ResultOK();
            }
            var buttons = Action.GetEntitys(u => u.MenuType == MenuType.Button && u.ParentId == menus.Id).OrderBy(u=>u.Number);
            ResultMsg.Data = buttons;
            return ResultOK();
        }

        /// <summary>
        /// 移除菜单,如有子菜单，默认移动到上一层
        /// </summary>
        /// <param name="viewMen"></param>
        /// <param name="IsDeleteAll">为True时删除全部子菜单</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Move(ViewMenu viewMen,bool IsDeleteAll)
        {
            if (IsDeleteAll)
            {
                return await DeleteAllEntity(viewMen);
            }
            Action.AutoAssign(viewMen);
            List<ViewMenu> viewMenuList = new List<ViewMenu>();
            viewMen.TreeToList(viewMenuList);
            var count = 0;
            if(viewMenuList!=null && viewMenuList.Count != 0)
            {
                List<SysMenu> menus = new List<SysMenu>();
                menus.ListAutoAssign(viewMenuList);
                foreach (var item in menus)
                {
                    if (item.ParentId == viewMen.Id)
                    {
                        item.ParentId = viewMen.ParentId;
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
        /// 批量删除
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        private async Task<IActionResult> DeleteAllEntity(ViewMenu viewMen)
        {
            List<ViewMenu> viewMenuList = new List<ViewMenu>();
            viewMen.TreeToList(viewMenuList);
            viewMenuList.Add(viewMen);//将自己添加入删除集合
            List<SysMenu> menus = new List<SysMenu>();
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