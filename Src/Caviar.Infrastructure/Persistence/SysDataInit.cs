using Caviar.Core;
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
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Caviar.Infrastructure.API.BaseApi;
using Caviar.Core.Services.ScannerServices;

namespace Caviar.Infrastructure.Persistence
{
    /// <summary>
    /// 同步系统与数据库的数据
    /// </summary>
    public class SysDataInit
    {
        IDbContext _dbContext;
        IServiceScope _serviceScope;
        public SysDataInit(IServiceProvider provider)
        {
            _serviceScope = provider.CreateScope();
            _dbContext = _serviceScope.ServiceProvider.GetRequiredService<IDbContext>();
            
        }
        /// <summary>
        /// 系统API初始化
        /// </summary>
        /// <returns></returns>
        public async Task ActionInit()
        {
            ApiScannerServices.GetAllApi(typeof(BaseApiController));

        }

        public async Task StartInit()
        {
            var isDatabaseInit = await DatabaseInit(_dbContext);
            await ActionInit();
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
            var set = _dbContext.Set<SysFields>();
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
            _dbContext.RemoveRange(deleteFields);
            _dbContext.AddRange(addFields);
            await _dbContext.SaveChangesAsync();
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
            var userManager = _serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
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
                },
                new SysMenu()
                {
                    MenuName = "首页",
                    Icon = "home",
                    MenuType = MenuType.Menu,
                    Url = "/",
                    Number = "10"
                },
                new SysMenu()
                {
                    MenuName = "菜单管理",
                    Icon = "profile",
                    Url = "SysMenu/Index",
                    MenuType = MenuType.Menu,
                    ParentId = 1
                },
                new SysMenu()
                {
                    MenuName = "角色管理",
                    Icon = "user-switch",
                    Url = "SysRole/Index",
                    MenuType = MenuType.Menu,
                    ParentId = 1
                }
            };
            menus.Reverse();
            _dbContext.AddRange(menus);
            await _dbContext.SaveChangesAsync();
        }

    }
}
