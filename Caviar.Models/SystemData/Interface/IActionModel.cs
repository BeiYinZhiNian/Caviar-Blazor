using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    public interface IActionModel
    {
        public IInteractor BC { get; set; }
    }
}
