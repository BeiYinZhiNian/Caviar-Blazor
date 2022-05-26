// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Collections.Generic;

namespace Caviar.SharedKernel.Entities
{
    public interface ITree<T>
    {
        /// <summary>
        /// id
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// 父id
        /// </summary>
        public int ParentId { get; }
        /// <summary>
        /// 孩子节点
        /// </summary>
        public List<T> Children { get; set; }
    }
}
