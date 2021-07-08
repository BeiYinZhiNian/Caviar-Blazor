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
            var result = await _Action.AddEntity();
            return Ok(result);
        }

        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <param name="Menu">方法操作器</param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<IActionResult> Update(ViewT view)
        {
            var result = await _Action.UpdateEntity(); 
            return Ok(result);
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="Menu">方法操作器</param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<IActionResult> Delete(ViewT view)
        {
            var result = await _Action.DeleteEntity(); 
            return Ok(result);
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
            var result = await _Action.GetPages(u => true, pageIndex, pageSize, isOrder, isNoTracking);
            return Ok(result);
        }

        /// <summary>
        /// 模糊查询
        /// 使用权限验证sql的正确性
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<IActionResult> FuzzyQuery(ViewQuery query)
        {
            var fields = BC.UserData.ModelFields.Where(u => u.BaseTypeName == typeof(T).Name).ToList();
            if (fields == null) return ResultError("没有对该对象的查询权限");
            var result = await _Action.FuzzyQuery(query, fields);
            return Ok(result);
        }

        /// <summary>
        /// 只能获取自身字段
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual async Task<IActionResult> GetFields()
        {
            var permissionAction = CreateModel<PermissionAction>();
            var result = await permissionAction.GetFieldsData(CavAssembly, typeof(ViewT).Name);
            return Ok(result);
        }
        #endregion
    }
}
