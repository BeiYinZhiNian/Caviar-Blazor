using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Pages.Permission
{
    public partial class PermissionRoleUserGroup : ITableTemplate
    {
        [Parameter]
        public ViewUserGroup DataSource { get; set; }
        [Parameter]
        public string Url { get; set; }

        PermissionRoleTemple<ViewUserGroup> Temple;
        public async Task<bool> Validate()
        {
            return await Temple.Submit();
        }
    }
}
