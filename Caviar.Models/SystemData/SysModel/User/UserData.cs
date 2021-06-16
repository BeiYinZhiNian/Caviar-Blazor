using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    /// <summary>
    /// 储存用户数据
    /// 这部分数据会进行缓存
    /// </summary>
    public partial class UserData
    {
        public List<SysRole> Roles { get; set; }
        public List<SysPermission> Permissions { get; set; }
        public List<SysMenu> Menus { get; set; }
    }
}
