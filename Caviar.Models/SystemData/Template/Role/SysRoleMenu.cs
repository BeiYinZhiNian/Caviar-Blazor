using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    public partial class SysRoleMenu:SysBaseModel
    {
        public virtual SysRole Role { get; set; }

        public virtual SysPowerMenu Menu { get; set; }

        public int MenuId { get; set; }

        public int RoleId { get; set; }
    }
}
