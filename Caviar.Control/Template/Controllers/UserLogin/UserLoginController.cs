using Caviar.Control;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
/// <summary>
/// 生成者：未登录用户
/// 生成时间：2021/5/17 9:48:00
/// 代码由代码生成器自动生成，更改的代码可能被进行替换
/// 可在上层目录使用partial关键字进行扩展
/// </summary>
namespace Caviar.Control
{
    [DisplayName("用户登录控制器")]
    public partial class UserLoginController : CaviarBaseController
    {
        #region 属性注入
        UserLoginAction _action;
        /// <summary>
        /// 方法操作器
        /// </summary>
        UserLoginAction Action 
        {
            get 
            {
                if (_action == null)
                {
                    _action = CreateModel<UserLoginAction>();
                }
                return _action;
            }
            set
            {
                _action = value;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 添加用户登录
        /// </summary>
        /// <param name="UserLogin">方法操作器</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Add(UserLoginAction UserLogin)
        {
            try
            {
                var count = await UserLogin.AddEntity();
                if (count > 0)
                {
                    return ResultOK();
                }
            }
            catch(Exception e)
            {
                return ResultErrorMsg("添加失败", e.Message);
            }
            return ResultErrorMsg("添加失败");
        }

        /// <summary>
        /// 修改用户登录
        /// </summary>
        /// <param name="UserLogin">方法操作器</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Update(UserLoginAction UserLogin)
        {
            try
            {
                var count = await UserLogin.UpdateEntity();
                if (count > 0)
                {
                    return ResultOK();
                }
            }
            catch(Exception e)
            {
                return ResultErrorMsg("修改失败", e.Message);
            }
            return ResultErrorMsg("修改失败");
        }

        /// <summary>
        /// 删除用户登录
        /// </summary>
        /// <param name="UserLogin">方法操作器</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Delete(UserLoginAction UserLogin)
        {
            try
            {
                var count = await UserLogin.DeleteEntity();
                if (count > 0)
                {
                    return ResultOK();
                }
            }
            catch(Exception e)
            {
                return ResultErrorMsg("删除失败", e.Message);
            }
            return ResultErrorMsg("删除失败");
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
        public async Task<IActionResult> GetPages(int pageIndex = 1, int pageSize = 10, bool isOrder = true, bool isNoTracking = false)
        {
            var pages = await Action.GetPages(u => true, pageIndex, pageSize, isOrder, isNoTracking);
            ResultMsg.Data = pages;
            return ResultOK();
        }
        #endregion

    }
}