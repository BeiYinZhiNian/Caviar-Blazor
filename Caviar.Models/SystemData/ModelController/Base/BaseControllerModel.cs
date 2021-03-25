using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    [DIInject(InjectType.SCOPED)]
    public class BaseControllerModel
    {
        /// <summary>
        /// 数据上下文
        /// </summary>
        public IDataContext DataContext { get; set; }
        /// <summary>
        /// 日志记录
        /// </summary>
        public ILogger Logger { get; set; } 
        /// <summary>
        /// 当前请求路径
        /// </summary>
        public string Current_Action { get; set; }
        /// <summary>
        /// 当前请求ip地址
        /// </summary>
        public string Current_Ipaddress { get; set; }
        /// <summary>
        /// 当前请求的完整Url
        /// </summary>
        public string Current_AbsoluteUri { get; set; }

        SysUserInfo _sysUserInfo;
        /// <summary>
        /// 当前用户信息
        /// </summary>
        public SysUserInfo SysUserInfo 
        {
            get 
            {
                if (_sysUserInfo == null)
                {
                    _sysUserInfo = new SysUserInfo
                    {
                        SysUserLogin = new SysUserLogin(),
                        SysRoles = new List<SysRole>(),
                        SysPowerMenus = new List<SysPowerMenu>(),
                        IsLogin = false
                    };
                }
                return _sysUserInfo;
            }
            set 
            {
                _sysUserInfo = value;
            } 
        }
    }
}
