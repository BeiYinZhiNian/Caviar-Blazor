using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    /// <summary>
    /// 权限表
    /// </summary>
    public class SysPermission : SysBaseModel
    {
        /// <summary>
        /// 权限类型
        /// </summary>
        [DisplayName("权限类型")]
        public PermissionType PermissionType { get; set; }
        [JsonIgnore]
        public virtual List<SysPermissionMenu> SysPermissionMenus { get; set; }
    }

    
}
