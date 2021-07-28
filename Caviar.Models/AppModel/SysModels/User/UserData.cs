using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models
{
    /// <summary>
    /// 储存用户数据
    /// 这部分数据会进行缓存
    /// </summary>
    public partial class UserData
    {
        /// <summary>
        /// 用户所有角色
        /// </summary>
        public List<SysRole> Roles { get; set; }
        /// <summary>
        /// 用户所有权限
        /// </summary>
        public List<SysPermission> Permissions { get; set; }
        /// <summary>
        /// 用户所有菜单
        /// </summary>
        public List<SysMenu> Menus { get; set; }
        /// <summary>
        /// 用户所在用户组
        /// </summary>
        public SysUserGroup UserGroup { get; set; }
        /// <summary>
        /// 下级用户组
        /// </summary>
        public List<SysUserGroup> SubordinateUserGroup { get; set; }
        /// <summary>
        /// 用户拥有字段
        /// </summary>
        public List<SysModelFields> ModelFields { get; set; }
    }
}
