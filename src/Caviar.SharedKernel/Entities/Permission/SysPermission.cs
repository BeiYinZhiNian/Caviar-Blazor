using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    public class SysPermission : SysBaseEntity
    {
        [StringLength(200)]
        public string Permission { get; set; }
        /// <summary>
        /// 实体id
        /// </summary>
        [StringLength(200)]
        public int Entity { get; set; }
        [Key]
        public PermissionType PermissionType { get; set; }
    }
}
