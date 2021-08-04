using Caviar.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Services
{
    /// <summary>
    /// 基础sdk封装
    /// </summary>
    /// <typeparam name="ViewT"></typeparam>
    public interface IBaseSdk<ViewT> : IDIinjectAtteribute where ViewT : class, IView, new()
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        public IAppDbContext DbContext { get; set; }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <returns>实体id</returns>
        public Task<int> AddEntity(ViewT entity);
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <returns>是否删除成功</returns>
        public Task<bool> DeleteEntity(ViewT entity);
        /// <summary>
        /// 修改实体
        /// </summary>
        /// <returns>修改后实体</returns>
        public Task<ViewT> UpdateEntity(ViewT entity);
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <returns></returns>
        public Task<PageData<ViewT>> GetPages(Expression<Func<ViewT, bool>> where, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public Task<bool> DeleteEntity(List<ViewT> menus);
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public Task<bool> UpdateEntity(List<ViewT> menus);
    }
}
