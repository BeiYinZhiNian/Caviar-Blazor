using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData.Template.Role
{
    [DisplayName("角色权限")]
    public class SysRolePermission : SysBaseModel
    {
        public int RoleId { get; set; }

        public int PermissionId { get; set; }

        [JsonIgnore]
        public virtual SysRole Role { get; set; }
        [JsonIgnore]
        public virtual SysPermission Permission { get; set; }
    }
}
