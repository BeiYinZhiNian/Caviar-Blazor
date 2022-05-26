// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public RoleFieldServices(IAppDbContext dbContext, RoleManager<ApplicationRole> roleManager)
        {
            _appDbContext = dbContext;
            _roleManager = roleManager;
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
            return fields;
        }


        public async Task<List<FieldsView>> GetRoleFields(List<FieldsView> fields, string fullName, IList<int> roleIds)
        {
            var sysFields = await _appDbContext.GetEntityAsync<SysFields>(u => u.FullName == fullName).ToListAsync();
            foreach (var item in fields)
            {
                if (item.Entity == null) continue;
                item.Entity = sysFields.SingleOrDefault(u => u.FieldName == item.Entity.FieldName);
                var set = _appDbContext.DbContext.Set<SysPermission>();
                var permission = set.FirstOrDefault(u => u.Permission == (item.Entity.FullName + item.Entity.FieldName) && roleIds.Contains(u.Entity) && u.PermissionType == (int)PermissionType.RoleFields);
                item.IsPermission = permission != null;
            }
            fields = fields.OrderBy(u => u.Entity.Number).ToList();
            return fields;
        }
    }
}
