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
        ApplicationRole AdminRole;
        ApplicationRole TemplateRole;
        ApplicationRole TouristRole;
        int DataId = 1;//数据权限id

        string[] TemplateRoleUrls = new string[]
        {
                UrlConfig.Home,
                UrlConfig.GetMenuBar,
                UrlConfig.Login,
                UrlConfig.Logout,
                UrlConfig.SignInActual,
                UrlConfig.LogoutServer,
                UrlConfig.GetApis,
                UrlConfig.CurrentUserInfo,
                UrlConfig.SetCookieLanguage,
                UrlConfig.UploadHeadPortrait,
                UrlConfig.ChangePassword,
                UrlConfig.AuvancedQuery,
                UrlConfig.MyDetails,
                UrlConfig.UpdateDetails,
        };

        string[] TemplateRoleFildes = new string[]
        {
            "Caviar.SharedKernel.Entities.SysMenuUrl",
            "Caviar.SharedKernel.Entities.SysMenuIcon",
            "Caviar.SharedKernel.Entities.SysMenuParentId",
            "Caviar.SharedKernel.Entities.SysMenuNumber",
            "Caviar.SharedKernel.Entities.SysMenuKey",
            "Caviar.SharedKernel.Entities.SysMenuMenuType",
            "Caviar.SharedKernel.Entities.SysMenuTargetType",
            "Caviar.SharedKernel.Entities.SysEnclosureFilePath"
        };

        string[] TouristRoleUrls = new string[]
        {
                UrlConfig.Home,
                UrlConfig.GetMenuBar,
                UrlConfig.Login,
                UrlConfig.SignInActual,
        };

        string[] TouristRoleFildes = new string[]
        {
            "Caviar.SharedKernel.Entities.SysMenuUrl",
            "Caviar.SharedKernel.Entities.SysMenuIcon",
            "Caviar.SharedKernel.Entities.SysMenuParentId",
            "Caviar.SharedKernel.Entities.SysMenuNumber",
            "Caviar.SharedKernel.Entities.SysMenuKey",
            "Caviar.SharedKernel.Entities.SysMenuMenuType",
            "Caviar.SharedKernel.Entities.SysMenuTargetType",
        };
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
        public async Task HttpMethodsInit(List<SysMenu> menus)
        {
            var set = _dbContext.Set<SysMenu>();
            foreach (var item in menus)
            {
                var menu = await set.SingleOrDefaultAsync(u => u.Url == item.Url);
                if (menu == null)
                {
                    var parent = await set.SingleOrDefaultAsync(u=>u.ControllerName == item.ControllerName && u.Key == item.ControllerName);
                    if (parent != null)
                    {
                        item.ParentId = parent.Id;
                    }
                    _dbContext.Add(item);
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        public List<SysMenu> GetSysMenu()
        {
            //需要忽略的控制器
            List<Type> baseController = new List<Type>()
            {
                typeof(EasyBaseApiController<,>)
            };
            var controllerList = ApiScannerServices.GetAllController(typeof(BaseApiController), baseController);
            var methodsList = ApiScannerServices.GetAllMethods(controllerList, typeof(HttpMethodAttribute));
            return methodsList;
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
                var permission = (item.FullName + item.FieldName);
                _dbContext.Add(new SysPermission()
                {
                    Entity = AdminRole.Id,
                    Permission = permission,
                    PermissionType = PermissionType.RoleFields
                });
                if (TemplateRoleFildes.Contains(permission))
                {
                    _dbContext.Add(new SysPermission()
                    {
                        Entity = TemplateRole.Id,
                        Permission = permission,
                        PermissionType = PermissionType.RoleFields
                    });
                }
                if (TemplateRoleFildes.Contains(permission))
                {
                    _dbContext.Add(new SysPermission()
                    {
                        Entity = TouristRole.Id,
                        Permission = permission,
                        PermissionType = PermissionType.RoleFields
                    });
                }
            }
            await _dbContext.SaveChangesAsync();
        }
        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        protected virtual async Task<bool> DatabaseInit(IDbContext dbContext)
        {
            try
            {
                return await dbContext.Database.EnsureCreatedAsync();
            }
            catch (InvalidCastException ex)
            {
                //此错误为mysql数据库已经存在时报错
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        protected virtual async Task CreateData(bool isDatabaseInit)
        {
            var sysMenus = GetSysMenu();
            await HttpMethodsInit(sysMenus);
            if (!isDatabaseInit) return;
            await CreateInitRole();
            await CreateInitUserGroup();
            await CreateInitUser();
            
            var createMenus = await CreateMenu();
            await CreatePermissionMenu(sysMenus);
            await CreatePermissionMenu(createMenus);
        }

        protected virtual async Task CreatePermissionMenu(List<SysMenu> menus)
        {
            var set = _dbContext.Set<SysPermission>();
            foreach (var item in menus)
            {
                if (string.IsNullOrEmpty(item.Url)) continue;
                set.Add(new SysPermission() { Permission = item.Url,Entity = AdminRole.Id, PermissionType = PermissionType.RoleMenus});
                if (TemplateRoleUrls.Contains(item.Url))
                {
                    set.Add(new SysPermission() { Permission = item.Url, Entity = TemplateRole.Id, PermissionType = PermissionType.RoleMenus });
                }
                if (TouristRoleUrls.Contains(item.Url))
                {
                    set.Add(new SysPermission() { Permission = item.Url, Entity = TouristRole.Id, PermissionType = PermissionType.RoleMenus });
                }
            }
            await _dbContext.SaveChangesAsync();
        }

       

        protected virtual async Task CreateInitUserGroup()
        {
            var set = _dbContext.Set<SysUserGroup>();
            set.Add(new SysUserGroup() { Name = "总部",DataId = DataId });
            await _dbContext.SaveChangesAsync();
        }
        protected virtual async Task CreateInitUser()
        {
            var userManager = _serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = new ApplicationUser { Email = "1031622947@qq.com", AccountName = "北音执念", UserName = CurrencyConstant.Admin, Remark = "生活的意义就是折腾！",UserGroupId = DataId,DataId = DataId};
            var result = await userManager.CreateAsync(user, CommonHelper.SHA256EncryptString(CurrencyConstant.DefaultPassword));
            if (!result.Succeeded) throw new Exception("创建管理员账号失败，数据初始化停止");
            await userManager.AddToRoleAsync(user, CurrencyConstant.Admin);
            user = new ApplicationUser { Email = "123@qq.com", AccountName = "游客", UserName = CurrencyConstant.TouristUser, Remark = "生活的意义就是折腾！", UserGroupId = DataId, DataId = DataId };
            result = await userManager.CreateAsync(user, CommonHelper.SHA256EncryptString(CurrencyConstant.DefaultPassword));
            if (!result.Succeeded) throw new Exception("创建游客账号失败，数据初始化停止");
            await userManager.AddToRoleAsync(user, CurrencyConstant.TouristRole);
        }

        protected virtual async Task CreateInitRole()
        {
            var roleManager = _serviceScope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var role = new ApplicationRole { Name = CurrencyConstant.Admin ,DataId = DataId ,DataRange = DataRange.All,Remark="超级管理员" };
            var result = await roleManager.CreateAsync(role);
            AdminRole = role;
            if (!result.Succeeded) throw new Exception("创建角色数据失败，数据初始化停止");
            role = new ApplicationRole { Name = CurrencyConstant.TemplateRole, DataId = DataId, DataRange = DataRange.Level, Remark = "模板角色，用于生成角色之后，自动拷贝该角色权限，请勿删除" };
            result = await roleManager.CreateAsync(role);
            TemplateRole = role;
            if (!result.Succeeded) throw new Exception("创建模板角色数据失败，数据初始化停止");
            role = new ApplicationRole { Name = CurrencyConstant.TouristRole, DataId = DataId, DataRange = DataRange.Level, Remark = "游客角色，用于设置游客访问权限，请勿删除" };
            result = await roleManager.CreateAsync(role);
            TouristRole = role;
            if (!result.Succeeded) throw new Exception("创建游客角色数据失败，数据初始化停止");
        }

        protected virtual async Task<List<SysMenu>> CreateMenu()
        {
            List<SysMenu> permissionMenu = new List<SysMenu>();
            List<SysMenuView> menus = new List<SysMenuView>()
            {
                new SysMenuView()
                {
                    Entity = new SysMenu()
                    {
                        Key = CurrencyConstant.SysManagementKey,
                        Icon = "windows",
                    }
                    
                },
                new SysMenuView()
                {
                    Entity = new SysMenu()
                    {
                        Key = CurrencyConstant.HomeKey,
                        Icon = "home",
                        MenuType = MenuType.Menu,
                        Url = UrlConfig.Home,
                        Number = "10",
                    }
                    
                },
                new SysMenuView()
                {
                    Entity = new SysMenu()
                    {
                        Key =  CurrencyConstant.Index,
                        MenuType = MenuType.Menu,
                        Icon = "code",
                        Url = $"{CurrencyConstant.CodeGenerationKey}/{CurrencyConstant.Index}",
                        ControllerName = CurrencyConstant.CodeGenerationKey,
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
                                ButtonPosition = ButtonPosition.Row,
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
                        ControllerName = "API",
                    }
                }
            };
            permissionMenu.AddRange(menus.Select(u => u.Entity));
            await AddMenus(menus);
            var buttons = await UpdateApi(menus);
            permissionMenu.AddRange(buttons);
            return permissionMenu;
        }
        /// <summary>
        /// 修改api类型
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        private async Task<List<SysMenu>> UpdateApi(List<SysMenuView> menus)
        {
            List<SysMenu> addButtons = new List<SysMenu>();//新增的按钮
            var set = _dbContext.Set<SysMenu>();
            var menuBars = set.Where(u => u.Key == CurrencyConstant.Index);
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
                var buttons = await AddOtherButton(catalogue);
                addButtons.AddRange(buttons);
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
                        case CurrencyConstant.UploadKey:
                            menu_item.MenuType = MenuType.Button;
                            menu_item.Icon = "cloud-upload";
                            menu_item.TargetType = TargetType.Callback;
                            menu_item.Number = "997";
                            break;
                        case CurrencyConstant.DownloadKey:
                            menu_item.MenuType = MenuType.Button;
                            menu_item.Icon = "cloud-download";
                            menu_item.TargetType = TargetType.Callback;
                            menu_item.Number = "997";
                            menu_item.ButtonPosition = ButtonPosition.Row;
                            break;
                        default:
                            break;
                    }
                    catalogueList.Add(menu_item);
                }
            }
            _dbContext.UpdateRange(catalogueList);
            await _dbContext.SaveChangesAsync();
            return addButtons;
        }

        private async Task<List<SysMenu>> AddOtherButton(SysMenu sysMenu)
        {
            List<SysMenu> menus = new List<SysMenu>();
            if (sysMenu == null) return menus;
            switch (sysMenu.Key)
            {
                case CurrencyConstant.ApplicationRoleKey:
                    menus = new List<SysMenu>()
                    {
                        new SysMenu()
                        {
                            ButtonPosition = ButtonPosition.Row,
                            TargetType = TargetType.CurrentPage,
                            Url = UrlConfig.FieldPermissionsUrl,
                            Key = CurrencyConstant.FieldPermissionsKey,
                            ControllerName = CurrencyConstant.ApplicationRoleKey,
                            ParentId = sysMenu.Id,
                            Number = "996",
                            MenuType = MenuType.Button,
                        },
                        new SysMenu()
                        {
                            ButtonPosition = ButtonPosition.Row,
                            TargetType = TargetType.EjectPage,
                            Url = UrlConfig.MenuPermissionsUrl,
                            Key = CurrencyConstant.MenuPermissionsKey,
                            ControllerName = CurrencyConstant.ApplicationRoleKey,
                            ParentId = sysMenu.Id,
                            Number = "996",
                            MenuType = MenuType.Button,
                        }
                    };
                    break;
                case CurrencyConstant.ApplicationUserKey:
                    menus = new List<SysMenu>()
                    {
                        new SysMenu()
                        {
                            ButtonPosition = ButtonPosition.Row,
                            TargetType = TargetType.EjectPage,
                            Url = UrlConfig.PermissionUserRoles,
                            Key = CurrencyConstant.PermissionUserRolesKey,
                            ControllerName = CurrencyConstant.ApplicationUserKey,
                            ParentId = sysMenu.Id,
                            Number = "996",
                            MenuType = MenuType.Button,
                        },
                    };
                    
                    break;
                default:
                    break;
            }
            await _dbContext.AddRangeAsync(menus);
            await _dbContext.SaveChangesAsync();
            return menus;
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
            {CurrencyConstant.SysMenuKey,"profile"} ,
            {CurrencyConstant.ApplicationUserKey,"user" },
            {CurrencyConstant.ApplicationRoleKey,"user-switch"},
            {CurrencyConstant.SysUserGroupKey,"usergroup-add" },
            {CurrencyConstant.SysLogKey,"bug" },
            {CurrencyConstant.SysEnclosureKey ,"deployment-unit" },
        };
    }
}
