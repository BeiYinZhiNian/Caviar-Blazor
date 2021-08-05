using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    /// <summary>
    /// 角色表
    /// </summary>
    [DisplayName("SysRole")]
    public partial class SysRole : SysBaseEntity, IBaseEntity
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [Required(ErrorMessage = "RequiredErrorMsg")]
        [DisplayName("RoleName")]
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string RoleName { get; set; }

        [DisplayName("ParentId")]
        public int ParentId { get; set; }

    }
}
