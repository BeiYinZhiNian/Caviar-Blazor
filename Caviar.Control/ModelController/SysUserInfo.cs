using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caviar.Models.SystemData;
using Microsoft.Extensions.DependencyInjection;

namespace Caviar.Control
{
    [DIInject]
    public class SysUserInfo
    {
        IBaseControllerModel _controllerModel;
        public SysUserInfo()
        {
            _controllerModel = CaviarConfig.ApplicationServices.GetRequiredService<BaseControllerModel>();
            GetRoleJurisdiction(null);
        }
        SysUserLogin _sysUserLogin;
        public SysUserLogin SysUserLogin
        {
            get
            {
                if (_sysUserLogin == null)
                {
                    GetRoleJurisdiction(null);
                }
                return _sysUserLogin;
            }
            set
            {
                GetRoleJurisdiction(value);
            }
        }

        void GetRoleJurisdiction(SysUserLogin sysUserLogin = null)
        {
            SysRoles = new List<SysRole>();
            SysPowerMenus = new List<SysPowerMenu>();
            if (sysUserLogin != null && sysUserLogin.Id > 0)
            {
                if (sysUserLogin.UserRoles != null)
                {
                    foreach (var role in sysUserLogin.UserRoles)
                    {
                        SysRoles.Add(role.Role);
                    }
                }
                _sysUserLogin = sysUserLogin;
            }
            else
            {
                _sysUserLogin = new SysUserLogin();
                //获取未登录角色权限
                _sysUserLogin.UserName = "未登录用户";
                var role = _controllerModel.DataContext.GetEntityAsync<SysRole>(u => u.Uid == CaviarConfig.NoLoginRoleGuid);
                SysRoles.AddRange(role);
            }
        }

        public List<SysRole> SysRoles { get; private set; }


        public List<SysPowerMenu> SysPowerMenus { get; private set; }

        /// <summary>
        /// 用户是否登录
        /// </summary>
        public bool IsLogin => SysUserLogin.Id > 0;
    }
}
