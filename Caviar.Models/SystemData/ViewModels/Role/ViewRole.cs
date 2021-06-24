using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    public partial class ViewRole : ITree<ViewRole>
    {
        [DisplayName("孩子节点")]
        public List<ViewRole> Children { get; set; } = new List<ViewRole>();
        /// <summary>
        /// 是否授权
        /// </summary>
        [DisplayName("是否授权")]
        public bool IsPermission { get; set; }
    }
}
