using Caviar.SharedKernel.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.View
{
    public partial class ViewMenu: ITree<ViewMenu>
    {
        public int ParentId { get => Entity.ParentId; set => Entity.ParentId = value; }

        public int Id { get=>Entity.Id; }
        /// <summary>
        /// 孩子节点
        /// </summary>
        [DisplayName("孩子节点")]
        public List<ViewMenu> Children { get; set; } = new List<ViewMenu>();
        /// <summary>
        /// 是否全部删除，包括孩子节点
        /// </summary>
        [DisplayName("全部删除")]
        public bool IsDeleteAll { get; set; } = false;
        /// <summary>
        /// 是否授权
        /// </summary>
        [DisplayName("是否授权")]
        public bool IsPermission { get; set; }
    }
}
