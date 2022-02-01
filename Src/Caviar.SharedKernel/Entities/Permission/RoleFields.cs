using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    public class RoleFields : SysBaseEntity
    {
        public int FieldId { get; set; }

        public int RoleId { get; set; }
    }
}
