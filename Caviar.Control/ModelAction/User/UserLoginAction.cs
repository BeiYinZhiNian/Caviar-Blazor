using Caviar.Control.ModelAction;
using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control
{
    public partial class UserLoginAction : BaseModelAction<SysUserLogin,ViewUserLogin>
    {
        /// <summary>
        /// 登录成功返回token，失败返回错误原因
        /// </summary>
        /// <returns></returns>
        public virtual string Login()
        {

            if (string.IsNullOrEmpty(Entity.Password) || Entity.Password.Length != 64) return "用户名或密码错误";
            SysUserLogin userLogin = null;
            if (!string.IsNullOrEmpty(Entity.UserName))
            {
                userLogin = BC.DC.GetFirstEntityAsync<SysUserLogin>(u => u.UserName == Entity.UserName && u.Password == Entity.Password).Result;
            }
            else if (!string.IsNullOrEmpty(Entity.PhoneNumber))
            {
                userLogin = BC.DC.GetFirstEntityAsync<SysUserLogin>(u => u.PhoneNumber == Entity.PhoneNumber && u.Password == Entity.Password).Result;
            }
            if (userLogin == null) return "用户名或密码错误";
            BC.UserToken.AutoAssign(userLogin);
            BC.UserToken.CreateTime = DateTime.Now;
            BC.UserToken.Token = CaviarConfig.GetUserToken(BC.UserToken);
            BC.UserToken.Duration = CaviarConfig.TokenDuration;
            return BC.UserToken.Token;
        }

        public virtual bool Register(out string msg)
        {
            var user = BC.DC.GetFirstEntityAsync<SysUserLogin>(u => u.UserName == Entity.UserName).Result;
            if (user != null)
            {
                msg = "该用户名已经被注册！";
                return false;
            }
            user = BC.DC.GetFirstEntityAsync<SysUserLogin>(u => u.PhoneNumber == Entity.PhoneNumber).Result;
            if (user != null)
            {
                msg = "该手机号已经被注册！";
                return false;
            }
            var count = BC.DC.AddEntityAsync(Entity).Result;
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
