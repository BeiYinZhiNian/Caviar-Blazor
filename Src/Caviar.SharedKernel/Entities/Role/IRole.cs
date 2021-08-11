using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    public interface IRole:IBaseEntity
    {
        public string Name { get; set; }
        public int ParentId { get; set; }
    }
}
