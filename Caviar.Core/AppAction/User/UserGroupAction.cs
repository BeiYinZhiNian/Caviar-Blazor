using Caviar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.UserGroup
{
    public partial class UserGroupAction
    {
        public override List<ViewUserGroup> ToViewModel(List<SysUserGroup> model)
        {
            model.AToB(out List<ViewUserGroup> viewModel);
            viewModel = viewModel.ListToTree();
            return viewModel;
        }
    }
}
