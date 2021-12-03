using Caviar.Core.Interface;
using Caviar.SharedKernel;
using Caviar.SharedKernel.View;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Services
{
    [DIInject]
    public partial class BaseServices
    {

    }
    
    public partial class DbServices : BaseServices
    {
        protected IAppDbContext _dbContext;
        public IAppDbContext DbContext
        {
            get
            {
                if (_dbContext != null) return _dbContext;
                throw new ApplicationException("未向Service注入DbContext");
            }
            set
            {
                _dbContext = value;
            }
        }
        public DbServices()
        {

        }

        public DbServices(IAppDbContext dbContext)
        {
            DbContext = dbContext;
        }


        /// <summary>
        /// 添加指定实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<int> CreateEntity<T>(T entity) where T : class, IBaseEntity, new()
        {
            var id = await DbContext.AddEntityAsync(entity);
            return id;
        }

        /// <summary>
        /// 删除指定实体
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> DeleteEntity<T>(T entity) where T : class, IBaseEntity, new()
        {
            return await DbContext.DeleteEntityAsync(entity);
        }

        /// <summary>
        /// 修改指定实体
        /// </summary>
        /// <returns></returns>
        public virtual Task UpdateEntity<T>(T entity) where T : class, IBaseEntity, new()
        {
            return DbContext.UpdateEntityAsync(entity);
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <returns></returns>
        public virtual async Task<PageData<T>> GetPages<T>(Expression<Func<T, bool>> where, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true) where T : class, IBaseEntity, new()
        {
            return await DbContext.GetPageAsync(where, u => u.Number, pageIndex, pageSize, isOrder, isNoTracking);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual Task DeleteEntity<T>(IEnumerable<T> menus) where T : class, IBaseEntity, new()
        {
            return DbContext.DeleteEntityAsync(menus);
        }
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual Task UpdateEntity<T>(IEnumerable<T> menus) where T : class, IBaseEntity, new()
        {
            return DbContext.UpdateEntityAsync(menus);
        }

        public virtual Task<T> GetEntity<T>(int id) where T : class, IBaseEntity, new()
        {
            return DbContext.SingleOrDefaultAsync<T>(u => u.Id == id);
        }

        public virtual Task<List<T>> GetEntity<T>(Expression<Func<T, bool>> where) where T : class, IBaseEntity, new()
        {
            return DbContext.GetEntityAsync(where);
        }

    }

    
    public partial class EasyBaseServices<T> : DbServices,IEasyBaseServices<T>   where T: class,IBaseEntity, new()
    {
        private new IEasyDbContext<T> _dbContext;
        public new IEasyDbContext<T> DbContext { 
            get 
            {
                if (_dbContext != null) return _dbContext;
                throw new ApplicationException("未向Service注入DbContext");
            }
            set
            {
                _dbContext = value;
            }
        }

        public EasyBaseServices()
        {

        }
        public EasyBaseServices(IEasyDbContext<T> dbContext):base(dbContext)
        {
            DbContext = dbContext;
        }

        /// <summary>
        /// 添加指定实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<int> CreateEntity(T entity)
        {
            var id = await DbContext.AddEntityAsync(entity);
            return id;
        }
       
        /// <summary>
        /// 删除指定实体
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> DeleteEntity(T entity)
        {
            return await DbContext.DeleteEntityAsync(entity);
        }
        
        /// <summary>
        /// 修改指定实体
        /// </summary>
        /// <returns></returns>
        public virtual Task UpdateEntity(T entity)
        {
            return DbContext.UpdateEntityAsync(entity);
        }
        
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <returns></returns>
        public virtual async Task<PageData<T>> GetPages(Expression<Func<T, bool>> where, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true)
        {
            return await DbContext.GetPageAsync(where, u => u.Number, pageIndex, pageSize, isOrder, isNoTracking);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual Task DeleteEntity(IEnumerable<T> menus)
        {
            return DbContext.DeleteEntityAsync(menus);
        }
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual Task UpdateEntity(IEnumerable<T> menus)
        {
            return DbContext.UpdateEntityAsync(menus);
        }

        public virtual Task<T> GetEntity(int id)
        {
            return DbContext.SingleOrDefaultAsync(u => u.Id == id);
        }

        public virtual Task<List<T>> GetEntity(Expression<Func<T, bool>> where)
        {
            return DbContext.GetEntityAsync(where);
        }
    }
}
