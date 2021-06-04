using Caviar.Control;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Caviar.Models.SystemData;
/// <summary>
/// 生成者：admin
/// 生成时间：2021/6/4 18:30:57
/// 代码由代码生成器自动生成，更改的代码可能被进行替换
/// 可在上层目录使用partial关键字进行扩展
/// </summary>
namespace Caviar.Control.Role
{
    [DisplayName("角色控制器")]
    public partial class RoleController : CaviarBaseController
    {
        #region 属性注入
        RoleAction _action;
        /// <summary>
        /// 方法操作器
        /// </summary>
        RoleAction Action 
        {
            get 
            {
                if (_action == null)
                {
                    SetMenuAction();
                    if(_action == null)
                    {
                        _action = CreateModel<RoleAction>();
                    }
                }
                return _action;
            }
            set
            {
                _action = value;
            }
        }

        partial void SetMenuAction();
        #endregion

        #region 方法
        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="Role">方法操作器</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Add(RoleAction Role)
        {
            try
            {
                var count = await Role.AddEntity();
                if (count > 0)
                {
                    return ResultOK();
                }
            }
            catch(Exception e)
            {
                return ResultError("添加失败", e.Message);
            }
            return ResultError("添加失败");
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="Role">方法操作器</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Update(RoleAction Role)
        {
            try
            {
                var count = await Role.UpdateEntity();
                if (count > 0)
                {
                    return ResultOK();
                }
            }
            catch(Exception e)
            {
                return ResultError("修改失败", e.Message);
            }
            return ResultError("修改失败");
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="Role">方法操作器</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Delete(RoleAction Role)
        {
            try
            {
                var count = await Role.DeleteEntity();
                if (count > 0)
                {
                    return ResultOK();
                }
            }
            catch(Exception e)
            {
                return ResultError("删除失败", e.Message);
            }
            return ResultError("删除失败");
        }
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="isOrder"></param>
        /// <param name="isNoTracking"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, bool isOrder = true, bool isNoTracking = true)
        {
            var pages = await Action.GetPages(u => true, pageIndex, pageSize, isOrder, isNoTracking);
            ResultMsg.Data = pages;
            return ResultOK();
        }
        #endregion

    }
}