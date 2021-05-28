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
        public RoleAction()
        {
            TransformationEvent += RoleAction_TransformationEvent;
        }

        private PageData<ViewRole> RoleAction_TransformationEvent(PageData<SysRole> model)
        {
            var pages = CommonHelper.AToB<PageData<ViewRole>, PageData<SysRole>>(model);
            if (pages.Total != 0)
            {
                var viewMenus = new List<ViewRole>().ListAutoAssign(model.Rows);
                pages.Rows = viewMenus.ListToTree();
            }
            return pages;
        }
    }
}
