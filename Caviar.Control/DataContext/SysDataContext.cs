using Caviar.Models.SystemData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control
{
    public partial class SysDataContext:IDataContext
    {

        public SysDataContext()
        {
            if (IsDataInit)//判断数据库是否初始化
            {
                IsDataInit = DataInit().Result;
            }
        }
        DataContext _dataContext;

        private DataContext Base_DataContext
        {
            get
            {
                if (_dataContext == null)
                {
                    _dataContext = CaviarConfig.ApplicationServices.GetRequiredService<DataContext>();
                }
                return _dataContext;
            }
            set { _dataContext = value; }
        }

        public SysUserInfo SysUserInfo { get; set; }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isSaveChange">默认为立刻保存</param>
        /// <returns></returns>
        public virtual async Task<int> AddEntityAsync<T>(T entity, bool isSaveChange = true) where T : class, IBaseModel
        {
            Base_DataContext.Entry(entity).State = EntityState.Added;
            if (isSaveChange)
            {
                return await SaveChangesAsync();
            }
            return 0;
        }
        /// <summary>
        /// 添加多个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="isSaveChange">默认为立刻保存</param>
        /// <returns></returns>
        public virtual async Task<int> AddRangeAsync<T>(List<T> entities, bool isSaveChange = true) where T : class, IBaseModel
        {
            var set = Base_DataContext.Set<T>();
            await set.AddRangeAsync(entities);
            if (isSaveChange)
            {
                return await SaveChangesAsync();
            }
            return 0;
        }
        /// <summary>
        /// 保存所有更改
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> SaveChangesAsync()
        {
            Base_DataContext.ChangeTracker.DetectChanges(); // Important!
            Base_DataContext.ChangeTracker
                .Entries()
                .Where(u => u.State == EntityState.Modified)
                .Select(u => u.Entity)
                .ToList()
                .ForEach(u=> {
                    var baseEntity = u as IBaseModel;
                    if (baseEntity != null)
                    {
                        baseEntity.UpdateTime = DateTime.Now;
                        baseEntity.OperatorUp = SysUserInfo?.SysUserLogin?.UserName;
                    }
                });
            Base_DataContext.ChangeTracker
                .Entries()
                .Where(u => u.State == EntityState.Added)
                .Select(u => u.Entity)
                .ToList()
                .ForEach(u => {
                    var baseEntity = u as IBaseModel;
                    if (baseEntity != null)
                    {
                        baseEntity.CreatTime = DateTime.Now;
                        baseEntity.OperatorCare = SysUserInfo?.SysUserLogin?.UserName;
                    }
                });
            return await Base_DataContext.SaveChangesAsync();
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
            Base_DataContext.Entry(entity).State = EntityState.Modified;
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
        public virtual async Task<int> UpdateAsync<T>(T entity, Expression<Func<T, object>> fieldExp, bool isSaveChange = true) where T : class, IBaseModel
        {
            Base_DataContext.Entry(entity).Property(fieldExp).IsModified = true;
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
        /// <param name="isSaveChange"></param>
        /// <param name="IsDelete">默认立刻保存</param>
        /// <returns></returns>
        public virtual async Task<int> DeleteEntityAsync<T>(T entity, bool isSaveChange = true, bool IsDelete = false) where T : class, IBaseModel
        {
            if (entity.IsDelete)
            {
                var set = Base_DataContext.Set<T>();
                set.Remove(entity);
                if (isSaveChange)
                {
                    return await SaveChangesAsync();
                }
            }
            else
            {
                entity.IsDelete = true;
            }
            return 0;
        }
        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual IQueryable<T> GetAllAsync<T>() where T : class, IBaseModel
        {
            var set = Base_DataContext.Set<T>();
            return set.Where(u => true);
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
            var set = Base_DataContext.Set<T>();
            IQueryable<T> data = isOrder ?
                set.OrderBy(orderBy) :
                set.OrderByDescending(orderBy);

            if (whereLambda != null)
            {
                data = isNoTracking ? data.Where(whereLambda).AsNoTracking() : data.Where(whereLambda);
            }
            PageData<T> pageData = new PageData<T>
            {
                Total = await data.CountAsync(),
                Rows = await data.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync()
            };
            return pageData;
        }
        /// <summary>
        /// 获取指定实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public IQueryable<T> GetEntityAsync<T>(Expression<Func<T, bool>> where) where T : class, IBaseModel
        {
            var set = Base_DataContext.Set<T>();
            return set.Where(where);
        }
        /// <summary>
        /// 根据id获取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<T> GetEntityAsync<T>(int id) where T : class, IBaseModel
        {
            var set = Base_DataContext.Set<T>();
            return set.FirstOrDefaultAsync(u => u.Id == id);
        }
        /// <summary>
        /// 根据guid获取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Task<T> GetEntityAsync<T>(Guid uid) where T : class, IBaseModel
        {
            var set = Base_DataContext.Set<T>();
            return set.FirstOrDefaultAsync(u => u.Uid == uid);
        }

        static bool IsDataInit = true;
        /// <summary>
        /// 数据初始化
        /// </summary>
        /// <param name="allModules"></param>
        /// <param name="IsSpa"></param>
        /// <returns>返回true表示需要进行初始化数据操作，返回false即数据库已经存在或不需要初始化数据</returns>
        public async Task<bool> DataInit()
        {
            bool IsExistence = await Base_DataContext.Database.EnsureCreatedAsync();
            if (IsExistence)
            {
                //创建初始角色
                SysUserLogin Login = new SysUserLogin()
                {
                    UserName = "admin",
                    Password = CommonHelper.GetMD5("123456"),
                    PhoneNumber = "11111111111",
                };
                await AddEntityAsync(Login);
                //创建基础角色
                var NoLoginRole = new SysRole
                {
                    RoleName = CaviarConfig.NoLoginRole,
                };
                await AddEntityAsync(NoLoginRole);
                var role = new SysRole()
                {
                    RoleName = CaviarConfig.SysAdminRole,
                };
                await AddEntityAsync(role);
                //默认角色加入管理员角色
                SysRoleLogin sysRoleLogin = new SysRoleLogin()
                {
                    RoleId = role.Id,
                    UserId = Login.Id
                };
                await AddEntityAsync(sysRoleLogin);
                //创建基础访问页面
                SysPowerMenu homePage = new SysPowerMenu()
                {
                    MenuType = MenuType.Page,
                    TargetType = TargetType.CurrentPage,
                    MenuName = "首页",
                    Url = "/"
                };
                await AddEntityAsync(homePage);
                SysRoleMenu homePageRole = new SysRoleMenu()
                {
                    MenuId = homePage.Id,
                    RoleId = role.Id,
                };
                await AddEntityAsync(homePageRole);
                //创建基础菜单
                SysPowerMenu sysPowerMenu = new SysPowerMenu()
                {
                    MenuType = MenuType.Catalog,
                    TargetType = TargetType.CurrentPage,
                    MenuName = "系统管理",
                    
                };
                await AddEntityAsync(sysPowerMenu);
                //基础菜单加入管理员角色
                SysRoleMenu sysRoleMenu = new SysRoleMenu()
                {
                    MenuId = sysPowerMenu.Id,
                    RoleId = role.Id,
                };
                await AddEntityAsync(sysRoleMenu);
            }
            return IsExistence;
        }
    }
}
