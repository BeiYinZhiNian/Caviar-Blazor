using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    public partial class SysBaseModel
    {
        [NotMapped]
        public IBaseControllerModel Model { get; set; }
    }
}
