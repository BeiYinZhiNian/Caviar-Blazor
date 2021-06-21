using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    public partial class ViewRole : ITree<ViewRole>
    {
        public List<ViewRole> Children { get; set; } = new List<ViewRole>();
        /// <summary>
        /// 是否授权
        /// </summary>
        public bool IsPermission { get; set; }
    }
}
