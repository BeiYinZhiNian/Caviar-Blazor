using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    [DisplayName("角色用户组")]
    public class SysRoleUserGroup : SysBaseModel
    {
        [JsonIgnore]
        public virtual SysRole Role { get; set; }
        [JsonIgnore]
        public virtual SysUserGroup UserGroup { get; set; }

        public int UserGroupId { get; set; }

        public int RoleId { get; set; }
    }
}
