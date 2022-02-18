using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities.View
{
    public partial class SysUserGroupView: ITree<SysUserGroupView>
    {
        public int ParentId { get => Entity.ParentId; }

        public int Id { get => Entity.Id; }
        /// <summary>
        /// 孩子节点
        /// </summary>
        public List<SysUserGroupView> Children { get; set; } = new List<SysUserGroupView>();
        /// <summary>
        /// 是否授权
        /// </summary>
        public bool IsPermission { get; set; }
    }
}
