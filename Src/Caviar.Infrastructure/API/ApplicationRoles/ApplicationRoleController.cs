using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Infrastructure.API
{
    public partial class ApplicationRoleController
    {
        private RoleManager<ApplicationRole> _roleManager;
        public ApplicationRoleController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }


    }
}
