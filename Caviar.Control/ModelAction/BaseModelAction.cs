using Caviar.Models;
using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control.ModelAction
{
    public class BaseModelAction<T,ViewT> : IBaseModelAction<T, ViewT> where T : class, IBaseModel
    {
        /// <summary>
        /// 数据实体
        /// </summary>
        public T Entity { get; set; }

        public IBaseControllerModel BC { get; set; }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> AddEntity()
        {
            var count = await BC.DC.AddEntityAsync(Entity);
            return count;
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> DeleteEntity()
        {
            var count = await BC.DC.DeleteEntityAsync(Entity);
            return count;
        }
        /// <summary>
        /// 修改实体
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> UpdateEntity()
        {
            var count = await BC.DC.UpdateEntityAsync(Entity);
            return count;
        }
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <returns></returns>
        public virtual async Task<PageData<ViewT>> GetPages(Expression<Func<T, bool>> where, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true)
        {
            var pages = await BC.DC.GetPageAsync(where, u => u.Number, pageIndex, pageSize, isOrder, isNoTracking);
            var list = ModelToViewModel(pages.Rows);
            PageData<ViewT> viewPage = new PageData<ViewT>(list);
            return viewPage;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteEntity(List<T> menus)
        {
            var count = await BC.DC.DeleteEntityAsync(menus);
            return count;
        }
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateEntity(List<SysMenu> menus)
        {
            var count = await BC.DC.UpdateEntityAsync(menus);
            return count;
        }
        /// <summary>
        /// 数据转换
        /// 需要达到一个model转为viewModel效果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual List<ViewT> ModelToViewModel(List<T> model)
        {
            var outModel = CommonHelper.AToB<List<ViewT>, List<T>>(model);
            return outModel;
        }
    }
}
