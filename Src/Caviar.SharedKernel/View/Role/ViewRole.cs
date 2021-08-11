using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.View
{
    public partial class ViewRole : ITree<ViewRole>
    {
        public int ParentId { get => Entity.ParentId; set => Entity.ParentId = value; }

        public int Id { get => Entity.Id; }
        /// <summary>
        /// 孩子节点
        /// </summary>
        [DisplayName("孩子节点")]
        public List<ViewRole> Children { get; set; } = new List<ViewRole>();

        public bool IsPermission { get; set; }
    }
}
