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
    public partial class SysMenuServices: EasyBaseServices<SysMenu>
    {
        public List<string> PermissionUrls { get; set; }

        private Expression<Func<SysMenu, bool>> _menuWhere;

        private ILanguageService _languageService;
        public SysMenuServices(IAppDbContext appDbContext,ILanguageService languageService):base(appDbContext)
        {
            _menuWhere = u => PermissionUrls.Contains(u.Url) || string.IsNullOrEmpty(u.Url);
            _languageService = languageService;
        }

        public override IQueryable<SysMenu> GetEntity(Expression<Func<SysMenu, bool>> where)
        {
            return base.GetEntity(where).Where(_menuWhere);
        }

        public override Task<List<SysMenu>> GetAllAsync()
        {
            return base.GetEntity(_menuWhere).ToListAsync();
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

        public override Task<SysMenu> GetEntity(int id)
        {
            return SingleOrDefaultAsync(u=>u.Id == id);
        }
        /// <summary>
        /// 获取当前菜单
        /// </summary>
        /// <returns></returns>
        public async Task<List<SysMenu>> GetMenuBar()
        {
            if (PermissionUrls == null) return new List<SysMenu>();
            var menus = await GetAllAsync();
            return menus;
        }

        public async Task<List<SysMenu>> GetApiList(string url,string[] controllerList)
        {
            if (url == null)
            {
                var unauthorized = _languageService[$"{CurrencyConstant.ExceptionMessage}.{CurrencyConstant.Null}"];
                throw new NotificationException("url" + unauthorized);
            }
            if (url[0] == '/' && url.Count() > 1) url = url.Substring(1);
            var entity = await SingleOrDefaultAsync(u => u.Url.ToLower() == url.ToLower());
            if (entity == null) throw new NotificationException($"未找到{url}所需资源");
            var apiList = await GetEntity(u => u.ControllerName == entity.ControllerName).ToListAsync();
            if(controllerList != null)
            {
                foreach (var item in controllerList)
                {
                    if (item == "") continue;
                    var otherApi = await GetEntity(u => u.ControllerName == item).ToListAsync();
                    apiList.AddRange(otherApi);
                }
            }
            return apiList;
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

        public async Task DeleteEntityAll(List<SysMenu> menus)
        {
            await base.DeleteEntity(menus);
        }

        public override async Task<bool> DeleteEntity(SysMenu menus)
        {
            List<SysMenu> menuList = await GetEntity(u => u.ParentId == menus.Id).ToListAsync();
            if (menuList != null && menuList.Count > 1)
            {
                foreach (var item in menuList)
                {
                    if (item.ParentId == menus.Id)
                    {
                        item.ParentId = menus.ParentId;
                    }
                }
                await base.UpdateEntity(menuList);
            }
            return await base.DeleteEntity(menus);
        }
    }
}
