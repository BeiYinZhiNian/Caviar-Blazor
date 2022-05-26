// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Collections.Generic;

namespace Caviar.SharedKernel.Entities.View
{
    public partial class SysMenuView : ITree<SysMenuView>
    {
        public int ParentId { get => Entity.ParentId; }

        public int Id { get => Entity.Id; }
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
        public string DisplayName { get; set; }
    }
}
