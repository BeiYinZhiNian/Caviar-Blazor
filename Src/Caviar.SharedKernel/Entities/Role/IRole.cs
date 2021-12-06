using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    public class IRole: SysBaseEntity,IBaseEntity
    {
        //
        // 摘要:
        //     Gets or sets the name for this role.
        public virtual string Name { get; set; }
        //
        // 摘要:
        //     Gets or sets the normalized name for this role.
        public virtual string NormalizedName { get; set; }
        //
        // 摘要:
        //     A random value that should change whenever a role is persisted to the store
        public virtual string ConcurrencyStamp { get; set; }

        public virtual int ParentId { get; set; }
    }
}
