using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caviar.SharedKernel
{
    public partial interface IAppDbContext
    {
        /// <summary>
        /// 保存操作
        /// </summary>
        /// <param name="IsFieldCheck">确保为系统内部更改时，可以取消验证</param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(bool IsFieldCheck = true);

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> AddEntityAsync<T>(T entity, bool isSaveChange = true) where T : class, IView, new();
        /// <summary>
        /// 批量添加
        /// 会进行事务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isSaveChange"></param>
        /// <returns></returns>
        Task<bool> AddEntityAsync<T>(List<T> entity, bool isSaveChange = true) where T : class, IView, new();
        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<T> UpdateEntityAsync<T>(T entity, bool isSaveChange = true) where T : class, IView, new();
        /// <summary>
        /// 修改部分实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isSaveChange"></param>
        /// <param name="updatePropertyList"></param>
        /// <param name="modified"></param>
        /// <returns></returns>
        public Task<T> UpdateEntityAsync<T>(T entity, Expression<Func<T, object>> fieldExp, bool isSaveChange = true) where T : class, IView, new();
        /// <summary>
        /// 批量修改
        /// 会进行事务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="fieldExp"></param>
        /// <param name="isSaveChange"></param>
        /// <returns></returns>
        public Task<bool> UpdateEntityAsync<T>(List<T> entity, bool isSaveChange = true) where T : class, IView, new();

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="IsDelete">是否彻底删除，默认不彻底删除</param>
        /// <returns></returns>
        public Task<bool> DeleteEntityAsync<T>(T entity, bool isSaveChange = true, bool IsDelete = false) where T : class, IView, new();
        /// <summary>
        /// 批量删除
        /// 会进行事务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isSaveChange"></param>
        /// <param name="IsDelete"></param>
        /// <returns></returns>
        public Task<bool> DeleteEntityAsync<T>(List<T> entity, bool isSaveChange = true, bool IsDelete = false) where T : class, IView, new();
        /// <summary>
        /// 异步获取所有数据
        /// </summary>
        /// <returns></returns>
        public Task<IQueryable<T>> GetAllAsync<T>(bool isNoTracking = true, bool isDataPermissions = true, bool isRecycleBin = false) where T : class, IView, new();
        /// <summary>
        /// 根据条件获取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public Task<IQueryable<T>> GetEntityAsync<T>(Expression<Func<T, bool>> where, bool isNoTracking = true, bool isDataPermissions = true, bool isRecycleBin = false) where T : class, IView, new();
        /// <summary>
        /// 根据条件获取单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public Task<T> GetSingleEntityAsync<T>(Expression<Func<T, bool>> where, bool isNoTracking = true, bool isDataPermissions = true, bool isRecycleBin = false) where T : class, IView, new();
        /// <summary>
        /// 根据条件获取首个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="isNoTracking"></param>
        /// <returns></returns>
        public Task<T> GetFirstEntityAsync<T>(Expression<Func<T, bool>> where, bool isNoTracking = true, bool isDataPermissions = true, bool isRecycleBin = false) where T : class, IView, new();
        /// <summary>
        /// 分页查询异步
        /// </summary>
        /// <param name="whereLambda">查询添加（可有，可无）</param>
        /// <param name="ordering">排序条件（一定要有）</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="isOrder">排序正反</param>
        /// <returns></returns>
        public Task<PageData<T>> GetPageAsync<T, TKey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true, bool isDataPermissions = true, bool isRecycleBin = false) where T : class, IView, new();
        /// <summary>
        /// 执行sql，请注意参数的检查防止sql注入
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task<DataTable> SqlQueryAsync(string sql, params object[] parameters);
    }
}
