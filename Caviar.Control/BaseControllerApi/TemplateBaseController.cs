using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Caviar.Models.SystemData;
using System.Collections.Generic;
using Caviar.Control.Permission;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace Caviar.Control
{
    /// <summary>
    /// 模板控制器
    /// </summary>
    /// <typeparam name="ModelAction">模型方法</typeparam>
    /// <typeparam name="T">模型</typeparam>
    /// <typeparam name="ViewT">前端模型</typeparam>
    public partial class TemplateBaseController<ModelAction,T,ViewT>: CaviarBaseController where ModelAction : class, IBaseModelAction<T, ViewT> where T:class, IBaseModel where ViewT:T
    {

        #region 属性注入
        ModelAction _action;
        /// <summary>
        /// 方法操作器
        /// </summary>
        protected virtual ModelAction _Action 
        {
            get
            {
                if (_action == null)
                {
                    _action = CreateModel<ModelAction>();
                    if (BC.ActionArguments.Count > 0)
                    {
                        //在这里可以将自己想要的属性通过接口注入
                        foreach (var ArgumentsItem in BC.ActionArguments)
                        {
                            //获取前端模型
                            if (ArgumentsItem.Value is ViewT)
                            {
                                //转到后端模型
                                _action.Entity = (T)ArgumentsItem.Value;
                            }
                            else if(ArgumentsItem.Value is IActionModel)
                            {
                                ((IActionModel)ArgumentsItem.Value).BC = BC;
                            }
                        }
                    }
                }
                return _action;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="Menu">方法操作器</param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<IActionResult> Add(ViewT view)
        {
            try
            {
                var count = await _Action.AddEntity();
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
        public virtual async Task<IActionResult> Update(ViewT view)
        {
            try
            {
                var count = await _Action.UpdateEntity();
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
        public virtual async Task<IActionResult> Delete(ViewT view)
        {
            try
            {
                var count = await _Action.DeleteEntity();
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
            var pages = await _Action.GetPages(u => true, pageIndex, pageSize, isOrder, isNoTracking);
            ResultMsg.Data = pages;
            return ResultOK();
        }

        /// <summary>
        /// 模糊查询
        /// 使用权限验证sql的正确性
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> FuzzyQuery(ViewQuery query)
        {
            var fields = BC.UserData.ModelFields.Where(u => u.BaseTypeName == typeof(T).Name).ToList();
            if (fields == null) return ResultError("没有对该对象的查询权限");
            var data = await _Action.FuzzyQuery(query, fields);
            if(data == null) return ResultError("没有对该对象的查询权限,查询时出错");
            return ResultOK();
        }
        #endregion
    }
}
