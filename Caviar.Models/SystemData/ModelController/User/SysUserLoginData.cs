using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    public partial class SysUserLogin
    {
        public SysUserInfo Login()
        {
            if(string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password)) return null;
            if(Password.Length!=32) return null;
            var userLogin = Model.DataContext.GetEntityAsync<SysUserLogin>(u => u.UserName == UserName && u.Password == Password).FirstOrDefault();
            if (userLogin == null) return null;
            Model.SysUserInfo.SysUserLogin = userLogin;
            Model.SysUserInfo.IsLogin = true;
            return Model.SysUserInfo;
        }
    }
}
