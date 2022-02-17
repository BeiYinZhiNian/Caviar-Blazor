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

        public override Task<List<SysMenu>> GetAllAsync()
        {
            return base.GetEntityAsync(_menuWhere).ToListAsync();
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
            throw new NotificationException(message);
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

        public override async Task<PageData<SysMenuView>> GetPageAsync(Expression<Func<SysMenu, bool>> where, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true)
        {
            var pages = await AppDbContext.GetPageAsync(where, u => u.Number, pageIndex, pageSize, isOrder, isNoTracking);
            var pageViews = ToView(pages);
            pageViews.Rows = pageViews.Rows.ListToTree();
            return pageViews;
        }

        /// <summary>
        /// 获取当前菜单
        /// </summary>
        /// <returns></returns>
        public async Task<List<SysMenuView>> GetMenuBar()
        {
            if (PermissionUrls == null) return new List<SysMenuView>();
            var menus = await GetAllAsync();
            return ToView(menus).ListToTree();
        }

        public async Task<List<SysMenuView>> GetApiList(string url,string[] controllerList)
        {
            if (url == null)
            {
                var unauthorized = _languageService[$"{CurrencyConstant.ExceptionMessage}.{CurrencyConstant.Null}"];
                throw new NotificationException($"{url}:{unauthorized}");
            }
            if (url[0] == '/' && url.Count() > 1) url = url.Substring(1);
            var entity = await SingleOrDefaultAsync(u => u.Url.ToLower() == url.ToLower());
            if (entity == null)
            {
                var notFound = _languageService[$"{CurrencyConstant.ExceptionMessage}.{CurrencyConstant.NotFound}"];
                throw new NotificationException($"{url}:{notFound}");
            }
            var apiList = await GetEntityAsync(u => u.ControllerName == entity.ControllerName).ToListAsync();
            if(controllerList != null)
            {
                foreach (var item in controllerList)
                {
                    if (item == "") continue;
                    var otherApi = await GetEntityAsync(u => u.ControllerName == item).ToListAsync();
                    apiList.AddRange(otherApi);
                }
            }
            return ToView(apiList);
        }

        public async Task<List<SysMenuView>> GetPermissionMenus()
        {
            var menus = await base.GetAllAsync();
            var menuViews = menus.Select(u => new SysMenuView() { Entity = u }).ToList();
            foreach (var item in menuViews)
            {
                item.IsPermission = PermissionUrls.Contains(item.Entity.Key);
            }
            return menuViews;
        }

        public async Task<int> DeleteEntityAll(SysMenuView menuViews)
        {
            var menus = ToEntity(menuViews);
            var count = await base.DeleteEntityAsync(menus);
            return count;
        }

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
