using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
namespace Caviar.Core.Services
{
    public class RoleFieldServices : DbServices
    {
        /// <summary>
        /// 保存角色字段权限
        /// </summary>
        /// <returns></returns>
        public async Task<List<ViewFields>> SavRoleFields(List<ViewFields> fields, string roleName) 
        {
            var entity = fields.Select(x => x.Entity).ToList();
            await AppDbContext.UpdateEntityAsync(entity);
            foreach (var item in fields)
            {
                var type = PermissionType.RoleFields.ToString();
                var value = CommonHelper.GetClaimValue(item.Entity);
                var set = AppDbContext.DbContext.Set<SysPermission>();
                var permission = set.SingleOrDefault(u => u.PermissionId == item.Entity.Id && u.EntityName == roleName && u.PermissionType == PermissionType.RoleFields);
                if (item.IsPermission && permission == null)
                {
                    permission = new SysPermission() { EntityName = roleName, PermissionId = item.Entity.Id, PermissionType = PermissionType.RoleFields };
                    set.Add(permission);
                }
                else if(!item.IsPermission && permission != null)
                {
                    set.Remove(permission);
                }
                await AppDbContext.SaveChangesAsync();
            }
            return fields;
        }


        public async Task<List<ViewFields>> GetRoleFields(List<ViewFields> fields, string fullName, IList<string> roleNames)
        {
            var sysFields = await GetEntity<SysFields>(u => u.FullName == fullName);
            foreach (var item in fields)
            {
                item.Entity = sysFields.SingleOrDefault(u => u.FieldName == item.Entity.FieldName);
                var set = AppDbContext.DbContext.Set<SysPermission>();
                var permission = set.SingleOrDefault(u => u.PermissionId == item.Entity.Id && roleNames.Contains(u.EntityName) && u.PermissionType == PermissionType.RoleFields);
                item.IsPermission = permission != null;
            }
            return fields;
        }
    }
}
