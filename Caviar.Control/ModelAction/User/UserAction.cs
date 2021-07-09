using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control.User
{
    public partial class UserAction
    {
        public override List<ViewUser> ToViewModel(List<SysUser> model)
        {
            var viewModel = base.ToViewModel(model);
            foreach (var item in viewModel)
            {
                var userGroup = BC.DC.GetSingleEntityAsync<SysUserGroup>(u => u.Id == item.UserGroupId).Result;
                if (userGroup == null) continue;
                item.UserGroupName = userGroup.Name;
            }
            return viewModel;
        }
        /// <summary>
        /// 修改自身信息
        /// </summary>
        /// <param name="viewUser"></param>
        /// <returns></returns>
        public async Task<ResultMsg> UpateMyData(ViewUser viewUser)
        {
            if (viewUser.Id != BC.UserToken.Id || viewUser.Uid != BC.UserToken.Uid)
            {
                return Error("正在进行非法修改");
            }
            var user = await BC.DC.GetSingleEntityAsync<SysUser>(u => u.UserName == viewUser.UserName && u.Uid != BC.UserToken.Uid);
            if (user != null)
            {
                return Error("该用户名已有人使用，请重新绑定");
            }
            user = await BC.DC.GetSingleEntityAsync<SysUser>(u => u.PhoneNumber == viewUser.PhoneNumber && u.Uid != BC.UserToken.Uid);
            if (user != null)
            {
                return Error("该手机号已有人使用，请重新绑定");
            }
            user = await BC.DC.GetSingleEntityAsync<SysUser>(u => u.EmailNumber == viewUser.EmailNumber && u.Uid != BC.UserToken.Uid);
            if (user != null)
            {
                return Error("该邮箱已有人使用，请重新绑定");
            }
            return await UpdateEntity(viewUser);
        }

        public async Task<ResultMsg> UpdatePwd(UserPwd userPwd)
        {
            var result = await GetEntity(BC.UserToken.Id);
            var user = result.Data;
            if (user.Password != userPwd.OriginalPwd)
            {
                result.Status = 403;
                result.Title = "初始密码输入错误";
                return result;
            }
            else if(userPwd.NewPwd != userPwd.SurePwd)
            {
                result.Status = 403;
                result.Title = "两次密码不一致";
                return result;
            }
            user.Password = userPwd.NewPwd;
            await BC.DC.UpdateEntityAsync(user, false);
            await BC.DC.SaveChangesAsync(IsFieldCheck: false);
            return result;
        }
    }
}
