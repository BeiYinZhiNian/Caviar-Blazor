using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData.Template
{
    public partial class Sys_Role_Login : Sys_BaseModel
    {
        public Sys_Role Role { get; set; }

        public Sys_User_Login User { get; set; }

        public int UserId { get; set; }

        public int RoleId { get; set; }
    }
}
