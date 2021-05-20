using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    public partial class ViewMenu : SysPowerMenu,ITree<ViewMenu>
    {
        public List<ViewMenu> Children { get; set; } = new List<ViewMenu>();
    }
}
