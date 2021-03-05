using Caviar.Models.SystemData;
using Caviar.Models.SystemData.Template;
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
    public partial class BaseController
    {
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
        protected virtual async Task<int> AddEntityAsync<T>(T entity, bool isSaveChange = true) where T : class, IBaseModel
        {
            entity.OperatorCare = Sys_User_Info.Sys_User_Login.UserName;
            Base_DataContext.Entry(entity).State = EntityState.Added;
            if (isSaveChange)
            {
                return await Base_DataContext.SaveChangesAsync();
            }
            return 0;
        }

        protected virtual async Task<int> AddRangeAsync<T>(List<T> entities, bool isSaveChange = true) where T : class, IBaseModel
        {
            var set = Base_DataContext.Set<T>();
            await set.AddRangeAsync(entities);
            if (isSaveChange)
            {
                return await Base_DataContext.SaveChangesAsync();
            }
            return 0;
        }

        protected virtual async Task<int> SaveChangesAsync()
        {
            return await Base_DataContext.SaveChangesAsync();
        }

        protected virtual async Task<int> UpdateEntityAsync<T>(T entity, bool isSaveChange = true) where T : class, IBaseModel
        {
            entity.OperatorUp = Sys_User_Info.Sys_User_Login.UserName;
            Base_DataContext.Entry(entity).State = EntityState.Modified;
            if (isSaveChange)
            {
                return await Base_DataContext.SaveChangesAsync();
            }
            return 0;
        }

        protected virtual async Task<int> UpdateAsync<T>(T entity, Expression<Func<T, object>> fieldExp, bool isSaveChange = true) where T : class, IBaseModel
        {
            entity.OperatorUp = Sys_User_Info.Sys_User_Login.UserName;
            Base_DataContext.Entry(entity).Property(fieldExp).IsModified = true;
            if (isSaveChange)
            {
                return await Base_DataContext.SaveChangesAsync();
            }
            return 0;
        }

        protected virtual async Task<int> DeleteEntityAsync<T>(T entity, bool isSaveChange = true, bool IsDelete = false) where T : class, IBaseModel
        {
            if (entity.IsDelete)
            {
                var set = Base_DataContext.Set<T>();
                set.Remove(entity);
                if (isSaveChange)
                {
                    return await Base_DataContext.SaveChangesAsync();
                }
            }
            else
            {
                entity.IsDelete = true;
            }
            return 0;
        }

        protected virtual IQueryable<T> GetAllAsync<T>() where T : class, IBaseModel
        {
            var set = Base_DataContext.Set<T>();
            return set.Where(u => true);
        }

        protected virtual async Task<PageData<T>> GetPageAsync<T, TKey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true) where T : class, IBaseModel
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

        protected IQueryable<T> GetEntity<T>(Expression<Func<T, bool>> where) where T : class, IBaseModel
        {
            var set = Base_DataContext.Set<T>();
            return set.Where(where);
        }

        protected Task<T> GetEntityAsync<T>(int id) where T : class, IBaseModel
        {
            var set = Base_DataContext.Set<T>();
            return set.FirstOrDefaultAsync(u => u.Id == id);
        }

        protected Task<T> GetEntityAsync<T>(Guid uid) where T : class, IBaseModel
        {
            var set = Base_DataContext.Set<T>();
            return set.FirstOrDefaultAsync(u => u.Uid == uid);
        }


        /// <summary>
        /// 数据初始化
        /// </summary>
        /// <param name="allModules"></param>
        /// <param name="IsSpa"></param>
        /// <returns>返回true表示需要进行初始化数据操作，返回false即数据库已经存在或不需要初始化数据</returns>
        protected async Task<bool> DataInit()
        {
            bool IsExistence = await Base_DataContext.Database.EnsureCreatedAsync();
            if (IsExistence)
            {
                //创建初始角色
                Sys_User_Login Login = new Sys_User_Login()
                {
                    UserName = "admin",
                    Password = CommonHelper.GetMD5("123456"),
                    PhoneNumber = "11111111111",
                };
                await AddEntityAsync(Login);
                //创建基础角色
                var roleList = new List<Sys_Role>();
                roleList.Add(new Sys_Role
                {
                    RoleName = CaviarConfig.NoLoginRole,
                });
                
                await AddRangeAsync(roleList);
                var role = new Sys_Role()
                {
                    RoleName = CaviarConfig.SysAdminRole,
                };
                await AddEntityAsync(role);
                //默认角色加入管理员角色
                Sys_Role_Login sys_Role_Login = new Sys_Role_Login()
                {
                    RoleId = role.Id,
                    UserId = Login.Id
                };
                await AddEntityAsync(sys_Role_Login);
                //创建基础菜单
                Sys_Power_Menu sys_Power_Menu = new Sys_Power_Menu()
                {
                    MenuType = MenuType.Catalog,
                    TargetType = TargetType.CurrentPage,
                    MenuName = "系统管理",
                };
                await AddEntityAsync(sys_Role_Login);
                //基础菜单加入管理员角色
                Sys_Role_Menu sys_Role_Menu = new Sys_Role_Menu()
                {
                    MenuId = sys_Power_Menu.Id,
                    RoleId = role.Id,
                };
            }
            return IsExistence;
        }
    }
}
