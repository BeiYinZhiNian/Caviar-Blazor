using Caviar.SharedKernel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caviar.Core.Interface
{
    public partial interface IAppDbContext
    {
        /// <summary>
        /// 保存操作
        /// </summary>
        /// <param name="IsFieldCheck">确保为系统内部更改时，可以取消字段验证</param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(bool IsFieldCheck = true);

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> AddEntityAsync<T>(T entity, bool isSaveChange = true) where T : class, IBaseEntity, new();
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isSaveChange"></param>
        /// <returns></returns>
        Task<bool> AddEntityAsync<T>(IEnumerable<T> entity, bool isSaveChange = true) where T : class, IBaseEntity, new();
        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task UpdateEntityAsync<T>(T entity, bool isSaveChange = true) where T : class, IBaseEntity, new();
        /// <summary>
        /// 修改部分实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isSaveChange"></param>
        /// <param name="updatePropertyIEnumerable"></param>
        /// <param name="modified"></param>
        /// <returns></returns>
        public Task UpdateEntityAsync<T>(T entity, Expression<Func<T, object>> fieldExp, bool isSaveChange = true) where T : class, IBaseEntity, new();
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="fieldExp"></param>
        /// <param name="isSaveChange"></param>
        /// <returns></returns>
        public Task UpdateEntityAsync<T>(IEnumerable<T> entity, bool isSaveChange = true) where T : class, IBaseEntity, new();

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="IsDelete">是否彻底删除，默认不彻底删除</param>
        /// <returns>返回代表是否真正删除了实体，如果为true则是物理删除，如果为false则是逻辑删除</returns>
        public Task<bool> DeleteEntityAsync<T>(T entity, bool isSaveChange = true, bool IsDelete = false) where T : class, IBaseEntity, new();
        /// <summary>
        /// 批量删除
        /// 会进行事务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isSaveChange"></param>
        /// <param name="IsDelete"></param>
        /// <returns></returns>
        public Task DeleteEntityAsync<T>(IEnumerable<T> entity, bool isSaveChange = true, bool IsDelete = false) where T : class, IBaseEntity, new();
        /// <summary>
        /// 异步获取所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isNoTracking">是否开启数据追踪</param>
        /// <param name="isDataPermissions">是否开启数据权限</param>
        /// <param name="isRecycleBin">是否读取回收站数据</param>
        /// <returns></returns>
        public Task<List<T>> GetAllAsync<T>(bool isNoTracking = true, bool isDataPermissions = true, bool isRecycleBin = false) where T : class, IBaseEntity, new();
        /// <summary>
        /// 根据条件获取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public Task<List<T>> GetEntityAsync<T>(Expression<Func<T, bool>> where, bool isNoTracking = true, bool isDataPermissions = true, bool isRecycleBin = false) where T : class, IBaseEntity, new();
        /// <summary>
        /// 根据条件获取单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public Task<T> SingleOrDefaultAsync<T>(Expression<Func<T, bool>> where, bool isNoTracking = true, bool isDataPermissions = true, bool isRecycleBin = false) where T : class, IBaseEntity, new();
        /// <summary>
        /// 根据条件获取首个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="isNoTracking"></param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> where, bool isNoTracking = true, bool isDataPermissions = true, bool isRecycleBin = false) where T : class, IBaseEntity, new();
        /// <summary>
        /// 分页查询异步
        /// </summary>
        /// <param name="whereLambda">查询添加（可有，可无）</param>
        /// <param name="ordering">排序条件（一定要有）</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="isOrder">排序正反</param>
        /// <returns></returns>
        public Task<PageData<T>> GetPageAsync<T, TKey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true, bool isDataPermissions = true, bool isRecycleBin = false) where T : class, IBaseEntity, new();
        /// <summary>
        /// 执行sql，请注意参数的检查防止sql注入
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task<DataTable> SqlQueryAsync(string sql, params object[] parameters);
    }
}
