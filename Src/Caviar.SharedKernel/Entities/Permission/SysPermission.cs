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
        [StringLength(200, ErrorMessage = "LengthErrorMsg")]
        public string Permission { get; set; }

        [StringLength(200, ErrorMessage = "LengthErrorMsg")]
        public string Entity { get; set; }

        public PermissionType PermissionType { get; set; }
    }
}
