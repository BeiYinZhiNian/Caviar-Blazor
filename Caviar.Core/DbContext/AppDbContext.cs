using Caviar.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Caviar.Core
{
    public partial class AppDbContext : IAppDbContext
    {

        public AppDbContext(SysDbContext dataContext,IInteractor Interactor, ICodeGeneration cavAssembly)
        {
            _SysDbContext = dataContext;
            _Interactor = Interactor;
            _cavAssembly = cavAssembly;
            if (IsDataInit)//判断数据库是否初始化
            {
                try
                {
                    IsDataInit = DataInit().Result;
                }
                catch(Exception e)
                {
                    throw new Exception("数据库初始化失败，请检查更新字段," + e.Message);
                }
            }
        }
        SysDbContext _SysDbContext;
        ICodeGeneration _cavAssembly;
        private SysDbContext DC => _SysDbContext;

        IInteractor _Interactor;
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isSaveChange">默认为立刻保存</param>
        /// <returns></returns>
        public virtual async Task<int> AddEntityAsync<T>(T entity, bool isSaveChange = true) where T : class, IBaseModel
        {
            DC.Entry(entity).State = EntityState.Added;
            if (isSaveChange)
            {
                return await SaveChangesAsync();
            }
            return 0;
        }
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isSaveChange"></param>
        /// <returns></returns>
        public virtual async Task<int> AddEntityAsync<T>(List<T> entity, bool isSaveChange = true) where T : class, IBaseModel
        {
            var count = 0;
            if (entity == null || entity.Count == 0) return count;
            using (var transaction = BeginTransaction())
            {
                foreach (var item in entity)
                {
                    await AddEntityAsync(item, false);
                }
                if (isSaveChange)
                {
                    count = await SaveChangesAsync();
                }
                transaction.Commit();
            }
            return count;
        }
        /// <summary>
        /// 保存所有更改
        /// </summary>
        /// <param name="IsFieldCheck">确保为系统内部更改时，可以取消验证</param>
        /// <returns></returns>
        public virtual async Task<int> SaveChangesAsync(bool IsFieldCheck = true)
        {
            DC.ChangeTracker.DetectChanges(); // Important!
            var entries = DC.ChangeTracker.Entries();
            foreach (var item in entries)
            {
                IBaseModel baseEntity;
                var entity = item.Entity;
                if (entity == null) continue;
                baseEntity = entity as IBaseModel;
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
                        baseEntity.OperatorUp = _Interactor.UserName;
                        baseEntity.UpdateTime = DateTime.Now;
                        var entityType = entity.GetType();
                        var baseType = CommonlyHelper.GetCavBaseType(entityType);
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
                            var field = _Interactor.UserData.ModelFields.FirstOrDefault(u => u.BaseTypeName == baseType.Name && sp.Name == u.TypeName);
                            if (field == null)
                            {
                                item.Property(sp.Name).IsModified = false;
                            }
                        }
                        break;
                    case EntityState.Added:
                        baseEntity.CreatTime = DateTime.Now;
                        baseEntity.OperatorCare = _Interactor.UserName;
                        break;
                    default:
                        break;
                }
            }
            return await DC.SaveChangesAsync();
        }
        /// <summary>
        /// 修改指定实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isSaveChange">默认为立刻保存</param>
        /// <returns></returns>
        public virtual async Task<int> UpdateEntityAsync<T>(T entity, bool isSaveChange = true) where T : class, IBaseModel
        {
            DC.Entry(entity).State = EntityState.Modified;
            if (isSaveChange)
            {
                return await SaveChangesAsync();
            }
            return 0;
        }
        /// <summary>
        /// 修改部分实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="fieldExp"></param>
        /// <param name="isSaveChange">默认为立刻保存</param>
        /// <returns></returns>
        public virtual async Task<int> UpdateEntityAsync<T>(T entity, Expression<Func<T, object>> fieldExp, bool isSaveChange = true) where T : class, IBaseModel
        {
            DC.Entry(entity).Property(fieldExp).IsModified = true;
            if (isSaveChange)
            {
                return await SaveChangesAsync();
            }
            return 0;
        }
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="fieldExp"></param>
        /// <param name="isSaveChange"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateEntityAsync<T>(List<T> entity, bool isSaveChange = true) where T : class, IBaseModel
        {
            var count = 0;
            if (entity == null || entity.Count == 0) return count;
            using(var transaction = BeginTransaction())
            {
                foreach (var item in entity)
                {
                    await UpdateEntityAsync(item, false);
                }

                if (isSaveChange)
                {
                    count = await SaveChangesAsync();
                }
                transaction.Commit();
            }
            return count;
        }


        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isSaveChange">默认立刻保存</param>
        /// <param name="IsDelete">是否立刻删除，默认不删除，只修改IsDelete,设为true则立刻删除</param>
        /// <returns></returns>
        public virtual async Task<int> DeleteEntityAsync<T>(T entity, bool isSaveChange = true, bool IsDelete = false) where T : class, IBaseModel
        {
            if (entity == null) return 0;
            if (entity.IsDelete || IsDelete)
            {
                DC.Entry(entity).State = EntityState.Deleted;
                if (isSaveChange)
                {
                    return await SaveChangesAsync();
                }
            }
            else
            {
                entity.IsDelete = true;
                return await UpdateEntityAsync(entity,isSaveChange);
            }
            return 0;
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isSaveChange"></param>
        /// <param name="IsDelete"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteEntityAsync<T>(List<T> entity, bool isSaveChange = true, bool IsDelete = false) where T : class, IBaseModel
        {
            var count = 0;
            if (entity == null || entity.Count == 0) return count;
            using (var transaction = BeginTransaction())
            {
                foreach (var item in entity)
                {
                    await DeleteEntityAsync(item, false, IsDelete);
                }
                if (isSaveChange)
                {
                    count = await SaveChangesAsync();
                }
                transaction.Commit();
            }
            return count;
        }
        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual Task<List<T>> GetAllAsync<T>(bool isNoTracking = true) where T : class, IBaseModel
        {
            return GetContext<T>(isNoTracking).ToListAsync();
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
        public virtual async Task<PageData<T>> GetPageAsync<T, TKey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true) where T : class, IBaseModel
        {
            IQueryable<T> data = GetContext<T>(isNoTracking);
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
        public virtual Task<List<T>> GetEntityAsync<T>(Expression<Func<T, bool>> where, bool isNoTracking = true) where T : class, IBaseModel
        {
            return GetContext<T>(isNoTracking).Where(where).ToListAsync();
        }
        /// <summary>
        /// 根据条件获取单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual Task<T> GetSingleEntityAsync<T>(Expression<Func<T, bool>> where, bool isNoTracking = true) where T : class, IBaseModel
        {
            return GetContext<T>(isNoTracking).Where(where).SingleOrDefaultAsync();
        }
        /// <summary>
        /// 根据条件获取首个单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual Task<T> GetFirstEntityAsync<T>(Expression<Func<T, bool>> where, bool isNoTracking = true) where T : class, IBaseModel
        {
            return GetContext<T>(isNoTracking).Where(where).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 根据id获取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Task<T> GetEntityAsync<T>(int id, bool isNoTracking = true) where T : class, IBaseModel
        {
            return GetContext<T>(isNoTracking).SingleOrDefaultAsync(u => u.Id == id);
        }
        /// <summary>
        /// 根据guid获取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uid"></param>
        /// <returns></returns>
        public virtual Task<T> GetEntityAsync<T>(Guid uid, bool isNoTracking = true) where T : class, IBaseModel
        {
            return GetContext<T>(isNoTracking).SingleOrDefaultAsync(u => u.Uid == uid);
        }

        /// <summary>
        /// 获取上下文
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isNoTracking">是否跟踪上下文</param>
        /// <param name="isDataPermissions">是否启动数据权限</param>
        /// <param name="isRecycleBin">是否获取回收站数据</param>
        /// <returns></returns>
        private IQueryable<T> GetContext<T>(bool isNoTracking = true,bool isDataPermissions = true,bool isRecycleBin = false) where T : class, IBaseModel
        {
            var set = DC.Set<T>();
            IQueryable<T> query;
            query = set.Where(u => u.IsDelete == isRecycleBin);
            if (isNoTracking)
            {
                query = query.AsNoTracking();
            }
            return query;
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        public virtual IDbContextTransaction BeginTransaction()
        {
            var transaction = DC.Database.BeginTransaction();
            return transaction;
        }
        /// <summary>
        /// 执行sql，请注意参数的检查防止sql注入
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable SqlQuery(string sql, params object[] parameters)
        {
            return DC.Database.SqlQuery(sql, parameters);
        }

        static bool IsDataInit = true;
        /// <summary>
        /// 数据初始化
        /// </summary>
        /// <param name="allModules"></param>
        /// <param name="IsSpa"></param>
        /// <returns>返回true表示需要进行初始化数据操作，返回false即数据库已经存在或不需要初始化数据</returns>
        public virtual async Task<bool> DataInit()
        {
            
            bool IsExistence = await DC.Database.EnsureCreatedAsync();
            #region 使用sql初始化数据库
            if (IsExistence)
            {
                var sql = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}/{CaviarConfig.SqlConfig.SqlFilePath}");
                string pattern = @"USE \[.*?\]";
                Match m = Regex.Match(sql, pattern, RegexOptions.IgnoreCase);
                sql = sql.Replace(m.Value, "");
                sql = sql.Replace("GO", "");
                SqlQuery(sql);
            }
            //同步系统与数据库的模型字段
            var fields = await GetAllAsync<SysModelFields>();
            var types = CommonlyHelper.GetModelList(true);
            List<SysModelFields> modelFields = new List<SysModelFields>();
            foreach (var item in types)
            {
                var viewModelFields = _cavAssembly.GetViewModelHeaders(item.Name);
                modelFields.AddRange(viewModelFields);
            }
            foreach (var modelFieldsItem in modelFields)
            {
                foreach (var fieldsItem in fields)
                {
                    if (modelFieldsItem.TypeName == fieldsItem.TypeName && modelFieldsItem.FullName == fieldsItem.FullName && modelFieldsItem.BaseTypeName==fieldsItem.BaseTypeName)
                    {
                        modelFieldsItem.Id = fieldsItem.Id;
                        fieldsItem.Id = 0;
                    }
                }
            }
            var addFields = modelFields.Where(u => u.Id == 0).ToList();
            var deleteFields = fields.Where(u => u.Id != 0).ToList();
            await DeleteEntityAsync(deleteFields,IsDelete:true);
            await AddEntityAsync(addFields);
            #endregion
            return IsExistence;
        }

        public void DetachAll()
        {
            DC.DetachAll();
        }

    }
}
