using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control
{
    public partial class SysUserLoginAction : SysUserLogin
    {
        IBaseControllerModel _controllerModel;
        public SysUserLoginAction()
        {
            _controllerModel = this.GetControllerModel();
        }
        public SysUserInfo Login()
        {

            if(string.IsNullOrEmpty(Password) || Password.Length != 32) return null;
            SysUserLogin userLogin = null;
            if (!string.IsNullOrEmpty(UserName))
            {
                userLogin = _controllerModel.DataContext.GetEntityAsync<SysUserLogin>(u => u.UserName == UserName && u.Password == Password).FirstOrDefault();
            }
            else if (!string.IsNullOrEmpty(PhoneNumber))
            {
                userLogin = _controllerModel.DataContext.GetEntityAsync<SysUserLogin>(u => u.PhoneNumber == PhoneNumber && u.Password == Password).FirstOrDefault();
            }
            if (userLogin == null) return null;
            _controllerModel.SysUserInfo.SysUserLogin = userLogin;
            _controllerModel.SysUserInfo.IsLogin = true;
            _controllerModel.HttpContext.Session.Set(CaviarConfig.SessionUserInfoName, _controllerModel.SysUserInfo);
            return _controllerModel.SysUserInfo;
        }
    }
}
