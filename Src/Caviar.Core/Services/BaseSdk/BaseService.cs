using Caviar.SharedKernel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Services
{
    public partial class BaseSdk<T> :  IBaseSdk<T>   where T: class,IBaseEntity, new()
    {
        private IAppDbContext _dbContext;
        public IAppDbContext DbContext { 
            get 
            {
                if (_dbContext != null) return _dbContext;
                throw new SharedKernel.ApplicationException("未向Sdk注入DbContext");
            }
            set
            {
                _dbContext = value;
            }
        }

        /// <summary>
        /// 添加指定实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<int> AddEntity(T entity)
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
        public virtual async Task<T> UpdateEntity(T entity)
        {
            return await DbContext.UpdateEntityAsync(entity);
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
        public virtual async Task<bool> DeleteEntity(List<T> menus)
        {
            return await DbContext.DeleteEntityAsync(menus);
        }
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateEntity(List<T> menus)
        {
            return await DbContext.UpdateEntityAsync(menus);
        }
    }
}
