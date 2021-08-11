using Caviar.SharedKernel.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.View
{
    public partial class ViewUserGroup: ITree<ViewUserGroup>
    {

        public int Id { get=>Entity.Id;  }

        public int ParentId { get => Entity.ParentId; set => Entity.ParentId = value; }

        [DisplayName("孩子节点")]
        public List<ViewUserGroup> Children { get; set; } = new List<ViewUserGroup>();
    }
}
