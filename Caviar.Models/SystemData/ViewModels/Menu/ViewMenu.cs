using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    public partial class ViewMenu: ITree<ViewMenu>
    {
        /// <summary>
        /// 孩子节点
        /// </summary>
        public List<ViewMenu> Children { get; set; } = new List<ViewMenu>();
        /// <summary>
        /// 是否全部删除，包括孩子节点
        /// </summary>
        public bool IsDeleteAll { get; set; } = false;
        /// <summary>
        /// 是否授权
        /// </summary>
        public bool IsPermission { get; set; }
    }
}
