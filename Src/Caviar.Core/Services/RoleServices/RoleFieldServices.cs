using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Caviar.Core.Interface;
using Microsoft.EntityFrameworkCore;

namespace Caviar.Core.Services
{
    public class RoleFieldServices : BaseServices
    {
        private IAppDbContext _appDbContext;
        public RoleFieldServices(IAppDbContext dbContext)
        {
            _appDbContext = dbContext;
        }

        /// <summary>
        /// 保存角色字段权限
        /// </summary>
        /// <returns></returns>
        public async Task<List<FieldsView>> SavRoleFields(List<FieldsView> fields, string roleName) 
        {
            var entity = fields.Select(x => x.Entity).ToList();
            await _appDbContext.UpdateEntityAsync(entity);
            foreach (var item in fields)
            {
                var type = PermissionType.RoleFields.ToString();
                var value = CommonHelper.GetClaimValue(item.Entity);
                var set = _appDbContext.DbContext.Set<SysPermission>();
                var permission = set.SingleOrDefault(u => u.Permission == (item.Entity.FullName + item.Entity.FieldName) && u.Entity == roleName && u.PermissionType == PermissionType.RoleFields);
                if (item.IsPermission && permission == null)
                {
                    permission = new SysPermission() { Entity = roleName, Permission = (item.Entity.FullName + item.Entity.FieldName), PermissionType = PermissionType.RoleFields };
                    set.Add(permission);
                }
                else if(!item.IsPermission && permission != null)
                {
                    set.Remove(permission);
                }
                await _appDbContext.SaveChangesAsync();
            }
            return fields;
        }


        public async Task<List<FieldsView>> GetRoleFields(List<FieldsView> fields, string fullName, IList<string> roleNames)
        {
            var sysFields = await _appDbContext.GetEntityAsync<SysFields>(u => u.FullName == fullName).ToListAsync();
            foreach (var item in fields)
            {
                item.Entity = sysFields.SingleOrDefault(u => u.FieldName == item.Entity.FieldName);
                var set = _appDbContext.DbContext.Set<SysPermission>();
                var permission = set.SingleOrDefault(u => u.Permission == (item.Entity.FullName + item.Entity.FieldName) && roleNames.Contains(u.Entity) && u.PermissionType == PermissionType.RoleFields);
                item.IsPermission = permission != null;
            }
            fields = fields.OrderBy(u => u.Entity.Number).ToList();
            return fields;
        }
    }
}
