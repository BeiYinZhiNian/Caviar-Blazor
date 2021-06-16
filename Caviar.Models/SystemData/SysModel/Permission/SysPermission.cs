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
        /// <summary>
        /// 对应的权限id，需要type来区分id种类
        /// </summary>
        public int PermissionId { get; set; }
        /// <summary>
        /// 权限类型
        /// </summary>
        public PermissionType PermissionType { get; set; }
        /// <summary>
        /// 对应的权限身份id，需要用identity来区分权限
        /// </summary>
        public int IdentityId { get; set; }
        /// <summary>
        /// 权限身份
        /// </summary>
        public PermissionIdentity PermissionIdentity { get; set; }
    }

    
}
