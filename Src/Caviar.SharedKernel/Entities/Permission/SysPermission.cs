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
        public int PermissionId { get; set; }

        public string EntityName { get; set; }

        public PermissionType PermissionType { get; set; }
    }
}
