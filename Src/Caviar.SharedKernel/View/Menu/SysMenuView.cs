using Caviar.SharedKernel.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Caviar.SharedKernel.View
{
    public partial class SysMenuView : ITree<SysMenuView>
    {
        public int ParentId { get => Entity.ParentId; }

        public int Id { get=>Entity.Id; }
        /// <summary>
        /// 孩子节点
        /// </summary>
        public List<SysMenuView> Children { get; set; } = new List<SysMenuView>();
        /// <summary>
        /// 是否全部删除，包括孩子节点
        /// </summary>
        public bool IsDeleteAll { get; set; } = false;
        /// <summary>
        /// 是否授权
        /// </summary>
        public bool IsPermission { get; set; }

        /// <summary>
        /// 用于menuName的显示，读取与语言配置文件
        /// </summary>
        [Required(ErrorMessage = "RequiredErrorMsg")]
        public string DisplayName { get; set; }
    }
}
