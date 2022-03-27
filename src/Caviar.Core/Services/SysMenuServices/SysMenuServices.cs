using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Services
{
    public partial class SysMenuServices: EasyBaseServices<SysMenu,SysMenuView>
    {
        public List<string> PermissionUrls { get; set; }

        private Expression<Func<SysMenu, bool>> _menuWhere;

        private ILanguageService _languageService;
        public SysMenuServices(IAppDbContext appDbContext,ILanguageService languageService):base(appDbContext)
        {
            _menuWhere = u => PermissionUrls.Contains(u.Url) || string.IsNullOrEmpty(u.Url);
            _languageService = languageService;
        }

        public override IQueryable<SysMenu> GetEntityAsync(Expression<Func<SysMenu, bool>> where)
        {
            return base.GetEntityAsync(where).Where(_menuWhere);
        }
        /// <summary>
        /// 获取所有权限url
        /// </summary>
        /// <returns></returns>
        public override IQueryable<SysMenu> GetAllAsync()
        {
            return base.GetEntityAsync(_menuWhere);
        }



        public override async Task<SysMenu> SingleOrDefaultAsync(Expression<Func<SysMenu, bool>> where)
        {
            var entity = await base.SingleOrDefaultAsync(where);
            if (entity == null) return null;
            if (PermissionUrls == null) return null;
            var func = _menuWhere.Compile();
            if (func(entity)) return entity;
            var unauthorized = _languageService[$"{CurrencyConstant.ExceptionMessage}.{CurrencyConstant.Unauthorized}"];
            var neme = _languageService[$"{CurrencyConstant.Menu}.{entity.Key}"];
            var message = neme + unauthorized;
            throw new Exception(message);
        }

        protected List<SysMenu> ToEntity(SysMenuView vm)
        {
            List<SysMenuView> menuViews = new List<SysMenuView>();
            vm.TreeToList(menuViews);
            return base.ToEntity(menuViews);
        }

        protected override List<SysMenuView> ToView(List<SysMenu> entity)
        {
            var vm = base.ToView(entity);
            foreach (var item in vm)
            {
                string key = $"{CurrencyConstant.Menu}.{item.Entity.Key}";
                item.DisplayName = _languageService[key];//翻译显示名称
            }
            return vm;
        }

        public override async Task<PageData<SysMenuView>> GetPageAsync(Expression<Func<SysMenu, bool>> where, int pageIndex, int pageSize, bool isOrder = true)
        {
            var pages = await AppDbContext.GetPageAsync(_menuWhere.And(where), u => u.Number, pageIndex, pageSize, isOrder);
            var pageViews = ToView(pages);
            pageViews.Rows = pageViews.Rows.ListToTree();
            return pageViews;
        }

        /// <summary>
        /// 获取左侧菜单栏
        /// </summary>
        /// <returns></returns>
        public async Task<List<SysMenuView>> GetMenuBar()
        {
            if (PermissionUrls == null) return new List<SysMenuView>();
            var menus = await GetAllAsync().Where(u=>u.MenuType == MenuType.Menu || u.MenuType == MenuType.Catalog || u.MenuType == MenuType.Setting).ToListAsync();
            return ToView(menus).ListToTree();
        }
        /// <summary>
        /// 获取当前url下可用api
        /// </summary>
        /// <param name="crcontrollerName"></param>
        /// <param name="controllerList"></param>
        /// <returns></returns>
        /// <exception cref="NotificationException"></exception>
        public async Task<List<SysMenuView>> GetApis(string indexUrl)
        {
            var menu = await SingleOrDefaultAsync(u=>u.Url == indexUrl);
            if (menu == null) throw new ArgumentException("未获得该页面权限");
            var apiList = await GetEntityAsync(u => u.ParentId == menu.Id).ToListAsync();
            apiList.Add(menu);
            return ToView(apiList);
        }
        /// <summary>
        /// 获取权限菜单
        /// </summary>
        /// <returns></returns>
        public async Task<List<SysMenuView>> GetPermissionMenus(List<string> permissionUrls)
        {
            var menus = await base.GetAllAsync().ToListAsync();
            var menuViews = ToView(menus);
            foreach (var item in menuViews)
            {
                if (string.IsNullOrEmpty(item.Entity.Url))
                {
                    item.IsPermission = true;
                }
                else
                {
                    item.IsPermission = permissionUrls.Contains(item.Entity.Url);
                }
            }
            menuViews = menuViews.ListToTree();
            return menuViews;
        }
        /// <summary>
        /// 删除所有菜单数
        /// </summary>
        /// <param name="menuViews"></param>
        /// <returns></returns>
        public async Task<int> DeleteEntityAll(SysMenuView menuViews)
        {
            var menus = ToEntity(menuViews);
            var count = await base.DeleteEntityAsync(menus);
            return count;
        }
        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public override async Task<bool> DeleteEntityAsync(SysMenu menus)
        {
            List<SysMenu> menuList = await GetEntityAsync(u => u.ParentId == menus.Id).ToListAsync();
            if (menuList != null && menuList.Count > 1)
            {
                foreach (var item in menuList)
                {
                    if (item.ParentId == menus.Id)
                    {
                        item.ParentId = menus.ParentId;
                    }
                }
                await base.UpdateEntityAsync(menuList);
            }
            return await base.DeleteEntityAsync(menus);
        }
    }
}
