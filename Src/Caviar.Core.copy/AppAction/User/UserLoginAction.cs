using Caviar.Core.ModelAction;
using Caviar.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.User
{
    public partial class UserAction
    {
        /// <summary>
        /// 登录成功返回token，失败返回错误原因
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultMsg<UserToken>> Login(ViewUser entity)
        {

            if (string.IsNullOrEmpty(entity.Password) || entity.Password.Length != 64) return Error<UserToken>("用户名或密码错误");
            SysUser userLogin = null;
            //由于未登录，需要使用isDataPermissions:false关闭数据权限进行数据获取
            if (!string.IsNullOrEmpty(entity.UserName))
            {
                userLogin = await Interactor.DbContext.GetSingleEntityAsync<SysUser>(u => u.UserName == entity.UserName && u.Password == entity.Password,isDataPermissions:false);
            }
            else if (!string.IsNullOrEmpty(entity.PhoneNumber))
            {
                userLogin = await Interactor.DbContext.GetSingleEntityAsync<SysUser>(u => u.PhoneNumber == entity.PhoneNumber && u.Password == entity.Password, isDataPermissions: false);
            }
            if (userLogin == null) return Error<UserToken>("用户名或密码错误");
            if (userLogin.IsDisable) return Error<UserToken>("该账号未被启动，请联系管理员");
            Interactor.UserToken.AutoAssign(userLogin);
            Interactor.UserToken.Duration = CaviarConfig.TokenConfig.Duration;
            Interactor.UserToken.Token = JwtHelper.CreateTokenByHandler(Interactor.UserToken);
            return Ok("登录成功，欢迎回来",Interactor.UserToken);
        }
    }
}
