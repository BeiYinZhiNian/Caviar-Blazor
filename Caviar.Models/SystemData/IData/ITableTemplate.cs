using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    public interface ITableTemplate
    {
        public Task<bool> Validate();
    }
}
