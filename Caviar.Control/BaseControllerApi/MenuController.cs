using Caviar.Control;
using Caviar.Control.Permission;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caviar.Control.Menu
{
    public partial class MenuController
    {

        /// <summary>
        /// 获取该页面的按钮
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetButtons(string url)
        {
            var result = _Action.GetButton(url);
            return Ok(result);
        }
        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetMenus()
        {
            var menus = _Action.GetMenus();
            return Ok(menus);
        }

        public override async Task<IActionResult> Delete(ViewMenu view)
        {
            ResultMsg result = null;
            if (view.IsDeleteAll)
            {
                result = await _Action.DeleteEntityAll(view);
            }
            else
            {
                result = await _Action.DeleteEntity(view);
            }
            return Ok(result);
        }

        public override async Task<IActionResult> Add(ViewMenu view)
        {
            var result = await base.Add(view);
            if (view.Id > 0)
            {
                var permissionAction = CreateModel<PermissionAction>();
                await permissionAction.SetMenuUser(view.Id);
            }
            return result;
        }
    }
}