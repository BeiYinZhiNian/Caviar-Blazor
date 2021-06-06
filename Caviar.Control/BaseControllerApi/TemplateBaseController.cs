using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Caviar.Models.SystemData;
namespace Caviar.Control.BaseControllerApi
{
    public partial class TemplateBaseController<T,ViewT>: CaviarBaseController where T : class, IBaseModel
    {

        #region 属性注入
        /// <summary>
        /// 方法操作器
        /// </summary>
        protected virtual IBaseModelAction<T, ViewT> Action { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="Menu">方法操作器</param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<IActionResult> Add(IBaseModelAction<T, ViewT> Menu)
        {
            try
            {
                var count = await Menu.AddEntity();
                if (count > 0)
                {
                    return ResultOK();
                }
            }
            catch (Exception e)
            {
                return ResultError("添加失败", e.Message);
            }
            return ResultError("添加失败");
        }

        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <param name="Menu">方法操作器</param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<IActionResult> Update(IBaseModelAction<T, ViewT> Menu)
        {
            try
            {
                var count = await Menu.UpdateEntity();
                if (count > 0)
                {
                    return ResultOK();
                }
            }
            catch (Exception e)
            {
                return ResultError("修改失败", e.Message);
            }
            return ResultError("修改失败");
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="Menu">方法操作器</param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<IActionResult> Delete(IBaseModelAction<T, ViewT> Menu)
        {
            try
            {
                var count = await Menu.DeleteEntity();
                if (count > 0)
                {
                    return ResultOK();
                }
            }
            catch (Exception e)
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
        public virtual async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, bool isOrder = true, bool isNoTracking = true)
        {
            var pages = await Action.GetPages(u => true, pageIndex, pageSize, isOrder, isNoTracking);
            ResultMsg.Data = pages;
            return ResultOK();
        }
        #endregion
    }
}
