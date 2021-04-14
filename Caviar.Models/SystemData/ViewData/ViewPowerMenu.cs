using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    public partial class ViewPowerMenu : SysPowerMenu
    {
        public List<ViewPowerMenu> SonMenu { get; set; } = new List<ViewPowerMenu>();
    }
}
