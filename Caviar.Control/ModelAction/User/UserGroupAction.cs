using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control.UserGroup
{
    public partial class UserGroupAction
    {
        public override List<ViewUserGroup> ModelToViewModel(List<SysUserGroup> model)
        {
            model.AToB(out List<ViewUserGroup> viewModel);
            viewModel = viewModel.ListToTree();
            return viewModel;
        }
    }
}
