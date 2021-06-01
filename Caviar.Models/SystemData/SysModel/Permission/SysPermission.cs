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
    [DisplayName("权限")]
    public class SysPermission : SysBaseModel
    {
        [JsonIgnore]
        public virtual List<SysPermissionMenu> SysPermissionMenus { get; set; }
    }

    
}
