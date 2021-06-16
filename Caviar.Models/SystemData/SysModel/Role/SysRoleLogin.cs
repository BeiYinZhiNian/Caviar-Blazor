using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    [DisplayName("用户角色")]
    public partial class SysRoleLogin : SysBaseModel
    {
        [JsonIgnore]
        public virtual SysRole Role { get; set; }
        [JsonIgnore]
        public virtual SysUser User { get; set; }

        public int UserId { get; set; }

        public int RoleId { get; set; }
    }
}
