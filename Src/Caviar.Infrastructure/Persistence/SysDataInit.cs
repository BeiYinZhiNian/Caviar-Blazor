using Caviar.SharedKernel.Entities.View;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Caviar.Infrastructure.API.BaseApi;
using Microsoft.AspNetCore.Mvc.Routing;
using Caviar.SharedKernel.Entities;
using Caviar.Core.Interface;
using Caviar.Core.Services;

namespace Caviar.Infrastructure.Persistence
{
    /// <summary>
    /// 同步系统与数据库的数据
    /// </summary>
    public class SysDataInit
    {
        IDbContext _dbContext;
        IServiceScope _serviceScope;
        ILanguageService _languageService;
        public SysDataInit(IServiceProvider provider)
        {
            _serviceScope = provider.CreateScope();
            _dbContext = _serviceScope.ServiceProvider.GetRequiredService<IDbContext>();
            _languageService = _serviceScope.ServiceProvider.GetRequiredService<ILanguageService>();


        }
        /// <summary>
        /// 系统API初始化
        /// </summary>
        /// <returns></returns>
        public async Task HttpMethodsInit()
        {
            //需要忽略的控制器
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
            var fields = FieldScannerServices.GetApplicationFields(_languageService).Select(u => u.Entity);
            await CreateData(isDatabaseInit);
            await FieldsInit(fields);
            await PermissionFields(fields, isDatabaseInit);
            
            
        }
        /// <summary>
        /// 初始化系统字段
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        protected virtual async Task FieldsInit(IEnumerable<SysFields> fields)
        {
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

        protected virtual async Task PermissionFields(IEnumerable<SysFields> fields,bool isDatabaseInit)
        {
            if (!isDatabaseInit) return;
            foreach (var item in fields)
            {
                _dbContext.Add(new SysPermission()
                {
                    EntityName = CurrencyConstant.Admin,
                    PermissionId = item.Id,
                    PermissionType = PermissionType.RoleFields
                });
            }
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
            await HttpMethodsInit();
            if (!isDatabaseInit) return;
            await CreateInitRole();
            await CreateInitUser();
            await CreateMenu();
        }

        protected virtual async Task CreateInitUser()
        {
            var userManager = _serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = new ApplicationUser { Email = "1031622947@qq.com", UserName = "admin"};
            var result = await userManager.CreateAsync(user, "1031622947@qq.COM");
            if (!result.Succeeded) throw new Exception("创建用户失败，数据初始化停止");
            await userManager.AddToRoleAsync(user, "admin");
        }

        protected virtual async Task CreateInitRole()
        {
            var roleManager = _serviceScope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var role = new ApplicationRole { Name = CurrencyConstant.Admin };
            var result = await roleManager.CreateAsync(role);
            if (!result.Succeeded) throw new Exception("创建用户失败，数据初始化停止");
        }

        protected virtual async Task CreateMenu()
        {
            List<SysMenuView> menus = new List<SysMenuView>()
            {
                new SysMenuView()
                {
                    Entity = new SysMenu()
                    {
                        Key = CurrencyConstant.SysManagementKey,
                        Icon = "windows"
                    }
                    
                },
                new SysMenuView()
                {
                    Entity = new SysMenu()
                    {
                        Key = "Home",
                        Icon = "home",
                        MenuType = MenuType.Menu,
                        Url = "/",
                        Number = "10"
                    }
                    
                },
                new SysMenuView()
                {
                    Entity = new SysMenu()
                    {
                        Key =  CurrencyConstant.HomeIndex,
                        MenuType = MenuType.Menu,
                        Icon = "code",
                        Url = $"{CurrencyConstant.CodeGenerationKey}/{CurrencyConstant.HomeIndex}",
                        ControllerName = CurrencyConstant.CodeGenerationKey
                    },
                    Children = new List<SysMenuView>()
                    {
                        new SysMenuView()
                        {
                            Entity = new SysMenu()
                            {
                                Key = "Select",
                                MenuType = MenuType.Button,
                                TargetType = TargetType.Callback,
                                ControllerName = CurrencyConstant.CodeGenerationKey,
                                ButtonPosition = ButtonPosition.Row
                            }
                        }
                    }

                },
                new SysMenuView()
                {
                    Entity = new SysMenu()
                    {
                        Key = "API",
                        MenuType = MenuType.API,
                        ControllerName = "API"
                    }
                }
            };
            await AddMenus(menus);
            await UpdateButton(menus);
        }

        private async Task UpdateButton(List<SysMenuView> menus)
        {
            var set = _dbContext.Set<SysMenu>();
            var menuBars = set.Where(u => u.Key == CurrencyConstant.HomeIndex);
            foreach (var item in menuBars)
            {
                item.MenuType = MenuType.Menu;
                item.Key = item.ControllerName;
                if (MenuIconDic.TryGetValue(item.Key, out string value))
                {
                    item.Icon = value;
                }
                item.ParentId = menus.Single(u => u.Entity.Key == CurrencyConstant.SysManagementKey).Id;
            }
            await _dbContext.SaveChangesAsync();
            var subMenu = set.AsEnumerable().Where(u => u.ControllerName != null && u.ControllerName != u.Key).GroupBy(u => u.ControllerName);
            List<SysMenu> catalogueList = new List<SysMenu>();
            foreach (var item in subMenu)
            {
                var catalogue = set.SingleOrDefault(u => u.ControllerName == item.Key && u.Key == item.Key);
                var id = 0;
                if (catalogue == null)
                {
                    id = menus.Single(u => u.Entity.Key == "API").Id;
                }
                else
                {
                    id = catalogue.Id;
                }
                await CreateButton(catalogue);
                foreach (var menu_item in item)
                {
                    menu_item.ParentId = id;
                    switch (menu_item.Key)
                    {
                        case CurrencyConstant.CreateEntityKey:
                            menu_item.MenuType = MenuType.Button;
                            menu_item.Icon = "appstore-add";
                            menu_item.TargetType = TargetType.EjectPage;
                            menu_item.Number = "997";
                            break;
                        case CurrencyConstant.UpdateEntityKey:
                            menu_item.MenuType = MenuType.Button;
                            menu_item.ButtonPosition = ButtonPosition.Row;
                            menu_item.Icon = "edit";
                            menu_item.TargetType = TargetType.EjectPage;
                            menu_item.Number = "998";
                            break;
                        case CurrencyConstant.DeleteEntityKey:
                            menu_item.MenuType = MenuType.Button;
                            menu_item.ButtonPosition = ButtonPosition.Row;
                            menu_item.IsDoubleTrue = true;
                            menu_item.Icon = "delete";
                            menu_item.TargetType = TargetType.Callback;
                            menu_item.Number = "999";
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

        private async Task CreateButton(SysMenu sysMenu)
        {
            if (sysMenu == null) return;
            switch (sysMenu.ControllerName)
            {
                case CurrencyConstant.ApplicationRoleKey:
                    SysMenu permissionFields = new SysMenu()
                    {
                        ButtonPosition = ButtonPosition.Row,
                        TargetType = TargetType.CurrentPage,
                        Url = UrlConfig.FieldPermissionsUrl,
                        Key = CurrencyConstant.FieldPermissionsKey,
                        ControllerName = CurrencyConstant.ApplicationRoleKey,
                        ParentId = sysMenu.ParentId,
                        Number = "996",
                        MenuType = MenuType.Button,
                    };
                    await _dbContext.AddAsync(permissionFields);
                    break;
                default:
                    break;
            }
            await _dbContext.SaveChangesAsync();
        }
        private async Task AddMenus(List<SysMenuView> sysMenuViews,int parentId = 0)
        {
            foreach (var item in sysMenuViews)
            {
                item.Entity.ParentId = parentId;
                _dbContext.Add(item.Entity);
                if (item.Children != null)
                {
                    await _dbContext.SaveChangesAsync();
                    await AddMenus(item.Children, item.Id);
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        public Dictionary<string,string> MenuIconDic { get; set; } =  new Dictionary<string, string>()
        {
            {"SysMenu","profile"} ,
            {"ApplicationUser","user" },
            { "ApplicationRole","user-switch"}
            
        };
    }
}
