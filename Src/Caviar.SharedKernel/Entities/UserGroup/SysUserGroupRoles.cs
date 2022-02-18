using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    /// <summary>
    /// 用户组角色
    /// </summary>
    public class SysUserGroupRoles : SysBaseEntity
    {
        public int RoleId { get; set; }
        public int UserId { get; set; }
    }
}
