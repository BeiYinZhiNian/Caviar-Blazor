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
        public async Task<List<ViewFields>> SavRoleFields<T>(List<ViewFields> fields,RoleManager<T> roleManager, int roleId) where T : class
        {
            var entity = fields.Select(x => x.Entity).ToList();
            await DbContext.UpdateEntityAsync(entity);
            var role = await roleManager.FindByIdAsync(roleId.ToString());
            var claims = await roleManager.GetClaimsAsync(role);
            foreach (var item in fields)
            {
                var type = PermissionType.Field.ToString();
                var value = CommonHelper.GetClaimValue(item.Entity);
                var claim = claims.SingleOrDefault(u => u.Type == type && u.Value == value);
                if(item.IsPermission && claim == null)
                {
                    claim = new Claim(type, value);
                    await roleManager.AddClaimAsync(role, claim);
                }
                else if(!item.IsPermission && claim != null)
                {
                    await roleManager.RemoveClaimAsync(role, claim);
                }
            }
            return fields;
        }

        public async Task<List<ViewFields>> GetRoleFields<T>(List<ViewFields> fields, RoleManager<T> roleManager, string fullName, int roleId) where T : class
        {
            var sysFields = await GetEntity<SysFields>(u => u.FullName == fullName);
            var role = await roleManager.FindByIdAsync(roleId.ToString());
            var claims = await roleManager.GetClaimsAsync(role);
            var type = PermissionType.Field.ToString();
            foreach (var item in fields)
            {
                var value = CommonHelper.GetClaimValue(item.Entity);
                item.Entity = sysFields.SingleOrDefault(u => u.FieldName == item.Entity.FieldName);
                var claim = claims.SingleOrDefault(u => u.Type == type && u.Value == value);
                item.IsPermission = claim != null ? true : false;
            }
            return fields;
        }
    }
}
