using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    public partial class SysRoleLogin : SysBaseModel
    {
        [JsonIgnore]
        public virtual SysRole Role { get; set; }
        [JsonIgnore]
        public virtual SysUserLogin User { get; set; }

        public int UserId { get; set; }

        public int RoleId { get; set; }
    }
}
