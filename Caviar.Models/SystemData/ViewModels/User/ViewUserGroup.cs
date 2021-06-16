using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    public partial class ViewUserGroup:ITree<ViewUserGroup>
    {
        public List<ViewUserGroup> Children { get; set; } = new List<ViewUserGroup>();
    }
}
