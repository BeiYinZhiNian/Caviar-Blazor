using Caviar.Core.Interface;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Identity;
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

        
    }
}
