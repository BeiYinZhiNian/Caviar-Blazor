// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CacheManager.Core;
using Caviar.Core.Interface;
using Caviar.SharedKernel.Common;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.EntityFrameworkCore;

namespace Caviar.Core.Services
{
    public partial class SysMenuServices : EasyBaseServices<SysMenu, SysMenuView>
    {
        private readonly Expression<Func<SysMenu, bool>> _menuWhere;
        private readonly ILanguageService _languageService;
        private readonly IInteractor _interactor;
        private readonly ICacheManager<object> _cacheManager;

        public SysMenuServices(IAppDbContext appDbContext,
            ILanguageService languageService,
            IInteractor interactor,
            ICacheManager<object> cacheManager) : base(appDbContext)
        {
            _cacheManager = cacheManager;
            _languageService = languageService;
            _interactor = interactor;
            _menuWhere = u => _interactor.PermissionUrls.Contains(u.Url) || (_interactor.PermissionUrls.Contains(u.Id.ToString()) && string.IsNullOrEmpty(u.Url));
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
            if (_interactor.PermissionUrls == null) return null;
            var func = _menuWhere.Compile();
            if (func(entity)) return entity;
            var unauthorized = _languageService[$"{CurrencyConstant.ExceptionMessage}.{CurrencyConstant.Unauthorized}"];
            var neme = _languageService[$"{CurrencyConstant.Menu}.{entity.MenuName}"];
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
                string key = $"{CurrencyConstant.Menu}.{item.Entity.MenuName}";
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
            var cacheName = $"menuBar_{string.Join(",", _interactor.ApplicationRoles.Select(u => u.Name))}";
            var cache = _cacheManager.Get<List<SysMenuView>>(cacheName);
            if (cache != null) return cache;
            if (_interactor.PermissionUrls == null) return new List<SysMenuView>();
            var menus = await GetAllAsync().Where(u => u.MenuType == MenuType.Menu || u.MenuType == MenuType.Catalog || u.MenuType == MenuType.Settings).ToListAsync();
            cache = ToView(menus).ListToTree();
            _cacheManager.Add(cacheName, cache);
            return cache;
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
            var cacheName = $"apis_{indexUrl}_{string.Join(",", _interactor.ApplicationRoles.Select(u => u.Name))}";
            var cache = _cacheManager.Get<List<SysMenuView>>(cacheName);
            if (cache != null) return cache;
            var menu = await SingleOrDefaultAsync(u => u.Url == indexUrl);
            if (menu == null) throw new ArgumentException("未获得该页面权限");
            var apiList = await GetEntityAsync(u => u.ParentId == menu.Id).ToListAsync();
            apiList.Add(menu);
            cache = ToView(apiList);
            _cacheManager.Add(cacheName, cache);
            return cache;
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

        public override Task<int> AddEntityAsync(SysMenu entity)
        {
            entity.DataId = CurrencyConstant.PublicData; // 创建菜单默认为公共数据
            return base.AddEntityAsync(entity);
        }
    }
}
