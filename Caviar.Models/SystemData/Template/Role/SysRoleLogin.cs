using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    public partial class SysRoleLogin : SysBaseModel
    {
        public virtual SysRole Role { get; set; }

        public virtual SysUserLogin User { get; set; }

        public int UserId { get; set; }

        public int RoleId { get; set; }
    }
}
