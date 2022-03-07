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
    [DIInject]
    public partial class BaseServices
    {

    }
    
    public partial class DbServices : BaseServices
    {
        protected IAppDbContext _appDbContext;
        protected IAppDbContext AppDbContext
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
        public DbServices(IAppDbContext dbContext)
        {
            AppDbContext = dbContext;
        }

    }

    
    public partial class EasyBaseServices<T,Vm> : DbServices,IEasyBaseServices<T,Vm>   where T: class,IUseEntity, new() where Vm : class,IView<T>,new()
    {

        public EasyBaseServices(IAppDbContext dbContext) : base(dbContext)
        {

        }

        /// <summary>
        /// 添加指定实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<int> AddEntityAsync(T entity)
        {
            var id = await AppDbContext.AddEntityAsync(entity);
            return id;
        }

        /// <summary>
        /// 删除指定实体
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> DeleteEntityAsync(T entity)
        {
            return await AppDbContext.DeleteEntityAsync(entity);
        }

        /// <summary>
        /// 修改指定实体
        /// </summary>
        /// <returns></returns>
        public virtual Task<T> UpdateEntityAsync(T entity)
        {
            return AppDbContext.UpdateEntityAsync(entity);
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <returns></returns>
        public virtual async Task<PageData<Vm>> GetPageAsync(Expression<Func<T, bool>> where, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true)
        {
            var pages = await AppDbContext.GetPageAsync(where, u => u.Number, pageIndex, pageSize, isOrder, isNoTracking);
            return ToView(pages);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual Task<int> DeleteEntityAsync(IEnumerable<T> menus)
        {
            return AppDbContext.DeleteEntityAsync(menus);
        }
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual Task<int> UpdateEntityAsync(IEnumerable<T> menus)
        {
            return AppDbContext.UpdateEntityAsync(menus);
        }

        public virtual async Task<Vm> SingleByIdAsync(int id)
        {
            var entity = await AppDbContext.SingleOrDefaultAsync<T>(u => u.Id == id);
            return ToView(entity);
        }

        public virtual IQueryable<T> GetEntityAsync(Expression<Func<T, bool>> where)
        {
            return AppDbContext.GetEntityAsync(where);
        }
        

        public virtual Task<List<T>> GetAllAsync()
        {
            return AppDbContext.GetAllAsync<T>();
        }

        public virtual Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> where)
        {
            return AppDbContext.SingleOrDefaultAsync(where);
        }

        public virtual Task<PageData<Vm>> FuzzyQuery(QueryView query)
        {
            return null;
        }

        protected virtual Vm ToView(T entity)
        {
            var vm = new Vm() { Entity = entity };
            return vm;
        }

        protected virtual List<Vm> ToView(List<T> entitys)
        {
            if (entitys == null) return null;
            entitys = Sort(entitys);
            return entitys.Select(x => new Vm() { Entity = x }).ToList();
        }

        protected virtual List<T> Sort(List<T> entitys)
        {
            return entitys.OrderBy(u => u.Number).ToList();
        }

        protected virtual PageData<Vm> ToView(PageData<T> page)
        {

            var pageVm = new PageData<Vm>()
            {
                Rows = ToView(page.Rows),
                PageIndex = page.PageIndex,
                PageSize = page.PageSize,
                Total = page.Total
            };
            return pageVm;
        }

        protected virtual List<T> ToEntity(List<Vm> vm)
        {
            if (vm == null) return null;
            return vm.Select(v => v.Entity).ToList();
        }


    }
}
