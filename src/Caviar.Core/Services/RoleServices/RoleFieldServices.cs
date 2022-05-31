// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CacheManager.Core;
using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Caviar.Core.Services
{
    public class RoleFieldServices : BaseServices
    {
        private readonly IAppDbContext _appDbContext;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ICacheManager<object> _cacheManager;
        public RoleFieldServices(IAppDbContext dbContext,
            RoleManager<ApplicationRole> roleManager,
            ICacheManager<object> cacheManager)
        {
            _appDbContext = dbContext;
            _roleManager = roleManager;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// 保存角色字段权限
        /// </summary>
        /// <returns></returns>
        public async Task<List<FieldsView>> SavRoleFields(List<FieldsView> fields, string roleName)
        {
            var entity = fields.Select(x => x.Entity).ToList();
            var role = await _roleManager.FindByNameAsync(roleName);
            await _appDbContext.UpdateEntityAsync(entity);
            foreach (var item in fields)
            {
                var type = PermissionType.RoleFields.ToString();
                var value = CommonHelper.GetClaimValue(item.Entity);
                var set = _appDbContext.DbContext.Set<SysPermission>();
                var permission = set.SingleOrDefault(u => u.Permission == (item.Entity.FullName + item.Entity.FieldName) && u.Entity == role.Id && u.PermissionType == (int)PermissionType.RoleFields);
                if (item.IsPermission && permission == null)
                {
                    permission = new SysPermission() { Entity = role.Id, Permission = (item.Entity.FullName + item.Entity.FieldName), PermissionType = (int)PermissionType.RoleFields };
                    set.Add(permission);
                }
                else if (!item.IsPermission && permission != null)
                {
                    set.Remove(permission);
                }
                await _appDbContext.SaveChangesAsync();
            }
            _cacheManager.Clear();
            return fields;
        }
        /// <summary>
        /// 读取指定表，将表中所有数据进行缓存(无过期)
        /// 适用于不常更新，经常需要查找的表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        private async Task<List<T>> ReadPermanentCache<T>(string cacheName) where T : class
        {
            var sysPermissions = _cacheManager.Get<List<T>>(cacheName);
            if (sysPermissions == null)
            {
                var set = _appDbContext.DbContext.Set<T>();
                sysPermissions = await set.ToListAsync();
                _cacheManager.Add(cacheName, sysPermissions);
                // 在未更新字段权限时，无需更新权限表
                _cacheManager.Expire(cacheName, ExpirationMode.None, default);
            }
            return sysPermissions;
        }

        public async Task<List<FieldsView>> GetRoleFields(List<FieldsView> fields, string fullName, IList<int> roleIds)
        {
            var sysFields = await ReadPermanentCache<SysFields>("field_service_sysFields");
            sysFields = sysFields.Where(u => u.FullName == fullName).ToList();
            var sysPermissions = await ReadPermanentCache<SysPermission>("field_service_sysPermissions");
            foreach (var item in fields)
            {
                if (item.Entity == null) continue;
                item.Entity = sysFields.SingleOrDefault(u => u.FieldName == item.Entity.FieldName);
                var permission = sysPermissions.FirstOrDefault(u => u.Permission == (item.Entity.FullName + item.Entity.FieldName) && roleIds.Contains(u.Entity) && u.PermissionType == (int)PermissionType.RoleFields);
                item.IsPermission = permission != null;
            }
            fields = fields.OrderBy(u => u.Entity.Number).ToList();
            return fields;
        }
    }
}
