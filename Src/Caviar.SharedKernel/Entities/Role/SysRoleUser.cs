using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    [DisplayName("SysRoleUser")]
    public partial class SysRoleUser : SysBaseEntity, IBaseEntity
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }
    }
}
