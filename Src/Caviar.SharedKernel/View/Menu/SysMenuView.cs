﻿using Caviar.SharedKernel.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities.View
{
    public partial class SysMenuView : ITree<SysMenuView>
    {
        public int ParentId { get => Entity.ParentId; }

        public int Id { get=>Entity.Id; }
        /// <summary>
        /// 孩子节点
        /// </summary>
        [DisplayName("孩子节点")]
        public List<SysMenuView> Children { get; set; } = new List<SysMenuView>();
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