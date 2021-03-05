using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData.Template
{
    public partial class Sys_Role_Menu:Sys_BaseModel
    {
        public Sys_Role Role { get; set; }

        public Sys_Power_Menu Menu { get; set; }

        public int MenuId { get; set; }

        public int RoleId { get; set; }
    }
}
