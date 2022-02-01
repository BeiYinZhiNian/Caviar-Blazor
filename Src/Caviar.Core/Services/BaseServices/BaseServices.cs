using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Caviar.Core.Services
{
    [DIInject]
    public partial class BaseServices
    {

    }
    
    public partial class DbServices : BaseServices
    {
        protected IAppDbContext _appDbContext;
        public IAppDbContext AppDbContext
        {
            get
            {
                if (_appDbContext != null) return _appDbContext;
                throw new ApplicationException("未向Service注入DbContext");
            }
            set
            {
                _appDbContext = value;
            }
        }
        public DbServices()
        {

        }

        public DbServices(IAppDbContext dbContext)
        {
            AppDbContext = dbContext;
        }


        /// <summary>
        /// 添加指定实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<int> CreateEntity<T>(T entity) where T : class, IUseEntity, new()
        {
            var id = await AppDbContext.AddEntityAsync(entity);
            return id;
        }

        /// <summary>
        /// 删除指定实体
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> DeleteEntity<T>(T entity) where T : class, IUseEntity, new()
        {
            return await AppDbContext.DeleteEntityAsync(entity);
        }

        /// <summary>
        /// 修改指定实体
        /// </summary>
        /// <returns></returns>
        public virtual Task UpdateEntity<T>(T entity) where T : class, IUseEntity, new()
        {
            return AppDbContext.UpdateEntityAsync(entity);
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <returns></returns>
        public virtual async Task<PageData<T>> GetPages<T>(Expression<Func<T, bool>> where, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true) where T : class, IUseEntity, new()
        {
            return await AppDbContext.GetPageAsync(where, u => u.Number, pageIndex, pageSize, isOrder, isNoTracking);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual Task DeleteEntity<T>(IEnumerable<T> menus) where T : class, IUseEntity, new()
        {
            return AppDbContext.DeleteEntityAsync(menus);
        }
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual Task UpdateEntity<T>(IEnumerable<T> menus) where T : class, IUseEntity, new()
        {
            return AppDbContext.UpdateEntityAsync(menus);
        }

        public virtual Task<T> GetEntity<T>(int id) where T : class, IUseEntity, new()
        {
            return AppDbContext.SingleOrDefaultAsync<T>(u => u.Id == id);
        }

        public virtual Task<List<T>> GetEntity<T>(Expression<Func<T, bool>> where) where T : class, IUseEntity, new()
        {
            return AppDbContext.GetEntityAsync(where);
        }

        public virtual Task<T> GetEntity<T>(Expression<Func<T, bool>> where,bool isSingle) where T : class, IUseEntity, new()
        {
            return AppDbContext.SingleOrDefaultAsync<T>(where);
        }

    }

    
    public partial class EasyBaseServices<T> : DbServices,IEasyBaseServices<T>   where T: class,IUseEntity, new()
    {

        public EasyBaseServices()
        {

        }
        public EasyBaseServices(IAppDbContext dbContext):base(dbContext)
        {
            AppDbContext = dbContext;
        }

        /// <summary>
        /// 添加指定实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<int> CreateEntity(T entity)
        {
            var id = await AppDbContext.AddEntityAsync(entity);
            return id;
        }

        /// <summary>
        /// 删除指定实体
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> DeleteEntity(T entity)
        {
            return await AppDbContext.DeleteEntityAsync(entity);
        }

        /// <summary>
        /// 修改指定实体
        /// </summary>
        /// <returns></returns>
        public virtual Task UpdateEntity(T entity)
        {
            return AppDbContext.UpdateEntityAsync(entity);
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <returns></returns>
        public virtual async Task<PageData<T>> GetPages(Expression<Func<T, bool>> where, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true)
        {
            return await AppDbContext.GetPageAsync(where, u => u.Number, pageIndex, pageSize, isOrder, isNoTracking);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual Task DeleteEntity(IEnumerable<T> menus)
        {
            return AppDbContext.DeleteEntityAsync(menus);
        }
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual Task UpdateEntity(IEnumerable<T> menus)
        {
            return AppDbContext.UpdateEntityAsync(menus);
        }

        public virtual Task<T> GetEntity(int id)
        {
            return AppDbContext.SingleOrDefaultAsync<T>(u => u.Id == id);
        }

        public virtual Task<List<T>> GetEntity(Expression<Func<T, bool>> where)
        {
            return AppDbContext.GetEntityAsync(where);
        }

        public virtual Task<List<T>> GetAllAsync()
        {
            return AppDbContext.GetAllAsync<T>();
        }

        public virtual Task<T> GetEntity(Expression<Func<T, bool>> where, bool isSingle)
        {
            return AppDbContext.SingleOrDefaultAsync(where);
        }
    }
}
