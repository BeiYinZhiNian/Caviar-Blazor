using Caviar.Control.Menu;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control.Permission
{
    public partial class PermissionController:CaviarBaseController
    {
        /// <summary>
        /// 获取角色菜单
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult RoleMenu(int roleId)
        {
            var menus = Action.GetRoleMenu(roleId);
            if (menus == null) return ResultForbidden("未查询到该角色权限，请联系管理员获取");
            ResultMsg.Data = menus;
            return ResultOK();
        }
        /// <summary>
        /// 设置角色菜单
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="menuIds"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RoleMenu(int roleId,int[] menuIds)
        {
            await Action.SetRoleMenu(roleId, menuIds);
            return ResultOK();
        }

        /// <summary>
        /// 获取所有model
        /// </summary>
        [HttpGet]
        public IActionResult GetModels()
        {
            List<ViewModelFields> viewModels = new List<ViewModelFields>();
            var types = CommonHelper.GetModelList();
            foreach (var item in types)
            {
                var displayName = item.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                viewModels.Add(new ViewModelFields() { TypeName = item.Name, DisplayName = displayName, FullName = item.FullName.Replace("." + item.Name, "") });
            }
            ResultMsg.Data = viewModels;
            return ResultOK();
        }

        /// <summary>
        /// 获取角色字段
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> RoleFields(string modelName, int roleId)
        {
            //if (string.IsNullOrEmpty(modelName)) return ResultError("请输入需要获取的数据名称");
            //var fields = await Action.GetRoleFields(modelName, roleId);
            //var modelFields = CavAssembly.GetViewModelHeaders(modelName);
            //foreach (var item in modelFields)
            //{
            //    var field = fields.FirstOrDefault(u => u.FullName == item.FullName && u.TypeName == item.TypeName);
            //    if (field != null)
            //    {
            //        item.IsDisable = field.IsDisable;
            //    }
            //}
            ResultMsg.Data = CavAssembly.GetViewModelHeaders(modelName);
            return ResultOK();
        }

        /// <summary>
        /// 设置角色字段
        /// </summary>
        /// <param name="viewModelFields"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RoleFields(string fullName, int roleId,List<ViewModelFields> viewModelFields)
        {
            var sysModelFields = CommonHelper.AToB<List<SysModelFields>,List<ViewModelFields>>(viewModelFields);
            await Action.SetRoleFields(fullName, roleId, sysModelFields);
            return ResultOK();
        }

    }
}
