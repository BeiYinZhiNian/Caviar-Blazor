using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.Services
{
    public class RoleServices:DbServices
    {
        private RoleManager<ApplicationRole> _roleManager;
        public RoleServices(RoleManager<ApplicationRole> roleManager,IAppDbContext appDbContext) : base(appDbContext)
        {
            _roleManager = roleManager;
        }

        public async Task<IList<ApplicationRole>> GetRoles(IList<string> roles)
        {
            IList<ApplicationRole> rolesList = new List<ApplicationRole>();
            foreach (var item in roles)
            {
                var role = await _roleManager.FindByNameAsync(item);
                rolesList.Add(role);
            }
            return rolesList;
        }

        public async Task<IdentityResult> CreateUserAsync(ApplicationRoleView vm)
        {
            var result = await _roleManager.CreateAsync(vm.Entity);
            if (result.Succeeded)
            {
                var templateRole = await _roleManager.FindByNameAsync(CurrencyConstant.TemplateRole);
                if(templateRole != null)
                {
                    var set = AppDbContext.DbContext.Set<SysPermission>();
                    var tempPermission = await set.Where(u => u.Entity == templateRole.Id).ToListAsync();
                    tempPermission = tempPermission.Select(u => new SysPermission() { Permission = u.Permission,PermissionType = u.PermissionType,Entity = vm.Entity.Id }).ToList();
                    set.AddRange(tempPermission);
                    await AppDbContext.DbContext.SaveChangesAsync();
                }
            }
            return result;
        }

        public async Task<IdentityResult> DeleteUserAsync(ApplicationRoleView vm)
        {
            if(vm.Entity.Name == CurrencyConstant.TemplateRole || vm.Entity.Name == CurrencyConstant.Admin)
            {
                throw new Exception("特殊角色，禁止删除");
            }
            var result = await _roleManager.DeleteAsync(vm.Entity);
            if (result.Succeeded)
            {
                var set = AppDbContext.DbContext.Set<SysPermission>();
                var permission = await set.Where(u => u.Entity == vm.Entity.Id).ToListAsync();
                set.RemoveRange(permission);
                AppDbContext.DbContext.SaveChanges();
            }
            return result;
        }

        public async Task<IdentityResult> UpdateUserAsync(ApplicationRoleView vm)
        {
            var role = await _roleManager.FindByNameAsync(vm.Entity.Name);
            if (role == null) throw new ArgumentNullException($"{vm.Entity.Name}不存在");
            role.Name = vm.Entity.Name;
            role.Number = vm.Entity.Number;
            role.Remark = vm.Entity.Remark;
            role.DataList = vm.Entity.DataList;
            role.IsDisable = vm.Entity.IsDisable;
            role.DataRange = vm.Entity.DataRange;
            role.UpdateTime = CommonHelper.GetSysDateTimeNow();
            role.OperatorUp = vm.Entity.OperatorUp;
            var result = await _roleManager.UpdateAsync(role);
            return result;
        }

        
    }
}
