using Caviar.Models.SystemData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caviar.Control
{
    public partial interface IDataContext
    {
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> AddEntityAsync<T>(T entity, bool isSaveChange = true) where T : class,IBaseModel;
        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> UpdateEntityAsync<T>(T entity, bool isSaveChange = true) where T : class,IBaseModel;
        /// <summary>
        /// 修改部分实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isSaveChange"></param>
        /// <param name="updatePropertyList"></param>
        /// <param name="modified"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync<T>(T entity, Expression<Func<T, object>> fieldExp, bool isSaveChange = true) where T : class,IBaseModel;
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="IsDelete">是否彻底删除，默认不彻底删除</param>
        /// <returns></returns>
        public Task<int> DeleteEntityAsync<T>(T entity, bool isSaveChange = true, bool IsDelete = false) where T : class,IBaseModel;
        /// <summary>
        /// 异步获取所有数据
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetAllAsync<T>() where T : class,IBaseModel;
        /// <summary>
        /// 根据条件获取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public IQueryable<T> GetEntity<T>(Expression<Func<T, bool>> where) where T : class, IBaseModel;
        /// <summary>
        /// 根据id获取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<T> GetEntityAsync<T>(int id) where T : class, IBaseModel;
        /// <summary>
        /// 根据uid获取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Task<T> GetEntityAsync<T>(Guid uid) where T : class, IBaseModel;
        /// <summary>
        /// 分页查询异步
        /// </summary>
        /// <param name="whereLambda">查询添加（可有，可无）</param>
        /// <param name="ordering">排序条件（一定要有）</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="isOrder">排序正反</param>
        /// <returns></returns>
        public Task<PageData<T>> GetPageAsync<T,TKey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true) where T:class,IBaseModel;

    }
}
