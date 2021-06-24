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
    }
}
