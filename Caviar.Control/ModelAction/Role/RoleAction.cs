using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caviar.Models.SystemData;

namespace Caviar.Control.Role
{
    public partial class RoleAction
    {
        partial void PartialModelToViewModel(ref bool isContinue, PageData<SysRole> model, ref PageData<ViewRole> outModel)
        {
            outModel = CommonHelper.AToB<PageData<ViewRole>, PageData<SysRole>>(model);
            if (outModel.Total != 0)
            {
                var viewMenus = new List<ViewRole>().ListAutoAssign(model.Rows);
                outModel.Rows = viewMenus.ListToTree();
            }
            isContinue = false;
        }
    }
}
