using Caviar.Models.SystemData;
using Caviar.Models.SystemData.Template;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Caviar.Control
{
    public class DataContext : EmptyContext
    {
        #region 表对象
        public virtual DbSet<Sys_Role_Login> Sys_Role_Login { get; set; }
        public virtual DbSet<Sys_User_Login> Sys_User_Login { get; set; }
        public virtual DbSet<Sys_Role> Sys_Role { get; set; }

        #endregion

        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {
            var IsDataInit = DataInit().Result;
        }

        public DataContext()
        {
            var IsDataInit = DataInit().Result;
        }
        /// <summary>
        /// 数据初始化
        /// </summary>
        /// <param name="allModules"></param>
        /// <param name="IsSpa"></param>
        /// <returns>返回true表示需要进行初始化数据操作，返回false即数据库已经存在或不需要初始化数据</returns>
        public async Task<bool> DataInit()
        {
            bool IsExistence = await Database.EnsureCreatedAsync();
            if (IsExistence)
            {
                Sys_User_Login Login = new Sys_User_Login()
                {
                    UserName = "admin",
                    Password = CommonHelper.GetMD5("123456"),
                    PhoneNumber = "11111111111",
                    OperatorCare = "系统",
                };
                await AddEntity(Login);
                Sys_Role role = new Sys_Role()
                {
                    RoleName = "管理员",
                    OperatorCare = "系统",
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = CaviarConfig.SqlConfig.Connections;
                switch (CaviarConfig.SqlConfig.DBTypeEnum)
                {
                    case DBTypeEnum.SqlServer:
                        optionsBuilder.UseSqlServer(connectionString);
                        break;
                    case DBTypeEnum.MySql:
                        optionsBuilder.UseMySql(connectionString, null, null);
                        break;
                    case DBTypeEnum.PgSql:
                        optionsBuilder.UseNpgsql(connectionString);
                        break;
                    case DBTypeEnum.Memory:
                        optionsBuilder.UseInMemoryDatabase(connectionString);
                        break;
                    case DBTypeEnum.SQLite:
                        optionsBuilder.UseSqlite(connectionString);
                        break;
                    case DBTypeEnum.Oracle:
                        optionsBuilder.UseOracle(connectionString);
                        break;
                    default:
                        break;
                }
            }
        }

    }


    public partial class EmptyContext : DbContext,IDataContext
    {
        #region 构造方法
        public EmptyContext(DbContextOptions<DataContext> options) : base(options) { }
        public EmptyContext() : base(){ } //非注入构造方式
        
        #endregion


        public async Task<int> AddEntity<T>(T entity, bool isSaveChange = true) where T : class,IBaseModel
        {
            this.Entry(entity).State = EntityState.Added;
            if (isSaveChange)
            {
                return await SaveChangesAsync();
            }
            return 0;
        }

        public async Task<int> UpdateEntity<T>(T entity, bool isSaveChange = true) where T : class,IBaseModel
        {
            this.Entry(entity).State = EntityState.Modified;
            if (isSaveChange)
            {
                return await SaveChangesAsync();
            }
            return 0;
        }

        public async Task<int> UpdateAsync<T>(T entity, Expression<Func<T, object>> fieldExp, bool isSaveChange = true) where T : class,IBaseModel
        {
            this.Entry(entity).Property(fieldExp).IsModified = true;
            if (isSaveChange)
            {
                return await SaveChangesAsync();
            }
            return 0;
        }

        public async Task<int> DeleteEntity<T>(T entity, bool isSaveChange = true, bool IsDelete = false) where T : class,IBaseModel
        {
            var set = this.Set<T>();
            set.Remove(entity);
            if (isSaveChange)
            {
                return await SaveChangesAsync();
            }
            return 0;
        }

        public IQueryable<T> GetAllAsync<T>() where T : class,IBaseModel
        {
            var set = this.Set<T>();
            return set.Where(u => true);
        }

        public async Task<PageData<T>> GetPageAsync<T, TKey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true) where T : class,IBaseModel
        {
            var set = this.Set<T>();
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

        public IQueryable<T> GetEntity<T>(Expression<Func<T, bool>> where) where T : class, IBaseModel
        {
            var set = this.Set<T>();
            return set.Where(where);
        }

        public Task<T> GetEntity<T>(int id) where T : class, IBaseModel
        {
            var set = this.Set<T>();
            return set.FirstOrDefaultAsync(u => u.Id == id);
        }

        public Task<T> GetEntity<T>(Guid uid) where T : class, IBaseModel
        {
            var set = this.Set<T>();
            return set.FirstOrDefaultAsync(u => u.Uid == uid);
        }
    }

}
