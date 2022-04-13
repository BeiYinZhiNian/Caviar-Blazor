using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    /// <summary>
    /// 部门/用户组
    /// </summary>
    public class SysUserGroup : SysUseEntity
    {
        /// <summary>
        /// 用户组名称
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// 父id
        /// </summary>
        public int ParentId { get; set; }
    }
}
