using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models
{
    public partial class ViewUserGroup:ITree<ViewUserGroup>
    {
        [DisplayName("孩子节点")]
        public List<ViewUserGroup> Children { get; set; } = new List<ViewUserGroup>();
    }
}
