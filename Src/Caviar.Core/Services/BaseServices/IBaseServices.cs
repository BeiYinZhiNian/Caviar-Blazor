using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Caviar.Core.Services
{
    /// <summary>
    /// 基础sdk封装
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEasyBaseServices<T> : IDIinjectAtteribute where T : class, IUseEntity, new()
    {
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <returns>实体id</returns>
        public Task<int> CreateEntity(T entity);
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <returns>是否删除成功</returns>
        public Task<bool> DeleteEntity(T entity);
        /// <summary>
        /// 修改实体
        /// </summary>
        /// <returns>修改后实体</returns>
        public Task UpdateEntity(T entity);
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
        public Task DeleteEntity(IEnumerable<T> menus);
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public Task UpdateEntity(IEnumerable<T> menus);
        /// <summary>
        /// 获取指定实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<T> GetEntity(int id);
        /// <summary>
        /// 根据条件获取列表
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IQueryable<T> GetEntity(Expression<Func<T, bool>> where);

        public Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> where);
    }
}
