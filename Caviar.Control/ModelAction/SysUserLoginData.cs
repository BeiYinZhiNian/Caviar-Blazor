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
        public SysUserInfo Login()
        {
            if(string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password)) return null;
            if(Password.Length!=32) return null;
            var userLogin = this.GetBaseModel().DataContext.GetEntityAsync<SysUserLogin>(u => u.UserName == UserName && u.Password == Password).FirstOrDefault();
            if (userLogin == null) return null;
            this.GetBaseModel().SysUserInfo.SysUserLogin = userLogin;
            this.GetBaseModel().SysUserInfo.IsLogin = true;
            return this.GetBaseModel().SysUserInfo;
        }
    }
}
