using Caviar.Core;
using Caviar.SharedKernel.Entities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Identity;
using Caviar.SharedKernel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Caviar.Infrastructure.API.BaseApi;
using Caviar.Core.Services.ScannerServices;
using Microsoft.AspNetCore.Mvc.Routing;

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
        public async Task HttpMethodsInit()
        {
            List<Type> baseController = new List<Type>()
            {
                typeof(EasyBaseApiController<,>)
            };
            var controllerList = ApiScannerServices.GetAllController(typeof(BaseApiController), baseController);
            var methodsList = ApiScannerServices.GetAllMethods(controllerList, typeof(HttpMethodAttribute));
            var set = _dbContext.Set<SysMenu>();
            foreach (var item in methodsList)
            {
                var menu = await set.SingleOrDefaultAsync(u => u.Url == item.Url);
                if (menu == null)
                {
                    _dbContext.Add(item);
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task StartInit()
        {
            var isDatabaseInit = await DatabaseInit(_dbContext);
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
            var dataBaseFields = set.AsNoTracking().ToList();
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
            await HttpMethodsInit();
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
                    MenuName = "SysManagement",
                    Icon = "windows"
                },
                new SysMenu()
                {
                    MenuName = "Home",
                    Icon = "home",
                    MenuType = MenuType.Menu,
                    Url = "/",
                    Number = "10"
                },
                new SysMenu()
                {
                    MenuName = "index",
                    MenuType = MenuType.Menu,
                    Icon = "code",
                    Url = "CodeGeneration/Index",
                    ControllerName = "CodeGeneration"
                },
                new SysMenu()
                {
                    MenuName = "API",
                    MenuType = MenuType.API,
                    ControllerName = "API"
                }
            };
            _dbContext.AddRange(menus);
            await _dbContext.SaveChangesAsync();
            var set = _dbContext.Set<SysMenu>();
            var menuBars = set.Where(u => u.MenuName == "index");
            foreach (var item in menuBars)
            {
                item.MenuType = MenuType.Menu;
                item.MenuName = item.ControllerName;
                item.ParentId = menus.Single(u => u.MenuName == "SysManagement").Id;
            }
            await _dbContext.SaveChangesAsync();
            var subMenu = set.AsEnumerable().Where(u => u.ControllerName != null && u.ControllerName != u.MenuName).GroupBy(u => u.ControllerName);
            List<SysMenu> catalogueList = new List<SysMenu>();
            foreach (var item in subMenu)
            {
                var catalogue = set.SingleOrDefault(u => u.ControllerName == item.Key && u.MenuName == item.Key);
                var id = 0;
                if (catalogue == null)
                {
                    id = menus.Single(u => u.MenuName == "API").Id;
                }
                else
                {
                    id = catalogue.Id;
                }
                foreach (var menu_item in item)
                {
                    menu_item.ParentId = id;
                    switch (menu_item.MenuName)
                    {
                        case "CreateEntity":
                            menu_item.MenuType = MenuType.Button;
                            menu_item.Icon = "appstore-add";
                            menu_item.TargetType = TargetType.EjectPage;
                            break;
                        case "UpdateEntity":
                            menu_item.MenuType = MenuType.Button;
                            menu_item.ButtonPosition = ButtonPosition.Row;
                            menu_item.Icon = "edit";
                            menu_item.TargetType = TargetType.EjectPage;
                            break;
                        case "DeleteEntity":
                            menu_item.MenuType = MenuType.Button;
                            menu_item.ButtonPosition = ButtonPosition.Row;
                            menu_item.IsDoubleTrue = true;
                            menu_item.Icon = "delete";
                            menu_item.TargetType = TargetType.Callback;
                            break;
                        default:
                            break;
                    }
                    catalogueList.Add(menu_item);
                }
            }
            _dbContext.UpdateRange(catalogueList);
            await _dbContext.SaveChangesAsync();
        }
    }
}
