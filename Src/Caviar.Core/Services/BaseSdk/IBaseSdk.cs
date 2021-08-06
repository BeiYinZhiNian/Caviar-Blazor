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
    /// <typeparam name="T"></typeparam>
    public interface IBaseSdk<T> : IDIinjectAtteribute where T : class, IBaseEntity, new()
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        public IAppDbContext DbContext { get; set; }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <returns>实体id</returns>
        public Task<int> AddEntity(T entity);
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <returns>是否删除成功</returns>
        public Task<bool> DeleteEntity(T entity);
        /// <summary>
        /// 修改实体
        /// </summary>
        /// <returns>修改后实体</returns>
        public Task<T> UpdateEntity(T entity);
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <returns></returns>
        public Task<PageData<T>> GetPages(Expression<Func<T, bool>> where, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public Task<bool> DeleteEntity(List<T> menus);
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public Task<bool> UpdateEntity(List<T> menus);
    }
}
