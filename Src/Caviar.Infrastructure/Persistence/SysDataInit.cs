using Caviar.Core;
using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Identity;
using Caviar.SharedKernel;
using Caviar.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Caviar.Infrastructure.Persistence
{
    /// <summary>
    /// 同步系统与数据库的数据
    /// </summary>
    public class SysDataInit
    {
        public IDbContext DbContext;
        public IServiceScope ServiceScope;
        public SysDataInit(IServiceProvider provider)
        {
            ServiceScope = provider.CreateScope();
            DbContext = ServiceScope.ServiceProvider.GetRequiredService<IDbContext>();
        }

        public async Task StartInit()
        {
            var isDatabaseInit = await DatabaseInit(DbContext);
            await FieldsInit();
            await CreateData(isDatabaseInit);
        }
        /// <summary>
        /// 初始化系统字段
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        protected virtual async Task FieldsInit()
        {
            var fields = FieldScannerServices.GetApplicationFields().Select(u => u.Entity);
            var set = DbContext.Set<SysFields>();
            var dataBaseFields = await set.AsNoTracking().Where(u=>true).ToListAsync();
            foreach (var sysField in fields)
            {
                foreach (var dataBaseField in dataBaseFields)
                {
                    if (dataBaseField.FieldName == sysField.FieldName && dataBaseField.FullName == sysField.FullName && dataBaseField.BaseFullName == sysField.BaseFullName)
                    {
                        sysField.Id = dataBaseField.Id;
                        dataBaseField.Id = 0;
                    }
                }
            }
            var addFields = fields.Where(u => u.Id == 0).ToList();
            var deleteFields = dataBaseFields.Where(u => u.Id != 0).ToList();
            DbContext.RemoveRange(deleteFields);
            DbContext.AddRange(addFields);
            await DbContext.SaveChangesAsync();
        }
        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        protected virtual Task<bool> DatabaseInit(IDbContext dbContext)
        {
            return dbContext.Database.EnsureCreatedAsync();
        }

        protected virtual async Task CreateData(bool isDatabaseInit)
        {
            if (!isDatabaseInit) return;
            await CreatAdminUser();
            await CreateMenu();

        }

        protected virtual async Task CreatAdminUser()
        {
            var userManager = ServiceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = new ApplicationUser { Email = "1031622947@qq.com", UserName = "admin" };
            var result = await userManager.CreateAsync(user, "1031622947@qq.COM");
            if (!result.Succeeded) throw new Exception("创建用户失败，数据初始化停止");
        }

        protected virtual async Task CreateMenu()
        {
            List<SysMenu> menus = new List<SysMenu>()
            {
                new SysMenu()
                {
                    MenuName = "系统管理",
                    Icon = "windows"
                }
            };
            DbContext.AddRange(menus);
            await DbContext.SaveChangesAsync();
        }

    }
}
