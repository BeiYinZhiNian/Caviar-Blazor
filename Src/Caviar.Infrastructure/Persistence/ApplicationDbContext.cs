using Caviar.Core;
using Caviar.Infrastructure.Identity;
using Caviar.Infrastructure.Persistence.SysDbContext;
using Caviar.SharedKernel;
using Caviar.SharedKernel.Exceptions;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Infrastructure.Persistence
{
    public class ApplicationDbContext<TUser, TRole, TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        protected SysDbContext<TUser, TRole, TKey> DbContext { get;private set; }
        protected Interactor Interactor { get; private set; }

        protected ILanguageService LanguageService { get; set; }

        public ApplicationDbContext(SysDbContext<TUser, TRole, TKey> identityDbContext, Interactor interactor, ILanguageService languageService)
        {
            DbContext = identityDbContext;
            Interactor = interactor;
            LanguageService = languageService;
        }

        void IsEntityNull<T>(T entity)
        {
            if (entity == null)
            {
                var errorMsg = LanguageService.Resources["app"]["null"]["Db"].ToString();
                var name = typeof(T).Name;
                errorMsg = errorMsg.Replace("{entityName}", name);
                throw new DbException(errorMsg);
            }
        }


        /// <summary>
        /// 添加实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isSaveChange">默认为立刻保存</param>
        /// <returns></returns>
        public virtual async Task<int> AddEntityAsync<T>(T entity, bool isSaveChange = true) where T : class, IBaseEntity, new()
        {
            IsEntityNull(entity);
            DbContext.Entry(entity).State = EntityState.Added;
            if (isSaveChange)
            {
                await SaveChangesAsync();
                return entity.Id;
            }
            return -1;
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isSaveChange"></param>
        /// <returns></returns>
        public virtual async Task<bool> AddEntityAsync<T>(IEnumerable<T> entity, bool isSaveChange = true) where T : class, IBaseEntity, new()
        {
            IsEntityNull(entity);
            DbContext.AddRange(entity);
            if (isSaveChange)
            {
                await SaveChangesAsync();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 修改指定实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isSaveChange">默认为立刻保存</param>
        /// <returns></returns>
        public virtual async Task<T> UpdateEntityAsync<T>(T entity, bool isSaveChange = true) where T : class, IBaseEntity,new()
        {
            IsEntityNull(entity);
            DbContext.Entry(entity).State = EntityState.Modified;
            if (isSaveChange)
            {
                await SaveChangesAsync();
                return entity;
            }
            return entity;
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="fieldExp"></param>
        /// <param name="isSaveChange"></param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateEntityAsync<T>(IEnumerable<T> entity, bool isSaveChange = true) where T : class, IBaseEntity,new()
        {
            IsEntityNull(entity);
            DbContext.UpdateRange(entity);
            if (isSaveChange)
            {
                await SaveChangesAsync();
                return true;
            }
            return false;
        }


        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isSaveChange">默认立刻保存</param>
        /// <param name="IsDelete">是否立刻删除，默认不删除，只修改IsDelete,设为true则立刻删除</param>
        /// <returns>返回代表是否真正删除了实体，如果为true则是物理删除，如果为false则是逻辑删除</returns>
        public virtual async Task<bool> DeleteEntityAsync<T>(T entity, bool isSaveChange = true, bool IsDelete = false) where T : class, IBaseEntity, new()
        {
            IsEntityNull(entity);
            if (entity.IsDelete || IsDelete)
            {
                DbContext.Entry(entity).State = EntityState.Deleted;
                if (isSaveChange)
                {
                    await SaveChangesAsync();
                    return true;
                }
            }
            else
            {
                entity.IsDelete = true;
                await UpdateEntityAsync(entity, isSaveChange);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isSaveChange"></param>
        /// <param name="IsDelete"></param>
        /// <returns></returns>
        public virtual async Task DeleteEntityAsync<T>(List<T> entity, bool isSaveChange = true, bool IsDelete = false) where T : class, IBaseEntity, new()
        {
            IsEntityNull(entity);
            var removeList = entity.Where(u => u.IsDelete);//取出物理删除数据
            DbContext.RemoveRange(removeList);
            removeList = entity.Where(u => u.IsDelete == false);//取出逻辑删除数据
            removeList.ToList().ForEach(w => w.IsDelete = true);
            DbContext.UpdateRange(removeList);
            if (isSaveChange)
            {
                await SaveChangesAsync();
            }
        }
        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual Task<List<T>> GetAllAsync<T>(bool isNoTracking = true, bool isDataPermissions = true, bool isRecycleBin = false) where T : class, IBaseEntity, new()
        {
            return GetContext<T>(isNoTracking, isDataPermissions, isRecycleBin).ToListAsync();
        }
        /// <summary>
        /// 获取指定页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="whereLambda"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="isOrder"></param>
        /// <param name="isNoTracking"></param>
        /// <returns></returns>
        public virtual async Task<PageData<T>> GetPageAsync<T, TOrder>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TOrder>> orderBy, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true, bool isDataPermissions = true, bool isRecycleBin = false) where T : class, IBaseEntity, new()
        {
            IQueryable<T> data = GetContext<T>(isNoTracking, isDataPermissions, isRecycleBin);
            data = isOrder ?
                data.OrderBy(orderBy).OrderByDescending(u => u.CreatTime) :
                data.OrderByDescending(orderBy).OrderByDescending(u => u.CreatTime);
            data = data.Where(whereLambda);
            PageData<T> pageData = new PageData<T>
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            if (data.Count() > 0)
            {
                pageData.Total = await data.CountAsync();
                pageData.Rows = await data.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            return pageData;
        }

        /// <summary>
        /// 根据条件获取指定实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual Task<List<T>> GetEntityAsync<T>(Expression<Func<T, bool>> where, bool isNoTracking = true, bool isDataPermissions = true, bool isRecycleBin = false) where T : class, IBaseEntity, new()
        {
            return GetContext<T>(isNoTracking, isDataPermissions, isRecycleBin).Where(where).ToListAsync();
        }
        /// <summary>
        /// 根据条件获取单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual Task<T> SingleOrDefaultAsync<T>(Expression<Func<T, bool>> where, bool isNoTracking = true, bool isDataPermissions = true, bool isRecycleBin = false) where T : class, IBaseEntity, new()
        {
            return GetContext<T>(isNoTracking, isDataPermissions, isRecycleBin).Where(where).SingleOrDefaultAsync();
        }
        /// <summary>
        /// 根据条件获取首个单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> where, bool isNoTracking = true, bool isDataPermissions = true, bool isRecycleBin = false) where T : class, IBaseEntity, new()
        {
            return GetContext<T>(isNoTracking, isDataPermissions, isRecycleBin).Where(where).FirstOrDefaultAsync();
        }


        /// <summary>
        /// 保存所有更改
        /// </summary>
        /// <param name="IsFieldCheck">确保为系统内部更改时，可以取消验证</param>
        /// <returns></returns>
        public virtual async Task<int> SaveChangesAsync(bool IsFieldCheck = true)
        {
            DbContext.ChangeTracker.DetectChanges(); // Important!
            var entries = DbContext.ChangeTracker.Entries();
            foreach (var item in entries)
            {
                IBaseEntity baseEntity;
                var entity = item.Entity;
                if (entity == null) continue;
                baseEntity = entity as IBaseEntity;
                switch (item.State)
                {
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    case EntityState.Deleted:
                        break;
                    case EntityState.Modified:
                        if (!IsFieldCheck) break;
                        IsEntityNull(Interactor?.UserData?.Fields);
                        baseEntity.OperatorUp = Interactor.UserName;
                        baseEntity.UpdateTime = DateTime.Now;
                        var entityType = entity.GetType();
                        var baseType = CommonHelper.GetCavBaseType(entityType);
                        foreach (PropertyInfo sp in baseType.GetProperties())
                        {
                            switch (sp.Name.ToLower())
                            {
                                //不可更新字段
                                case "id":
                                case "uid":
                                    item.Property(sp.Name).IsModified = false;
                                    continue;
                                //系统更新字段
                                case "creattime":
                                case "updatetime":
                                case "operatorup":
                                case "isdelete":
                                    item.Property(sp.Name).IsModified = true;
                                    continue;
                                default:
                                    break;
                            }
                            var field = Interactor.UserData.Fields.FirstOrDefault(u => u.BaseFullName == baseType.Name && sp.Name == u.FieldName);
                            if (field == null)
                            {
                                item.Property(sp.Name).IsModified = false;
                            }
                        }
                        break;
                    case EntityState.Added:
                        baseEntity.CreatTime = DateTime.Now;
                        baseEntity.OperatorCare = Interactor.UserName;
                        break;
                    default:
                        break;
                }
            }
            return await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        public virtual IDbContextTransaction BeginTransaction()
        {
            var transaction = DbContext.Database.BeginTransaction();
            return transaction;
        }

        /// <summary>
        /// 获取上下文
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isNoTracking">是否跟踪上下文</param>
        /// <param name="isDataPermissions">是否启动数据权限</param>
        /// <param name="isRecycleBin">是否获取回收站数据</param>
        /// <returns></returns>
        private IQueryable<T> GetContext<T>(bool isNoTracking = true, bool isDataPermissions = true, bool isRecycleBin = false) where T : class, IBaseEntity
        {
            var set = DbContext.Set<T>();
            IQueryable<T> query;
            query = set.Where(u => u.IsDelete == isRecycleBin);
            if (isNoTracking)
            {
                query = query.AsNoTracking();
            }
            if (isDataPermissions)
            {
                //定义总的lambda树
                var lambdaTotalTree = PredicateBuilder.True<T>();
                //定义或lambda数
                var lambdaTree = PredicateBuilder.False<T>();
                lambdaTree = lambdaTree.Or(u => u.DataId == 0);
                if (Interactor.UserData.UserGroup != null)
                {
                    lambdaTree = lambdaTree.Or(u => u.DataId == Interactor.UserData.UserGroup.Id);
                    if (Interactor.UserData.SubordinateUserGroup != null && Interactor.UserData.SubordinateUserGroup.Count != 0)
                    {
                        foreach (var item in Interactor.UserData.SubordinateUserGroup)
                        {
                            lambdaTree = lambdaTree.Or(u => u.DataId == item.Id);
                        }
                    }
                }
                lambdaTotalTree.And(lambdaTree);
                query = query.Where(lambdaTotalTree);
            }
            return query;
        }
    }
}
