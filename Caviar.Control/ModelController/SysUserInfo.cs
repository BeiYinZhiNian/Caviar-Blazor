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
            GetRoleJurisdiction();
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
                if (SysUserLogin.UserRoles != null)
                {
                    foreach (var role in SysUserLogin.UserRoles)
                    {
                        SysRoles.Add(role.Role);
                        if (role.Role.RoleMenus != null)
                        {
                            foreach (var menu in role.Role.RoleMenus)
                            {
                                SysPowerMenus.Add(menu.Menu);
                            }
                        }
                    }
                }
                _sysUserLogin = sysUserLogin;
                IsLogin = true;
            }
            else
            {
                _sysUserLogin = new SysUserLogin();
                //获取未登录角色权限
                _sysUserLogin.UserName = CaviarConfig.NoLoginRole;
                var role = _controllerModel.DataContext.GetEntityAsync<SysRole>(u => u.RoleName == CaviarConfig.NoLoginRole);
                SysRoles.AddRange(role);
                foreach (var item in SysRoles)
                {
                    var menus = _controllerModel.DataContext.GetEntityAsync<SysRoleMenu>(u => u.RoleId == item.Id).FirstOrDefault();
                    if (menus == null) continue;
                    SysPowerMenus.Add(menus.Menu);
                }
                IsLogin = false;
            }
            _controllerModel.HttpContext.Session.Set(CaviarConfig.SessionUserInfoName, _sysUserLogin);
        }

        public List<SysRole> SysRoles { get; private set; }


        public List<SysPowerMenu> SysPowerMenus { get; private set; }

        /// <summary>
        /// 用户是否登录
        /// </summary>
        public bool IsLogin { get; private set; }
    }
}
