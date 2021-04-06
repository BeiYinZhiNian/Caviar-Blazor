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
        public virtual string Login()
        {
            if (string.IsNullOrEmpty(Password) || Password.Length != 32) return "您的密码不能为空";
            SysUserLogin userLogin = null;
            if (!string.IsNullOrEmpty(UserName))
            {
                userLogin = _controllerModel.DataContext.GetEntityAsync<SysUserLogin>(u => u.UserName == UserName && u.Password == Password).FirstOrDefault();
            }
            else if (!string.IsNullOrEmpty(PhoneNumber))
            {
                userLogin = _controllerModel.DataContext.GetEntityAsync<SysUserLogin>(u => u.PhoneNumber == PhoneNumber && u.Password == Password).FirstOrDefault();
            }
            if (userLogin == null) return "用户名或密码错误";
            this.AutoAssign(userLogin);
            _controllerModel.SysUserInfo.SysUserLogin = userLogin;
            return "登录成功，欢迎回来";
        }

        public virtual bool Register(out string msg)
        {
            var count = _controllerModel.DataContext.GetEntityAsync<SysUserLogin>(u => u.UserName == UserName).Count();
            if (count > 0)
            {
                msg = "该用户名已经被注册！";
                return false;
            }
            count = _controllerModel.DataContext.GetEntityAsync<SysUserLogin>(u => u.PhoneNumber == PhoneNumber).Count();
            if (count > 0)
            {
                msg = "该手机号已经被注册！";
                return false;
            }
            count = _controllerModel.DataContext.AddEntityAsync(this).Result;
            if (count <= 0)
            {
                msg = "注册账号失败！";
                return false;
            }
            msg = "账号注册成功";
            return true;
        }
    }
}
