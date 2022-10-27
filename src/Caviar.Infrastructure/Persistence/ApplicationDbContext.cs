﻿// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Caviar.Core;
using Caviar.Core.Interface;
using Caviar.SharedKernel.Common;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Caviar.Infrastructure.Persistence
{
    public class ApplicationDbContext : IAppDbContext
    {
        public IDbContext DbContext { get; private set; }
        private readonly IInteractor _interactor;
        private readonly ILanguageService _languageService;

        public ApplicationDbContext(IDbContext identityDbContext,
            IInteractor interactor,
            ILanguageService languageService)
        {
            DbContext = identityDbContext;
            _interactor = interactor;
            _languageService = languageService;
        }


        void IsEntityNull<T>(T entity)
        {
            if (entity == null)
            {
                var errorMsg = _languageService[$"{CurrencyConstant.AppNull}.Db"];
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
        public virtual async Task<int> AddEntityAsync<T>(T entity, bool isSaveChange = true) where T : class, IUseEntity, new()
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
        public virtual async Task<bool> AddEntityAsync<T>(IEnumerable<T> entity, bool isSaveChange = true) where T : class, IUseEntity, new()
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
        public virtual async Task<T> UpdateEntityAsync<T>(T entity, bool isSaveChange = true) where T : class, IUseEntity, new()
        {
            IsEntityNull(entity);
            //var dbEntity = await SingleOrDefaultAsync<T>(u => u.Id == entity.Id,false);
            //if (dbEntity == null) throw new ArgumentException("非法操作，修改未授权数据");
            DbContext.Entry(entity).State = EntityState.Modified;
            if (isSaveChange)
            {
                await SaveChangesAsync();
            }
            return entity;
        }
        /// <summary>
        /// 修改部分实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="fieldExp"></param>
        /// <param name="isSaveChange">默认为立刻保存</param>
        /// <returns></returns>
        public virtual async Task<T> UpdateEntityAsync<T>(T entity, Expression<Func<T, object>> fieldExp, bool isSaveChange = true) where T : class, IUseEntity, new()
        {
            IsEntityNull(entity);
            //var dbEntity = await SingleOrDefaultAsync<T>(u => u.Id == entity.Id,false);
            //if (dbEntity == null) throw new ArgumentException("非法操作，修改未授权数据");
            DbContext.Entry(entity).Property(fieldExp).IsModified = true;
            if (isSaveChange)
            {
                await SaveChangesAsync();
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
        public virtual async Task<int> UpdateEntityAsync<T>(IEnumerable<T> entity, bool isSaveChange = true) where T : class, IUseEntity, new()
        {
            IsEntityNull(entity);
            //var dbEntity = GetEntityAsync<T>(u => entity.Contains(u),false);
            //if (dbEntity.Count() != entity.Count()) throw new ArgumentException("非法操作，修改未授权数据");
            DbContext.UpdateRange(entity);
            if (isSaveChange)
            {
                return await SaveChangesAsync();
            }
            return 0;
        }


        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isSaveChange">默认立刻保存</param>
        /// <param name="isDelete">是否立刻删除，默认不删除，只修改IsDelete,设为true则立刻删除</param>
        /// <returns>返回代表是否真正删除了实体，如果为true则是物理删除，如果为false则是逻辑删除</returns>
        public virtual async Task<bool> DeleteEntityAsync<T>(T entity, bool isSaveChange = true, bool isDelete = false) where T : class, IUseEntity, new()
        {
            IsEntityNull(entity);
            //var dbEntity = await SingleOrDefaultAsync<T>(u=>u.Id == entity.Id,false);
            //if (dbEntity == null) throw new ArgumentException("非法操作，删除未授权数据");
            DbContext.Entry(entity).State = EntityState.Deleted;
            if (isSaveChange)
            {
                await SaveChangesAsync();
            }
            return true;
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isSaveChange"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteEntityAsync<T>(IEnumerable<T> entity, bool isSaveChange = true, bool isDelete = false) where T : class, IUseEntity, new()
        {
            IsEntityNull(entity);
            //var dbEntity = GetEntityAsync<T>(u => entity.Contains(u),false);
            //if (dbEntity.Count() != entity.Count()) throw new ArgumentException("非法操作，删除未授权数据");
            DbContext.RemoveRange(entity);
            if (isSaveChange)
            {
                return await SaveChangesAsync();
            }
            return 0;
        }
        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual IQueryable<T> GetAllAsync<T>(bool isNoTracking = false) where T : class, IUseEntity, new()
        {
            return GetContext<T>(isNoTracking);
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
        public virtual async Task<PageData<T>> GetPageAsync<T, TOrder>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TOrder>> orderBy, int pageIndex, int pageSize, bool isOrder = true) where T : class, IUseEntity, new()
        {
            IQueryable<T> data = GetContext<T>();
            data = isOrder ?
                data.OrderBy(orderBy).OrderByDescending(u => u.CreatTime) :
                data.OrderByDescending(orderBy).OrderByDescending(u => u.CreatTime);
            if (whereLambda != null)
            {
                data = data.Where(whereLambda);
            }
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

        public virtual async Task<PageData<T>> QueryAsync<T>(QueryView query) where T : class, IUseEntity, new()
        {
            QueryCollection queries = new QueryCollection();
            foreach (var item in query.QueryModels)
            {
                queries.Add(item.Value);
            }
            IQueryable<T> data = GetContext<T>(false);
            data = data.Where(queries.AsExpression<T>());
            PageData<T> pageData = new PageData<T>
            {
                PageIndex = 1,
                PageSize = Convert.ToInt32(query.Number)
            };
            if (data.Count() > 0)
            {
                pageData.Total = pageData.PageSize;
                pageData.Rows = await data.Take(pageData.PageSize).ToListAsync();
            }
            return pageData;
        }

        /// <summary>
        /// 根据条件获取指定实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual IQueryable<T> GetEntityAsync<T>(Expression<Func<T, bool>> where, bool isNoTracking = false) where T : class, IUseEntity, new()
        {
            return GetContext<T>(isNoTracking).Where(where);
        }
        /// <summary>
        /// 根据条件获取单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual Task<T> SingleOrDefaultAsync<T>(Expression<Func<T, bool>> where, bool isNoTracking = false) where T : class, IUseEntity, new()
        {
            return GetContext<T>(isNoTracking).Where(where).SingleOrDefaultAsync();
        }
        /// <summary>
        /// 根据条件获取首个单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> where, bool isNoTracking = false) where T : class, IUseEntity, new()
        {
            return GetContext<T>(isNoTracking).Where(where).FirstOrDefaultAsync();
        }


        /// <summary>
        /// 保存所有更改
        /// </summary>
        /// <param name="IsFieldCheck">确保为系统内部更改时</param>
        /// <returns></returns>
        public virtual Task<int> SaveChangesAsync()
        {
            return DbContext.SaveChangesAsync();
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
        /// 执行sql，请注意参数的检查防止sql注入
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task<DataTable> SqlQueryAsync(string sql, params object[] parameters)
        {
            return DbContext.Database.SqlQueryAsync(sql, parameters);
        }

        /// <summary>
        /// 获取上下文
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isNoTracking">是否跟踪上下文</param>
        /// <returns></returns>
        private IQueryable<T> GetContext<T>(bool isNoTracking = false) where T : class, IUseEntity
        {
            var set = DbContext.Set<T>();
            var roleRange = GetRoleDataRange(_interactor.ApplicationRoles);
            var dataRange = GetDataRange(roleRange).Result;
            dataRange.Add(CurrencyConstant.PublicData);//公共访问数据
            IQueryable<T> query = set.Where(u => dataRange.Contains(u.DataId));
            if (isNoTracking)
            {
                query = query.AsNoTracking();
            }
            return query;
        }

        private Dictionary<DataRange, int[]> GetRoleDataRange(IList<ApplicationRole> roles)
        {
            var roleRange = new Dictionary<DataRange, int[]>();
            foreach (var item in roles)
            {
                if (item.DataRange == DataRange.Custom)
                {
                    int[] data = null;
                    data = item.DataList.Split(CurrencyConstant.CustomDataSeparator).Where(u => !string.IsNullOrEmpty(u)).Select(u => int.Parse(u)).ToArray();
                    roleRange.TryAdd(item.DataRange, null);
                    if (roleRange[item.DataRange] == null)
                    {
                        roleRange[item.DataRange] = data;
                    }
                    else
                    {
                        List<int> dataRange = new List<int>();
                        dataRange.AddRange(data);
                        dataRange.AddRange(roleRange[item.DataRange]);
                        roleRange[item.DataRange] = dataRange.ToArray();
                    }
                }
                else
                {
                    roleRange.TryAdd(item.DataRange, null);
                }
            }
            return roleRange;
        }

        private async Task<List<int>> GetDataRange(Dictionary<DataRange, int[]> dataRanges)
        {
            List<int> ranges = new List<int>();
            var set = DbContext.Set<SysUserGroup>();
            var groupId = _interactor.UserInfo.UserGroupId;
            foreach (var dataRange in dataRanges)
            {
                switch (dataRange.Key)
                {
                    case DataRange.Level:
                        ranges.Add(groupId);
                        break;
                    case DataRange.Subordinate:
                        ranges.Add(groupId);
                        var groups = set.Where(u => u.ParentId == groupId);
                        var groupIds = groups.Select(u => u.Id).ToList();
                        ranges.AddRange(groupIds);
                        break;
                    case DataRange.Custom:
                        ranges.AddRange(dataRange.Value);
                        break;
                    case DataRange.All:
                        return await set.Select(u => u.Id).ToListAsync();
                }
            }
            return ranges.ToList();
        }
    }
}
