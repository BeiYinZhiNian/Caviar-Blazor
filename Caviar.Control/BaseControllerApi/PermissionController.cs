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
        public async Task<IActionResult> RoleMenu(int roleId)
        {
            var menus = await Action.GetRoleMenu(roleId);
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

        [HttpGet]
        public async Task<IActionResult> GetFields(string modelName)
        {
            if (string.IsNullOrEmpty(modelName)) return ResultError("请输入需要获取的数据名称");
            var fields = await Action.GetRoleFields(modelName);
            var modelFields = CavAssembly.GetViewModelHeaders(modelName);//其他信息
            var viewFields = new List<ViewModelFields>();
            foreach (var item in modelFields)
            {
                var field = fields.FirstOrDefault(u => u.FullName == item.FullName && u.TypeName == item.TypeName);
                if (field != null && !field.IsDisable)
                {
                    item.IsDisable = field.IsDisable;
                    item.Width = field.Width;
                    viewFields.Add(item);
                }
            }
            ResultMsg.Data = viewFields;
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
            var modelFields = CavAssembly.GetViewModelHeaders(modelName);
            var fields = await Action.GetRoleFields(modelName, roleId);
            var viewFields = new List<ViewModelFields>();
            foreach (var item in modelFields)
            {
                var field = fields.FirstOrDefault(u => u.FullName == item.FullName && u.TypeName == item.TypeName);
                if (field != null && (!field.IsDisable || BC.IsAdmin))
                {
                    item.IsDisable = field.IsDisable;
                    item.Width = field.Width;
                    viewFields.Add(item);
                }
            }
            ResultMsg.Data = viewFields;
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
            viewModelFields.AToB(out List<SysModelFields> sysModelFields);
            await Action.SetRoleFields(fullName, roleId, sysModelFields);
            return ResultOK();
        }

    }
}
