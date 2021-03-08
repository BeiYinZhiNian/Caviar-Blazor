using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData.Template.User
{
    public class SysUserInfo
    {
        public SysUserLogin SysUserLogin { get; set; }

        public List<SysRole> SysRoles { get; set; }

        public List<SysPowerMenu> SysPowerMenus { get; set; }

        /// <summary>
        /// 用户是否登录
        /// </summary>
        public bool IsLogin { get; set; }
    }
}
