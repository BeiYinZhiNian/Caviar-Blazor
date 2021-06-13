using Caviar.Models;
using Caviar.Models.SystemData;
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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Caviar.Control
{
    public partial class SysDataContext : IDataContext
    {

        public SysDataContext(DataContext dataContext,BaseControllerModel baseControllerModel, IAssemblyDynamicCreation cavAssembly)
        {
            _dataContext = dataContext;
            _baseControllerModel = baseControllerModel;
            _cavAssembly = cavAssembly;
            if (IsDataInit)//判断数据库是否初始化
            {
                IsDataInit = DataInit().Result;
            }
        }
        DataContext _dataContext;
        IAssemblyDynamicCreation _cavAssembly;
        private DataContext DC => _dataContext;

        IBaseControllerModel _baseControllerModel;
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
        /// <returns></returns>
        public virtual async Task<int> SaveChangesAsync()
        {
            DC.ChangeTracker.DetectChanges(); // Important!
            DC.ChangeTracker
                .Entries()
                .Where(u => u.State == EntityState.Modified)
                .Select(u => u.Entity)
                .ToList()
                .ForEach(u =>
                {
                    var baseEntity = u as IBaseModel;
                    if (baseEntity != null)
                    {
                        baseEntity.UpdateTime = DateTime.Now;
                        baseEntity.OperatorUp = _baseControllerModel.UserName;
                    }
                });
            DC.ChangeTracker
                .Entries()
                .Where(u => u.State == EntityState.Added)
                .Select(u => u.Entity)
                .ToList()
                .ForEach(u =>
                {
                    var baseEntity = u as IBaseModel;
                    if (baseEntity != null)
                    {
                        baseEntity.CreatTime = DateTime.Now;
                        baseEntity.OperatorCare = _baseControllerModel.UserName;
                    }
                });
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
        public virtual Task<List<T>> GetAllAsync<T>() where T : class, IBaseModel
        {
            return GetContext<T>().ToListAsync();
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
        public virtual async Task<PageData<T>> GetPageAsync<T, TKey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = false) where T : class, IBaseModel
        {
            IQueryable<T> data = GetContext<T>();
            data = isOrder ?
                data.OrderBy(orderBy) :
                data.OrderByDescending(orderBy);
            if (whereLambda != null)
            {
                data = isNoTracking ? data.Where(whereLambda).AsNoTracking() : data.Where(whereLambda);
            }
            PageData<T> pageData = new PageData<T>
            {
                Rows = await data.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            return pageData;
        }
        /// <summary>
        /// 根据条件获取指定实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual Task<List<T>> GetEntityAsync<T>(Expression<Func<T, bool>> where) where T : class, IBaseModel
        {
            return GetContext<T>().Where(where).ToListAsync();
        }
        /// <summary>
        /// 根据条件获取单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual Task<T> GetFirstEntityAsync<T>(Expression<Func<T, bool>> where) where T : class, IBaseModel
        {
            return GetContext<T>().Where(where).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 根据id获取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Task<T> GetEntityAsync<T>(int id) where T : class, IBaseModel
        {
            return GetContext<T>().FirstOrDefaultAsync(u => u.Id == id);
        }
        /// <summary>
        /// 根据guid获取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uid"></param>
        /// <returns></returns>
        public virtual Task<T> GetEntityAsync<T>(Guid uid) where T : class, IBaseModel
        {
            return GetContext<T>().FirstOrDefaultAsync(u => u.Uid == uid);
        }

        private IQueryable<T> GetContext<T>() where T : class, IBaseModel
        {
            var set = DC.Set<T>();
            return set.Where(u => u.IsDelete == false);
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

                //同步系统与数据库的模型字段
                var fields = await GetAllAsync<SysModelFields>();
                var types = CommonHelper.GetModelList();
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
                        if(modelFieldsItem.TypeName == fieldsItem.TypeName && modelFieldsItem.FullName == fieldsItem.FullName)
                        {
                            modelFieldsItem.Id = fieldsItem.Id;
                            fieldsItem.Id = 0;
                        }
                    }
                }
                var addFields = modelFields.Where(u => u.Id == 0).ToList();
                var deleteFields = fields.Where(u => u.Id != 0).ToList();
                await DeleteEntityAsync(deleteFields);
                await AddEntityAsync(addFields);
            }
            #endregion
            return IsExistence;
        }

        public void DetachAll()
        {
            DC.DetachAll();
        }
        public async Task<int> CreateButton(string menuName,string outName,int parentId,bool isTree = false)
        {
            SysMenu menu = new SysMenu()
            {
                MenuName = menuName + "管理",
                TargetType = TargetType.CurrentPage,
                MenuType = MenuType.Menu,
                Url = $"{outName}/Index",
                Icon = "border-outer",
                ParentId = parentId,
                Number = "999"
            };
            menu = await AddMenu(menu);
            parentId = menu.Id;
            SysMenu AddButton = new SysMenu()
            {
                MenuType = MenuType.Button,
                TargetType = TargetType.EjectPage,
                MenuName = "新增",
                ButtonPosition = ButtonPosition.Header,
                Url = $"{outName}/Add",
                Icon = "appstore-add",
                ParentId = parentId,
                IsDoubleTrue = false,
                Number = "999"
            };
            await AddMenu(AddButton);
            AddButton = new SysMenu()
            {
                MenuType = MenuType.Button,
                Url = $"{outName}/Update",
                MenuName = "修改",
                TargetType = TargetType.EjectPage,
                ButtonPosition = ButtonPosition.Row,
                Icon = "edit",
                ParentId = parentId,
                Number = "999"
            };
            await AddMenu(AddButton);
            AddButton = new SysMenu()
            {
                MenuType = MenuType.Button,
                MenuName = "删除",
                ButtonPosition = ButtonPosition.Row,
                Url = $"{outName}/Delete",
                TargetType = TargetType.Callback,
                Icon = "delete",
                ParentId = parentId,
                IsDoubleTrue = true,
                Number = "9999"
            };
            if (isTree)
            {
                AddButton.Url = $"{outName}/Move";
            }
            await AddMenu(AddButton);
            return parentId;
        }


        private async Task<SysMenu> AddMenu(SysMenu menu)
        {
            SysMenu entity = null;
            if (menu.MenuType != MenuType.Button)
            {
                entity = await GetFirstEntityAsync<SysMenu>(u => u.MenuName == menu.MenuName);
            }
            else
            {
                entity = await GetFirstEntityAsync<SysMenu>(u => u.MenuName == menu.MenuName && u.ParentId == menu.ParentId);
            }
            var count = 0;
            if (entity == null)
            {
                count = await AddEntityAsync(menu);
                entity = menu;
            }
            return entity;
        }

    }
}
