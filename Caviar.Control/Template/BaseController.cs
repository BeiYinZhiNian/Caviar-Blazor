using Caviar.Models.SystemData;
using Caviar.Models.SystemData.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
        protected virtual async Task<int> AddEntity<T>(T entity, bool isSaveChange = true) where T : class, IBaseModel
        {
            entity.OperatorCare = Sys_User_Info.Sys_User_Login.UserName;
            Base_DataContext.Entry(entity).State = EntityState.Added;
            if (isSaveChange)
            {
                return await Base_DataContext.SaveChangesAsync();
            }
            return 0;
        }

        protected virtual async Task<int> UpdateEntity<T>(T entity, bool isSaveChange = true) where T : class, IBaseModel
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

        protected virtual async Task<int> DeleteEntity<T>(T entity, bool isSaveChange = true, bool IsDelete = false) where T : class, IBaseModel
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

        protected Task<T> GetEntity<T>(int id) where T : class, IBaseModel
        {
            var set = Base_DataContext.Set<T>();
            return set.FirstOrDefaultAsync(u => u.Id == id);
        }

        protected Task<T> GetEntity<T>(Guid uid) where T : class, IBaseModel
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
                Sys_User_Login Login = new Sys_User_Login()
                {
                    UserName = "admin",
                    Password = CommonHelper.GetMD5("123456"),
                    PhoneNumber = "11111111111",
                };
                await AddEntity(Login);
                Sys_Role role = new Sys_Role()
                {
                    RoleName = "管理员",
                };
                await AddEntity(role);
                Sys_Role_Login sys_Role_Login = new Sys_Role_Login()
                {
                    RoleId = role.Id,
                    UserId = Login.Id
                };
                await AddEntity(sys_Role_Login);
            }
            return IsExistence;
        }
    }
}
