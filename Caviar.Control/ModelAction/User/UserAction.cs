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
        public override List<ViewUser> ModelToViewModel(List<SysUser> model)
        {
            var viewModel = base.ModelToViewModel(model);
            foreach (var item in viewModel)
            {
                var userGroup = BC.DC.GetSingleEntityAsync<SysUserGroup>(u => u.Id == item.UserGroupId).Result;
                if (userGroup == null) continue;
                item.UserGroupName = userGroup.Name;
            }
            return viewModel;
        }

        public async Task<ResultMsg> UpdatePwd(UserPwd userPwd)
        {
            var user = await GetEntity(BC.UserToken.Id);
            ResultMsg result = new ResultMsg();
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
