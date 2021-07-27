using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Caviar.Models
{
    [DisplayName("用户角色")]
    public partial class SysRoleUser : SysBaseModel
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }
    }
}
