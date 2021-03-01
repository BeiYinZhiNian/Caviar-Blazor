using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData.Template.User
{
    public class Sys_User_Info
    {
        public Sys_User_Login Sys_User_Login { get; set; }

        public Sys_Role[] Sys_Roles { get; set; }

        public Sys_Power_Menu[] Sys_Power_Menus { get; set; }
    }
}
